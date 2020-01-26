using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CDCswitchserver.Converter
{
    public class SliderScoreSizeCoverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double Del;
            if (double.TryParse(value.ToString(), out Del))
            {
                var IfaceButton = setupwindow.FindControl(setupwindow.Selected.Name, setupwindow.listorigin);
                if (IfaceButton == null) return DependencyProperty.UnsetValue;
                return ((double)Del / (double)IfaceButton.Height) * 100;
            }
            return DependencyProperty.UnsetValue;
        }

  
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double Size_score;
            if (double.TryParse(value.ToString(), out Size_score))
            {
                var IfaceButton = setupwindow.FindControl(setupwindow.Selected.Name, setupwindow.listorigin);
                if (IfaceButton == null) return DependencyProperty.UnsetValue;
                return (Size_score / (double)100) * IfaceButton.Height;
            }
            return DependencyProperty.UnsetValue;
        }

    }

    public class Aroundslider : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double? v = value as double?;
            return (int)v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int v;
            if (int.TryParse(value.ToString(), out v))
            {
                return v;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
