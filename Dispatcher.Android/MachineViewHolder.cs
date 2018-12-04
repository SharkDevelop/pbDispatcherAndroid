using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Dispatcher.Android
{
    public class MachineViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; private set; }
        public TextView Title { get; private set; }
        public TextView Description { get; private set; }

        public ImageView SensorIconCell { get; set; }

        //public string SensorIcon
        //{
        //    get { return SensorIconCell.Image.ToString(); }
        //    set { SensorIconCell.Image = FromFile(value); }
        //}

        public TextView MainValueCell { get; set; }
        public TextView MainValueSymbolCell { get; set; }

        public TextView AdditionalValueCell { get; set; }
        public TextView AdditionalValueSymbolCell { get; set; }

        public string MainValue
        {
            get { return MainValueCell.Text; }
            set { MainValueCell.Text = value; }
        }

        public string MainValueSymbol
        {
            get { return MainValueSymbolCell.Text; }
            set { MainValueSymbolCell.Text = value; }
        }

        public string AdditionalValue
        {
            get { return AdditionalValueCell.Text; }
            set { AdditionalValueCell.Text = value; }
        }

        public string AdditionalValueSymbol
        {
            get { return AdditionalValueSymbolCell.Text; }
            set { AdditionalValueSymbolCell.Text = value; }
        }

        //public string MachineIcon
        //{
        //    get { return MachineIconCell.ToString(); }
        //    set { MachineIconCell.Image = UIImage.FromFile(value); }
        //}

        public MachineViewHolder(View itemView) : base(itemView)
        {
            // Locate and cache view references:
            Image = itemView.FindViewById<ImageView>(Resource.Id.imageView);
            Title = itemView.FindViewById<TextView>(Resource.Id.nameTextView);
            Description = itemView.FindViewById<TextView>(Resource.Id.descTextView);
            MainValueCell = itemView.FindViewById<TextView>(Resource.Id.mainValueTextView);
            MainValueSymbolCell = itemView.FindViewById<TextView>(Resource.Id.mainValueSymbolTextView);
            AdditionalValueCell = itemView.FindViewById<TextView>(Resource.Id.addValueTextView);
            AdditionalValueSymbolCell = itemView.FindViewById<TextView>(Resource.Id.addValueSymbolTextView);
        }

        public void SetColor(Color color)
        {
            Title.SetTextColor(color);
            Description.SetTextColor(color);
            MainValueCell.SetTextColor(color);
            MainValueSymbolCell.SetTextColor(color);
            AdditionalValueCell.SetTextColor(color);
            AdditionalValueSymbolCell.SetTextColor(color);         
        }
    }
}