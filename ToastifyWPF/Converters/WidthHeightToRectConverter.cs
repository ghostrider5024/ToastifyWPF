﻿using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace ToastifyWPF.Converters
{
    public class WidthHeightToRectConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 &&
                values[0] is double width &&
                values[1] is double height)
            {
                return new Rect(0, 0, width, height);
            }
            return Rect.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }


}
