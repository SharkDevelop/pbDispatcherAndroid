using Android.App;
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
        private bool _needHistoryGraphRedraw;
        private DateTime _lastUpdateTime = DateTime.MinValue;

        private Machine _machine;
        private readonly List<MachineStatesLogElement> _statesLogs = new List<MachineStatesLogElement>();
        private readonly List<HistoryPoint> _sensorHistoryList = new List<HistoryPoint>();
        private DateTime _sensorHistoryTimeStart, _sensorHistoryTimeEnd;

        private RecyclerView _rvMachineStatesLog;
        private RecyclerView.LayoutManager _layoutManager;
        private MachineStatesAdapter _adapter;

        private PlotView _plotView;
        private DateTimeAxis _dateAxis;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            _timerHolder = new TimerHolder(UpdateInterval, CheckNewData);

            SetContentView(Resource.Layout.activity_machine_details);

            _plotView = FindViewById<PlotView>(Resource.Id.plotHistory);
                        
            FindViewById<TextView>(Resource.Id.tvService)
                .Click += (sender, args) => StartServiceRequestActivity();
            
            FindViewById<ImageView>(Resource.Id.ivSensorAlerts)
                .Click += (sender, args) => StartSensorAlertsActivity();
            
            InitCurrentMachine();
            InitMachineStatesLogListView();
            InitDataUpdating();
        }
        
        protected override void InitDataUpdating()
        {
            DataManager.SheduleGetMachineStatesLogRequest(
                _machine, 
                Settings.machineStatesLogMaxElements, 
                DataUpdateCallback);
            
            _lastUpdateTime = DateTime.Now;

            _sensorHistoryTimeStart = DateTime.Now.AddDays(-1);
            _sensorHistoryTimeEnd = DateTime.Now;
            
            InitHistoryPlot();
            
            if (_machine.sensors.Count != 0)
                DataManager.SheduleGetSensorHistoryDataRequest(
                    _machine.sensors[0], 
                    (byte)SensorValueArrayIndexes.MainValue, 
                    _sensorHistoryTimeStart, 
                    _sensorHistoryTimeEnd, 
                    Settings.sensorHistoryPointsCount, 
                    DataUpdateCallback);
            
            _timerHolder.Start();
            
            FillList();
        }

        protected override void OnStart()
        {
            base.OnStart();
            
            _timerHolder.Start();
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
            _plotView.Model = CreatePlotModel(_sensorHistoryTimeStart, _sensorHistoryTimeEnd);

            PlotModel CreatePlotModel(DateTime startDate, DateTime endDate)
            {
                var plotModel = new PlotModel
                {
                    Padding = new OxyThickness(0),
                    PlotAreaBorderColor = OxyColors.Transparent,                    
                };
                
                var minValue = DateTimeAxis.ToDouble(startDate);
                var maxValue = DateTimeAxis.ToDouble(endDate);                

                plotModel.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Left,
                    AxislineStyle = LineStyle.Solid,
                    AxislineColor = OxyColors.Black,
                    AxisDistance = 1,
                    IsZoomEnabled = false,
                    IsPanEnabled = false                    
                });               

                _dateAxis = new DateTimeAxis
                {
                    Position = AxisPosition.Bottom,
                    AxislineStyle = LineStyle.Solid,
                    AxislineColor = OxyColors.Black,
                    Minimum = minValue,
                    Maximum = maxValue,
                    StringFormat="HH:mm dd.MM.yy"
                };

                plotModel.Axes.Add(_dateAxis);
                
                return plotModel;
            }
        }

        private readonly object _locker = new object();
        
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

            if (requestResult is List<HistoryPoint> list)
            {
                lock (_locker)
                {
                    _sensorHistoryList.Clear();
                    foreach (var item in list)
                    {
                        _sensorHistoryList.Add(item);
                    }
                }
                
                _needHistoryGraphRedraw = true;
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
            
            if ((_sensorHistoryTimeStart != DateTimeAxis.ToDateTime(_dateAxis.ActualMinimum) || 
                _sensorHistoryTimeEnd != DateTimeAxis.ToDateTime(_dateAxis.ActualMaximum)))
            {
                _sensorHistoryTimeStart = DateTimeAxis.ToDateTime(_dateAxis.ActualMinimum);
                _sensorHistoryTimeEnd = DateTimeAxis.ToDateTime(_dateAxis.ActualMaximum);

                var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours;

                var start = DateTimeAxis.ToDateTime(_dateAxis.ActualMinimum).AddHours(-offset);
                var end = DateTimeAxis.ToDateTime(_dateAxis.ActualMaximum).AddHours(-offset);                

                if (_machine.sensors.Count != 0)
                    DataManager.SheduleGetSensorHistoryDataRequest(
                        _machine.sensors[0], 
                        (byte)SensorValueArrayIndexes.MainValue, 
                        start, 
                        end, 
                        Settings.sensorHistoryPointsCount, 
                        DataUpdateCallback);
            }

            if (_needHistoryGraphRedraw)
            {
                _needHistoryGraphRedraw = false;
                RunOnUiThread(UpdatePlot);
            }

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

        private void UpdatePlot()
        {
            _plotView.Model.Series.Clear();
            
            var series = new LineSeries
            {
                MarkerType = MarkerType.None,
                MarkerSize = 1,
                Color = OxyColor.Parse("#a7a7a7")
            };

            lock (_locker)
            {
                foreach (var point in _sensorHistoryList)
                {
                    series.Points
                        .Add(new DataPoint(DateTimeAxis.ToDouble(point.time), point.value));
                }
            }

            _plotView.Model.Series.Add(series);
            _plotView.InvalidatePlot();
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