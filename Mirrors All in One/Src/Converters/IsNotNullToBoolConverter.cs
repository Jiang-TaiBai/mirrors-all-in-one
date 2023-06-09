using System;
using System.Globalization;
using System.Windows.Data;

namespace Mirrors_All_in_One.Converters
{
    /// <summary>
    /// 判断是否为null，返回bool值
    /// </summary>
    public class IsNotNullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}