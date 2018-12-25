using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using DataUtils;

namespace Dispatcher.Android
{
    public class HistoryGraphView : View
    {
        const float leftBorder = 30;
        const float rightBorder = 10;
        const float downBorder = 40;

        private List<HistoryPoint> historyList;
        public DateTime timeStart, timeEnd;
        public bool readyToDataUpdate = false;

        public HistoryGraphView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public HistoryGraphView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        private void Initialize()
        {
        }

        public void SetData(List<HistoryPoint> historyList, DateTime timeStart, DateTime timeEnd)
        {
            this.historyList = historyList;
            this.timeStart = timeStart;
            this.timeEnd = timeEnd;
        }

        double initialMinValue = double.MaxValue;
        double initialMaxValue = double.MinValue;

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            var paint = new Paint
            {
                Color = Color.Black,
                StrokeWidth = 1                
            };

            paint.SetStyle(Paint.Style.Stroke);

            var path = new Path();

            float width = Width - leftBorder - rightBorder;
            float height = Height - downBorder;

            path.Reset();
            path.MoveTo(leftBorder - 3, 3);
            path.LineTo(leftBorder, 0);
            path.LineTo(leftBorder + 3, 3);            
            path.MoveTo(leftBorder, 0);
            path.LineTo(leftBorder, height + 2);
            path.LineTo(leftBorder + width + rightBorder / 2, height + 2);
            path.LineTo(leftBorder + width + rightBorder / 2 - 3, height + 2 - 3);
            path.LineTo(leftBorder + width + rightBorder / 2, height + 2);
            path.LineTo(leftBorder + width + rightBorder / 2 - 3, height + 2 + 3);

            canvas.DrawPath(path, paint);
            
            canvas.DrawText(timeStart.ToString("HH:mm"), leftBorder, height + 15, new Paint() { Color = Color.Black, TextSize = 12 });
            canvas.DrawText(timeStart.ToString("dd.MM.yy"), leftBorder, height + 27, new Paint() { Color = Color.Black, TextSize = 7 });
            canvas.DrawText(timeEnd.ToString("HH:mm"), leftBorder + width - 25, height + 15, new Paint() { Color = Color.Black, TextSize = 12 });
            canvas.DrawText(timeEnd.ToString("dd.MM.yy"), leftBorder + width - 25, height + 27, new Paint() { Color = Color.Black, TextSize = 7 });

            if (historyList == null || !historyList.Any()) return;

            var graphPaint = new Paint
            {
                Color = Color.Gray,
                StrokeWidth = 2
            };

            graphPaint.SetStyle(Paint.Style.Stroke);

            float period = (timeEnd - timeStart).Ticks;

            double minValue = initialMinValue;
            double maxValue = initialMaxValue;
            for (int s = 0; s < historyList.Count; s++)
            {
                if (historyList[s].value < minValue)
                    minValue = historyList[s].value;
                if (historyList[s].value > maxValue)
                    maxValue = historyList[s].value;
            }

            initialMinValue = minValue;
            initialMaxValue = maxValue;

            double valueRange = maxValue - minValue;

            path.Reset();

            int pointX = 0;

            for (int s = 0; s < historyList.Count; s++)
            {
                if ((historyList[s].time < timeStart) || (historyList[s].time > timeEnd))
                    continue;

                float X = leftBorder + (historyList[s].time - timeStart).Ticks / period * width;
                float Y = (float)(height + 2 - ((historyList[s].value - minValue) / valueRange * height));

                if (pointX == (int)width) break;

                if (s == 0)
                    path.MoveTo(X, Y);
                else
                    path.LineTo(X, Y);

                pointX++;                
            }

            canvas.DrawPath(path, graphPaint);

            // X axis
            DateTime quartDate = timeStart + new TimeSpan(((timeEnd - timeStart).Ticks / 4));
            DateTime mediumDate = timeStart + new TimeSpan(((timeEnd - timeStart).Ticks / 2));
            DateTime threeQuartDate = timeStart + new TimeSpan(((timeEnd - timeStart).Ticks * 3 / 4));

            canvas.DrawText(quartDate.ToString("HH:mm"), leftBorder + width / 4 - 15, height + 15, new Paint() { Color = Color.Black, TextSize = 12 });
            canvas.DrawText(quartDate.ToString("dd.MM.yy"), leftBorder + width / 4 - 15, height + 27, new Paint() { Color = Color.Black, TextSize = 7 });
            canvas.DrawText(mediumDate.ToString("HH:mm"), leftBorder + width / 2 - 15, height + 15, new Paint() { Color = Color.Black, TextSize = 12 });
            canvas.DrawText(mediumDate.ToString("dd.MM.yy"), leftBorder + width / 2 - 15, height + 27, new Paint() { Color = Color.Black, TextSize = 7 });
            canvas.DrawText(threeQuartDate.ToString("HH:mm"), leftBorder + width * 3 / 4 - 15, height + 15, new Paint() { Color = Color.Black, TextSize = 12 });
            canvas.DrawText(threeQuartDate.ToString("dd.MM.yy"), leftBorder + width * 3 / 4 - 15, height + 27, new Paint() { Color = Color.Black, TextSize = 7 });

            path.Reset();
            path.MoveTo(leftBorder + width / 4, height + 2 - 3);
            path.LineTo(leftBorder + width / 4, height + 2 + 3);

            path.MoveTo(leftBorder + width / 2, height + 2 - 3);
            path.LineTo(leftBorder + width / 2, height + 2 + 3);

            path.MoveTo(leftBorder + width * 3 / 4, height + 2 - 3);
            path.LineTo(leftBorder + width * 3 / 4, height + 2 + 3);

            canvas.DrawPath(path, paint);

            // Y axis

            double quartValue = minValue + ((maxValue - minValue) / 4);
            double mediumValue = minValue + ((maxValue - minValue) / 2);
            double threeQuartValue = minValue + ((maxValue - minValue) * 3 / 4);

            canvas.DrawText(minValue.ToString("F1"), 2, height, new Paint() { Color = Color.Black, TextSize = 11 });
            canvas.DrawText(quartValue.ToString("F1"), 2, height + 6 - height / 4, new Paint() { Color = Color.Black, TextSize = 11 });
            canvas.DrawText(mediumValue.ToString("F1"), 2, height + 6 - height / 2, new Paint() { Color = Color.Black, TextSize = 11 });
            canvas.DrawText(threeQuartValue.ToString("F1"), 2, height + 6 - height * 3 / 4, new Paint() { Color = Color.Black, TextSize = 11 });
            canvas.DrawText(maxValue.ToString("F1"), 2, 12, new Paint() { Color = Color.Black, TextSize = 11 });

            path.Reset();
            path.MoveTo(leftBorder - 3, height + 2 - height / 4);
            path.LineTo(leftBorder + 3, height + 2 - height / 4);

            path.MoveTo(leftBorder - 3, height + 2 - height / 2);
            path.LineTo(leftBorder + 3, height + 2 - height / 2);

            path.MoveTo(leftBorder - 3, height + 2 - height * 3 / 4);
            path.LineTo(leftBorder + 3, height + 2 - height * 3 / 4);

            canvas.DrawPath(path, paint);
        }
    }
}