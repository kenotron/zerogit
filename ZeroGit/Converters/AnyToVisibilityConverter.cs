using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ZeroGit.Converters
{
    public class AnyToVisibilityConverter : IValueConverter
    {
        public bool Inverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is IList)
            {
                if (this.Inverse)
                {
                    return ((IList)value).Count > 0 ? Visibility.Visible : Visibility.Collapsed;
                }

                return ((IList)value).Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
