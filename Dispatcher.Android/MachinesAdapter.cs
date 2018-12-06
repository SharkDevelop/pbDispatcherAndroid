using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.App;
using Android.Content.Res;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using DataUtils;
using Console = System.Console;

namespace Dispatcher.Android
{
    public class MachinesAdapter : RecyclerView.Adapter
    {
        public List<Machine> Machines;

        public MachinesAdapter(List<Machine> machines)
        {
            Machines = machines;
            cachedImages = new Dictionary<string, Bitmap>();
        }

        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                Inflate(Resource.Layout.MachineCardView, parent, false);
            MachineViewHolder vh = new MachineViewHolder(itemView);
            return vh;
        }

        readonly AssetManager assets = Application.Context.Assets;

        private Dictionary<string, Bitmap> cachedImages;

        private Bitmap GetImageFromResources(string imagefileName)
        {
            if (string.IsNullOrEmpty(imagefileName)) return null;

            if (cachedImages.ContainsKey(imagefileName))
            {
                return cachedImages[imagefileName];
            }

            try
            {
                var stream = assets.List("").Contains(imagefileName) ? assets.Open(imagefileName) : assets.Open(imagefileName + ".png");
                var bitmap = BitmapFactory.DecodeStream(stream);

                if (!cachedImages.ContainsKey(imagefileName))
                {
                    cachedImages.Add(imagefileName, bitmap);
                }

                return bitmap;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // throw;
            }

            return null;
        }
        
        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MachineViewHolder cell = holder as MachineViewHolder;
            var item = Machines[position];

            cell.Image.SetImageBitmap(GetImageFromResources(item.type.iconName));
            cell.Title.Text = item.GetNameStr();
            cell.Description.Text = item.GetDivisionStr();

            cell.MachineStateIcon.SetImageBitmap(item.serviceState.code > MachineServiceStateCodes.Work
                ? GetImageFromResources(item.serviceState.iconName)
                : GetImageFromResources(item.state.iconName));

            if (item.sensors.Count != 0)
            {
                Sensor sensor = item.sensors[0];

                cell.SensorIconCell.SetImageBitmap(GetImageFromResources(sensor.type.iconName));
                cell.MainValue = sensor.mainValue.ToString("F2");
                cell.MainValueSymbol = sensor.type.mainValueSymbol;

                var numberFormatInfo = new System.Globalization.CultureInfo("en-Us", false).NumberFormat;
                numberFormatInfo.NumberGroupSeparator = " ";
                numberFormatInfo.NumberDecimalSeparator = ",";

                cell.AdditionalValue = sensor.additionalValue.ToString("N", numberFormatInfo);
                cell.AdditionalValueSymbol = sensor.type.additionalValueSymbol;
            }
            else
            {
               // cell.SensorIcon = "";
                cell.MainValue = "";
                cell.MainValueSymbol = "";
                cell.AdditionalValue = "";
                cell.AdditionalValueSymbol = "";
            }

            cell.SetColor(Color.Black);

            if ((item.state.code == MachineStateCodes.Failure) || (item.serviceState.code == MachineServiceStateCodes.Broken))
                cell.SetColor(Color.Red);
            else if ((item.divisionPosition.ID != item.divisionOwner.ID) && (item.divisionPosition.ID != 0))
                cell.SetColor(Color.Argb(255, 0, 0, 128));

            if (item.sensors.Count != 0)
                if ((DateTime.Now - item.sensors[0].lastTime).TotalMinutes > Settings.greyOfflineMinutes)
                    cell.SetColor(Color.Gray);            

            /*
             * 

                if (item.serviceState.code > MachineServiceStateCodes.Work)
                    cell.MachineStateIcon = item.serviceState.iconName;
                else
                    cell.MachineStateIcon = item.state.iconName;

                cell.Name = item.GetNameStr();

                cell.Division = item.GetDivisionStr();

               

                cell.SetColor(UIColor.Black);

                if ((item.state.code == MachineStateCodes.Failure) || (item.serviceState.code == MachineServiceStateCodes.Broken))
                    cell.SetColor(UIColor.Red);
                else if ((item.divisionPosition.ID != item.divisionOwner.ID) && (item.divisionPosition.ID != 0))
                    cell.SetColor(UIColor.FromRGBA(1, 0, 0, 0.6f));

                if (item.sensors.Count != 0)
                    if ((DateTime.Now - item.sensors[0].lastTime).TotalMinutes > Settings.greyOfflineMinutes)
                        cell.SetColor(UIColor.Gray);      
                

                cell.UserInteractionEnabled = true;
             */
        }

        public override int ItemCount
        {
            get { return Machines.Count; }
        }
    }
}