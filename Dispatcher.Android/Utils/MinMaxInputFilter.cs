﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;

namespace Dispatcher.Android.Utils
{
    public class MinMaxInputFilter : Java.Lang.Object, IInputFilter
    {
        private int _min = 0;
        private int _max = 0;

        public MinMaxInputFilter(int min, int max)
        {
            _min = min;
            _max = max;
        }

        public Java.Lang.ICharSequence FilterFormatted(Java.Lang.ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
        {
            try
            {
                string val = dest.ToString().Insert(dstart, source.ToString());
                int input = int.Parse(val);
                if (IsInRange(_min, _max, input))
                    return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("FilterFormatted Error: " + ex.Message);
            }

            return new Java.Lang.String(string.Empty);
        }

        private bool IsInRange(int min, int max, int input)
        {
            return max > min ? input >= min && input <= max : input >= max && input <= min;
        }
    }
}