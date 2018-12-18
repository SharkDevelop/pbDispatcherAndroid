using Android.Support.V7.App;
using Android.Widget;

namespace Dispatcher.Android
{
    public abstract class BaseActivity : AppCompatActivity
    {
        protected void InitActionBar(int title)
        {
            FindViewById<ImageView>(Resource.Id.ivBack)
                .Click += (sender, args) => OnBackPressed();
            FindViewById<TextView>(Resource.Id.tvBack)
                .Click += (sender, args) => OnBackPressed();
            FindViewById<TextView>(Resource.Id.tvActionBarTitle).SetText(title);
        }

        protected void InitActionBar(string title)
        {
            FindViewById<ImageView>(Resource.Id.ivBack)
                .Click += (sender, args) => OnBackPressed();
            FindViewById<TextView>(Resource.Id.tvBack)
                .Click += (sender, args) => OnBackPressed();
            FindViewById<TextView>(Resource.Id.tvActionBarTitle).Text = title;
        }
    }
}