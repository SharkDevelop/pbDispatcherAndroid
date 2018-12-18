using System.Collections.Generic;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;

namespace Dispatcher.Android
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class FilterListActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.activity_filters_list);
            InitActionBar();
            
            var listView = FindViewById<ListView>(Resource.Id.lvFilters);
            listView.ItemClick += OnListItemClick;
            listView.Adapter = new FilterListAdapter(this, new List<Filter>
            {
                new Filter {ImageName = "Mixer", Description = "FirstItem"},
                new Filter {ImageName = "Mixer", Description = "FirstItem"},
                new Filter {ImageName = "Mixer", Description = "FirstItem"},
                new Filter {ImageName = "Mixer", Description = "FirstItem"},
                new Filter {ImageName = "Mixer", Description = "FirstItem"},
                new Filter {ImageName = "Mixer", Description = "FirstItem"},
                new Filter {ImageName = "Mixer", Description = "SecondItem"}
            });
        }

        private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            
        }
    }
}