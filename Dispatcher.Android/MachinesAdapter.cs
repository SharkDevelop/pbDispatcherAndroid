using System;
using System.Collections.Generic;
using System.Globalization;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using DataUtils;
using Dispatcher.Android.Helpers;

namespace Dispatcher.Android
{
    public class MachinesAdapter : RecyclerView.Adapter
    {
        private readonly List<Machine> _machines;
        public event Action<int> ItemClicked;

        public MachinesAdapter(List<Machine> machines)
        {
            _machines = machines;            
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(parent.Context)
                .Inflate(Resource.Layout.item_machine, parent, false);
            
            var vh = new MachineViewHolder(itemView, OnClick);
            return vh;
        }       
        
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                if (!(holder is MachineViewHolder cell) || 
                    _machines.Count <= position) return;
            
                var item = _machines[position];

                cell.MachineIcon.SetImageBitmap(ResourcesHelper.GetImageFromResources(item.type.iconName));

                cell.MachineStateIcon.SetImageBitmap(item.serviceState.code > MachineServiceStateCodes.Work
                    ? ResourcesHelper.GetImageFromResources(item.serviceState.iconName)
                    : ResourcesHelper.GetImageFromResources(item.state.iconName));

                cell.Title.Text = item.GetNameStr();
                cell.Description.Text = item.GetDivisionStr();           

                if (item.sensors.Count != 0)
                {
                    var sensor = item.sensors[0];

                    cell.SensorIconCell.SetImageBitmap(ResourcesHelper.GetImageFromResources(sensor.type.iconName));
                    cell.MainValue = sensor.mainValue.ToString("F2");
                    cell.MainValueSymbol = sensor.type.mainValueSymbol;

                    var numberFormatInfo = new CultureInfo("en-Us", false).NumberFormat;
                    numberFormatInfo.NumberGroupSeparator = " ";
                    numberFormatInfo.NumberDecimalSeparator = ",";

                    cell.AdditionalValue = sensor.additionalValue.ToString("N", numberFormatInfo);
                    cell.AdditionalValueSymbol = sensor.type.additionalValueSymbol;
                }
                else
                {
                    cell.SensorIconCell.SetImageBitmap(null);
                    cell.MainValue = "";
                    cell.MainValueSymbol = "";
                    cell.AdditionalValue = "";
                    cell.AdditionalValueSymbol = "";
                }

                cell.SetColor(Color.Black);

                if (item.state.code == MachineStateCodes.Failure || item.serviceState.code == MachineServiceStateCodes.Broken)
                    cell.SetColor(Color.Red);
                else if (item.divisionPosition.ID != item.divisionOwner.ID && (item.divisionPosition.ID != 0))
                    cell.SetColor(Color.Argb(128, 255, 0, 0));

                if (item.sensors.Count != 0)
                {
                    if ((DateTime.Now - item.sensors[0].lastTime).TotalMinutes > Settings.greyOfflineMinutes)
                        cell.SetColor(Color.Gray);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
               
        public override int ItemCount => _machines.Count;

        private void OnClick(int position)
        {
            ItemClicked?.Invoke(position);
        }
    }
}