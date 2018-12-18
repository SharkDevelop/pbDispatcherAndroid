using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using DataUtils;

namespace Dispatcher.Android
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class SettingsActivity : BaseActivity
    {
        private TextView _tvUserNameValue;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_settings);
            InitActionBar(Resource.String.settings);

            FindViewById<TextView>(Resource.Id.tvCityValue)
                .Click += (sender, args) => StartFilterActivity(0);
            FindViewById<TextView>(Resource.Id.tvUnitValue)
                .Click += (sender, args) => StartFilterActivity(1);
            FindViewById<TextView>(Resource.Id.tvFacilityTypeValue)
                .Click += (sender, args) => StartFilterActivity(2);
            FindViewById<TextView>(Resource.Id.tvWorkSateValue)
                .Click += (sender, args) => StartFilterActivity(3);
            FindViewById<TextView>(Resource.Id.tvServiceStateValue)
                .Click += (sender, args) => StartFilterActivity(0);

            _tvUserNameValue = FindViewById<TextView>(Resource.Id.tvUserNameValue);
            _tvUserNameValue.Text = DataManager.UserName;
            _tvUserNameValue.Click += (sender, args) => StartUserActivity();
        }

        private void StartFilterActivity(int filterType)
        {
            var intent = new Intent(this, typeof(FilterListActivity));
            StartActivity(intent);
        }

        private void StartUserActivity()
        {
            var intent = new Intent();
            intent.SetClass(BaseContext, typeof(UserSettingsActivity));
            intent.SetFlags(ActivityFlags.ReorderToFront);
            StartActivity(intent);
        }
    }
}