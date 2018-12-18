using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Dispatcher.Android
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class SensorAlertsActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_sensor_alerts);
            InitActionBar(Resource.String.sensor_alerts);
        }
    }
}