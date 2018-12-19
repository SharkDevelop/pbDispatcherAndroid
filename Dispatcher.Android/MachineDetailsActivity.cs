﻿using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using DataUtils;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Xamarin.Android;
using System;
using System.Collections.Generic;
using Android.Content;
using Android.Support.V7.Widget;
using Dispatcher.Android.Appl;
using Dispatcher.Android.Helpers;
using Dispatcher.Android.Utils;

namespace Dispatcher.Android
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class MachineDetailsActivity : BaseActivity
    {
        private const float UpdateInterval = 100f;
        private TimerHolder _timerHolder;
        private byte _needDataUpdate;
        private DateTime _lastUpdateTime = DateTime.MinValue;

        private Machine _machine;
        private readonly List<MachineStatesLogElement> _statesLogs = new List<MachineStatesLogElement>();

        private RecyclerView _rvMachineStatesLog;
        private RecyclerView.LayoutManager _layoutManager;
        private MachineStatesAdapter _adapter;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            _timerHolder = new TimerHolder(UpdateInterval, CheckNewData);

            SetContentView(Resource.Layout.activity_machine_details);
            
            FindViewById<TextView>(Resource.Id.tvService)
                .Click += (sender, args) => StartServiceRequestActivity();
            
            FindViewById<ImageView>(Resource.Id.ivSensorAlerts)
                .Click += (sender, args) => StartSensorAlertsActivity();
            
            InitCurrentMachine();
            InitMachineStatesLogListView();
            InitHistoryPlot();
        }

        protected override void OnStart()
        {
            base.OnStart();
            
            DataManager.SheduleGetMachineStatesLogRequest(
                _machine, 
                Settings.machineStatesLogMaxElements, 
                DataUpdateCallback);
            
            _timerHolder.Start();
            FillList();
        }

        protected override void OnStop()
        {
            base.OnStop();
            _timerHolder.Stop();
        }

        private void InitCurrentMachine()
        {
            _machine = AppSession.SelectedMachine;
            InitActionBar(_machine.GetNameStr());

            string sensorsCount = null;
            if (_machine.sensors.Count != 0)
            {
                sensorsCount = " (" + BitConverter.ToString(BitConverter.GetBytes(_machine.sensors[0].nodeID), 1, 1)
                    + BitConverter.ToString(BitConverter.GetBytes(_machine.sensors[0].nodeID), 0, 1) + ")";
            }               

            FindViewById<TextView>(Resource.Id.tvMachineNumber)
                .Text = $"Инв. №: {_machine.inventoryID}{sensorsCount}";
        }

        private void InitMachineStatesLogListView()
        {
            _rvMachineStatesLog = FindViewById<RecyclerView>(Resource.Id.rvMachineStates);
            _layoutManager = new LinearLayoutManager(this);
            _rvMachineStatesLog.SetLayoutManager(_layoutManager);
            
            _adapter = new MachineStatesAdapter(_statesLogs);
            _rvMachineStatesLog.SetAdapter(_adapter);
            _adapter.ItemClicked += ShowSateDetails;
        }

        private void InitHistoryPlot()
        {
            FindViewById<PlotView>(Resource.Id.plotHistory)
                .Model = CreatePlotModel();

            PlotModel CreatePlotModel()
            {
                var plotModel = new PlotModel { Title = "OxyPlot Demo" };

                plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
                plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Maximum = 10, Minimum = 0 });

                var series1 = new LineSeries
                {
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 4,
                    MarkerStroke = OxyColors.White
                };

                series1.Points.Add(new DataPoint(0.0, 6.0));
                series1.Points.Add(new DataPoint(1.4, 2.1));
                series1.Points.Add(new DataPoint(2.0, 4.2));
                series1.Points.Add(new DataPoint(3.3, 2.3));
                series1.Points.Add(new DataPoint(4.7, 7.4));
                series1.Points.Add(new DataPoint(6.0, 6.2));
                series1.Points.Add(new DataPoint(8.9, 8.9));

                plotModel.Series.Add(series1);

                return plotModel;
            }
        }
        
        private void DataUpdateCallback(object requestResult)
        {
            if (requestResult is List<MachineStatesLogElement> stateList)
            {
                _statesLogs.Clear();
                foreach (var item in stateList)
                {
                    _statesLogs.Add(item);
                    
                    if (item.timeEnd != DateTime.MinValue || 
                        !(item.state is MachineServiceState state)) continue;
                    
                    _machine.serviceState = state;
                    _machine.serviceStateTimeStart = item.timeStart;
                    _machine.userName = item.userName;
                }
                
                _needDataUpdate++;
            }

            if (requestResult is List<HistoryPoint>)
            {
//                sensorHistoryList.Clear();
//                foreach (var item in (List<HistoryPoint>)requestResult)
//                {
//                    sensorHistoryList.Add(item);
//                }
//                needHistoryGraphRedraw = true;
            }
        }
        
        private void CheckNewData()
        {
            if (DataManager.ConnectState == ConnectStates.AuthPassed)
                UpdateTitle(_machine.name);
            else if (DataManager.ConnectState == ConnectStates.SocketConnected)
                UpdateTitle(Resource.String.no_authorization);
            else
                UpdateTitle(Resource.String.no_connection);

//            if ((HistoryGraph.readyToDataUpdate == true) && ((sensorHistoryTimeStart != HistoryGraph.timeStart) || (sensorHistoryTimeEnd != HistoryGraph.timeEnd)))
//            {
//                HistoryGraph.readyToDataUpdate = false;
//
//                sensorHistoryTimeStart = HistoryGraph.timeStart;
//                sensorHistoryTimeEnd = HistoryGraph.timeEnd;
//
//                if (Application.selectedMachine.sensors.Count != 0)
//                    DataManager.SheduleGetSensorHistoryDataRequest(Application.selectedMachine.sensors[0], (byte)SensorValueArrayIndexes.MainValue, sensorHistoryTimeStart, sensorHistoryTimeEnd, Settings.sensorHistoryPointsCount, DataUpdateCallback);
//            }
//
//            if (needHistoryGraphRedraw == true) 
//            {
//                needHistoryGraphRedraw = false;
//                HistoryGraph.SetNeedsDisplay();
//            }

            if (_needDataUpdate > 0)
            {
                _needDataUpdate--;
                UpdateViewValues();
            }
            else if (DateTime.Now.Subtract(_lastUpdateTime).TotalMilliseconds > Settings.updatePeriodMs && 
                     DataManager.machineTypes.Count != 0 && 
                     DataManager.NotAnsweredRequestsCount == 0)
            {
                DataManager.SheduleGetMachineStatesLogRequest(
                    _machine, 
                    Settings.machineStatesLogMaxElements, 
                    DataUpdateCallback);

                _lastUpdateTime = DateTime.Now;
            }
        }
        
        private void UpdateViewValues()
        {
            RunOnUiThread(FillList);
        }
        
        private void FillList()
        {
            if (_statesLogs.Count <= 0) return;

            _adapter.NotifyDataSetChanged();
        }
        
        private void ShowSateDetails(int position)
        {
            if (_statesLogs.Count < position) return;
            
            _timerHolder.Stop();

            var selectedState = _statesLogs[position];
            
            var description = selectedState.description + "\n\n" + selectedState.timeStart;
            if (selectedState.timeEnd != DateTime.MinValue)
            {
                description += " - \n" + selectedState.timeEnd;
            }
            description += "\n" + selectedState.userName;

            var caption = selectedState.state.name;
            
            DialogHelper.ShowDialog(this, caption, description, "OK", _timerHolder.Start);
        }
        
        private void StartServiceRequestActivity()
        {
            var intent = new Intent(this, typeof(ServiceRequestActivity));
            StartActivity(intent);
        }
        
        private void StartSensorAlertsActivity()
        {
            var intent = new Intent(this, typeof(SensorAlertsActivity));
            StartActivity(intent);
        }
    }
}