using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using DataUtils;
using Dispatcher.Android.Utils;

namespace Dispatcher.Android
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class UserSettingsActivity : AppCompatActivity
    {
        private Timer timer;
        private byte needDataUpdate = 0;

        DateTime lastUpdateTime = DateTime.MinValue;

        private Button changeUserButton;

        private Button serverButton;

        private TextView userNameText;

        private EditText loginEdit;

        private EditText passwordEdit;

        private EditText warningHourFrom;

        private EditText warningMinuteFrom;

        private EditText warningHourTo;

        private EditText warningMinuteTo;

        private Switch warningSwitch;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.UserSettingsActivity);

            userNameText = FindViewById<TextView>(Resource.Id.userNameLabel);

            loginEdit = FindViewById<EditText>(Resource.Id.loginEdit);

            passwordEdit = FindViewById<EditText>(Resource.Id.passEdit);

            changeUserButton = FindViewById<Button>(Resource.Id.changeUserButton);

            serverButton = FindViewById<Button>(Resource.Id.serverButton);

            warningHourFrom = FindViewById<EditText>(Resource.Id.warningTime1);
            warningHourFrom.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(2), new MinMaxInputFilter(0, 23), });

            warningMinuteFrom = FindViewById<EditText>(Resource.Id.warningTime2);
            warningMinuteFrom.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(2), new MinMaxInputFilter(0, 59) });

            warningHourTo = FindViewById<EditText>(Resource.Id.warningTime3);
            warningHourTo.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(2), new MinMaxInputFilter(0, 23) });

            warningMinuteTo = FindViewById<EditText>(Resource.Id.warningTime4);
            warningMinuteTo.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(2), new MinMaxInputFilter(0, 59) });

            warningSwitch = FindViewById<Switch>(Resource.Id.warningSwitch);            

            changeUserButton.Click += (sender, args) =>
            {
                userNameText.Text = "Обновление...";

                DataManager.ChangeUser(loginEdit.Text, passwordEdit.Text, ChangeUserCallback);
            };

            serverButton.Click += (sender, args) =>
            {
                DataManager.SwitchToNextServer();
                DataUtils.StoreValues();

                serverButton.Text = "Загрузка...";

                DataManager.ReconnectToServer(ChangeServerCallback);
            };

            UpdateViewValues();
        }

        protected override void OnStart()
        {
            base.OnStart();

            //SetLayout();

            DataManager.SheduleGetUserSettingsRequest(DataUpdateCallback);

            timer = new Timer {AutoReset = true, Interval = 200f};
            timer.Elapsed += delegate { CheckNewData(); };
            timer.Start();

            if (DataManager.LoginImpossible == true)
            {
                loginEdit.Hint = "demo";
                passwordEdit.Hint = "demo";
            }

            Title = "Пользователь";
        }

        protected override void OnStop()
        {
            base.OnStop();

            if (DataManager.LoginImpossible == false)
            {
                DataManager.userSettings.notificationsFrom = new DateTime(1970, 1, 1, sbyte.Parse(warningHourFrom.Text), sbyte.Parse(warningMinuteFrom.Text), 0);
                DataManager.userSettings.notificationsTo = new DateTime(1970, 1, 1, sbyte.Parse(warningHourTo.Text), sbyte.Parse(warningMinuteTo.Text), 0);
                DataManager.userSettings.sendNodesOfflineNotifications = warningSwitch.Checked;

                DataManager.SheduleSetUserSettingsRequest(DataManager.userSettings.notificationsFrom, DataManager.userSettings.notificationsTo, DataManager.userSettings.sendNodesOfflineNotifications, DataUpdateCallback);
            }
            else
                DataManager.ChangeUser(loginEdit.Text, passwordEdit.Text, ChangeUserCallback);

            timer.Stop();
            timer.Dispose();
        }

        public void ChangeUserCallback(object requestState)
        {
            DataUtils.StoreValues();

            DataManager.SheduleGetUserSettingsRequest(DataUpdateCallback);
            DataManager.SheduleGetCommonTypesRequest(null);
            DataManager.SheduleGetDivisionsRequest(null);
        }

        public void ChangeServerCallback(object requestState)
        {
            needDataUpdate++;

            DataManager.SheduleGetCommonTypesRequest(null);
            DataManager.SheduleGetDivisionsRequest(null);
        }

        public void DataUpdateCallback(object requestState)
        {
            //if (requestState == RequestStates.Completed)
            needDataUpdate++;
        }

        private int timerCnt = 0;
        public void CheckNewData()
        {
            switch (DataManager.ConnectState)
            {
                case ConnectStates.AuthPassed:
                    Title = "Пользователь";
                    break;
                case ConnectStates.SocketConnected:
                    Title = "Нет авторизации";
                    break;
                default:
                    Title = "Нет связи";
                    break;
            }           

            if (needDataUpdate > 0)
            {
                needDataUpdate--;
                UpdateViewValues();
            }
        }

        private void UpdateViewValues()
        {
            serverButton.Text = DataManager.ServerName;

            if (DataManager.ConnectState == ConnectStates.AuthPassed)
                userNameText.Text = DataManager.UserName;
            else if (DataManager.ConnectState == ConnectStates.SocketConnected)
                userNameText.Text = "Нет авторизации";
            else
                userNameText.Text = "Нет связи";

            warningHourFrom.Text = DataManager.userSettings.notificationsFrom.Hour.ToString("D2");
            warningMinuteFrom.Text = DataManager.userSettings.notificationsFrom.Minute.ToString("D2");
            warningHourTo.Text = DataManager.userSettings.notificationsTo.Hour.ToString("D2");
            warningMinuteTo.Text = DataManager.userSettings.notificationsTo.Minute.ToString("D2");
            warningSwitch.Checked = DataManager.userSettings.sendNodesOfflineNotifications;
        }
    }
}