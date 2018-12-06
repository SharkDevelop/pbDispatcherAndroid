using System;
using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using DataUtils;
using Timer = System.Timers.Timer;

namespace Dispatcher.Android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        private byte needDataUpdate = 0;

        DateTime lastUpdateTime = DateTime.MinValue;

        private Timer timer;

        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        MachinesAdapter mAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            DataUtils.Init();

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            //var button = FindViewById<Button>(Resource.Id.button1);

            //button.Click += (sender, args) =>
            //{
            //    Intent intent = new Intent();
            //    intent.SetClass(BaseContext, typeof(UserSettingsActivity));
            //    intent.SetFlags(ActivityFlags.ReorderToFront);
            //    StartActivity(intent);
            //};

            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);

            // Plug in the linear layout manager:
            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            mAdapter = new MachinesAdapter(DataManager.machines);
            mRecyclerView.SetAdapter(mAdapter);
            
            // MachineTable.Source = new MachinesTableSource(this);
            //MachineTable.Delegate = new 
            UpdateViewValues();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.toolbar_menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                Intent intent = new Intent();
                intent.SetClass(BaseContext, typeof(UserSettingsActivity));
                intent.SetFlags(ActivityFlags.ReorderToFront);
                StartActivity(intent);
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        protected override void OnStart()
        {
            base.OnStart();

            DataManager.SheduleGetMachinesRequest(DataUpdateCallback);
            lastUpdateTime = DateTime.Now;

            if (DataManager.LoginImpossible == true)
            {
                Intent intent = new Intent();
                intent.SetClass(BaseContext, typeof(UserSettingsActivity));
                intent.SetFlags(ActivityFlags.ReorderToFront);
                StartActivity(intent);
                return;
            }

            timer = new Timer { AutoReset = true, Interval = 1000f };
            timer.Elapsed += delegate { CheckNewData(); };
            timer.Start();

            FillList();
        }

        protected void OnListItemClick(ListView l, View v, int position, long id)
        {
          //  var t = items[position];
          //  Android.Widget.Toast.MakeText(this, t, Android.Widget.ToastLength.Short).Show();
        }

        public void CheckNewData()
        {
            if (DataManager.ConnectState == ConnectStates.AuthPassed)
            {
               // this.Title = "Все";
                if (DataManager.selectedDivision != null)
                {
               //     Title = DataManager.selectedDivision.name;
                  
                    //Window.SetTitle(DataManager.selectedDivision.name);
                }
                
                //if (DataManager.Ping < 200)
                //    pingImageView.Image = UIImage.FromFile("GreenCircle.png");
                //else if (DataManager.Ping < 500)
                //    pingImageView.Image = UIImage.FromFile("YelowCircle.png");
                //else
                //    pingImageView.Image = UIImage.FromFile("RedCircle.png");
            }
            else if (DataManager.ConnectState == ConnectStates.SocketConnected)
            {
              // this.Title = "Нет авторизации";
                //  pingImageView.Image = UIImage.FromFile("GreyCircle.png");
            }
            else
            {
               // this.Title = "Нет связи";
                //  pingImageView.Image = UIImage.FromFile("GreyCircle.png");
            }

            if (needDataUpdate > 0)
            {
                needDataUpdate--;
                UpdateViewValues();
               // pingImageView.Hidden = false;
            }
            else if ((DateTime.Now.Subtract(lastUpdateTime).TotalMilliseconds > Settings.updatePeriodMs) && (DataManager.machineTypes.Count != 0) && (DataManager.NotAnsweredRequestsCount == 0))
            {
                DataManager.SheduleGetMachinesRequest(DataUpdateCallback);
                lastUpdateTime = DateTime.Now;
                //pingImageView.Hidden = true;
            }
        }

        public void UpdateViewValues()
        {
             this.RunOnUiThread(FillList);

            //todo: добавить это
            //UIApplication.SharedApplication.ApplicationIconBadgeNumber = DataManager.problemsCount;
        }

        public void DataUpdateCallback(object requestState)
        {
            //if (requestState == RequestStates.Completed)
            needDataUpdate++;
        }

        private void FillList()
        {
            var machines = DataManager.machines;

            if (machines.Count <= 0) return;

            mAdapter.NotifyDataSetChanged();
        }
    }
}

