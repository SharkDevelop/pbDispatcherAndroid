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

            _tvUserNameValue = FindViewById<TextView>(Resource.Id.tvUserNameValue);
            _tvUserNameValue.Text = DataManager.UserName;
            _tvUserNameValue.Click += (sender, args) => StartUserActivity();
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