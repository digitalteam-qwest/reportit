using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace ReportIt.Converters
{
    public class AlternatingHighlightColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color rowcolor = Color.FromHex("#087887");

            if (value == null || parameter == null)
            {
                return rowcolor;
            }

            int index = ((Xamarin.Forms.CollectionView)parameter).ItemsSource.Cast<object>().ToList().IndexOf(value);
            if (index % 2 == 0)
            {
                rowcolor = Color.FromHex("#314a91");
            }

            return rowcolor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
