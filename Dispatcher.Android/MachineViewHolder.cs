﻿using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;

namespace Dispatcher.Android
{
    public class MachineViewHolder : RecyclerView.ViewHolder
    {
        private Action<int> _listener;
        private View _itemView;

        public ImageView MachineIcon { get; private set; }

        public ImageView MachineStateIcon { get; private set; }

        public ImageView SensorIconCell { get; private set; }     

        public TextView Title { get; private set; }
        public TextView Description { get; private set; }

        public TextView MainValueCell { get; private set; }
        public TextView MainValueSymbolCell { get; private set; }

        public TextView AdditionalValueCell { get; private set; }
        public TextView AdditionalValueSymbolCell { get; private set; }       

        public MachineViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            MachineIcon = itemView.FindViewById<ImageView>(Resource.Id.imageView);
            SensorIconCell = itemView.FindViewById<ImageView>(Resource.Id.stateImageView);
            MachineStateIcon = itemView.FindViewById<ImageView>(Resource.Id.sensorImageView);

            Title = itemView.FindViewById<TextView>(Resource.Id.nameTextView);
            Description = itemView.FindViewById<TextView>(Resource.Id.descTextView);
            MainValueCell = itemView.FindViewById<TextView>(Resource.Id.mainValueTextView);
            MainValueSymbolCell = itemView.FindViewById<TextView>(Resource.Id.mainValueSymbolTextView);
            AdditionalValueCell = itemView.FindViewById<TextView>(Resource.Id.addValueTextView);
            AdditionalValueSymbolCell = itemView.FindViewById<TextView>(Resource.Id.addValueSymbolTextView);

            _listener = listener ?? throw new ArgumentNullException(nameof(listener));

            _itemView = itemView;
            _itemView.Click += ItemView_Click;
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

        public bool IsEmpty()
        {
            return MachineIcon == null;
        }

        public void Clear()
        {
            MachineIcon?.Dispose();
            MachineIcon = null;

            MachineStateIcon?.Dispose();
            MachineStateIcon = null;

            SensorIconCell?.Dispose();
            SensorIconCell = null;

            Title?.Dispose();
            Title = null;

            Description?.Dispose();
            Description = null;

            MainValueCell?.Dispose();
            MainValueCell = null;

            MainValueSymbolCell?.Dispose();
            MainValueSymbolCell = null;

            AdditionalValueCell?.Dispose();
            AdditionalValueCell = null;

            AdditionalValueSymbolCell?.Dispose();
            AdditionalValueSymbolCell = null;

            if (_itemView != null)
            {
                _itemView.Click -= ItemView_Click;
                _itemView.Dispose();
                _itemView = null;
            }

            _listener = null;
        }

        private void ItemView_Click(object sender, EventArgs e)
        {
            _listener(LayoutPosition);
        }
    }
}