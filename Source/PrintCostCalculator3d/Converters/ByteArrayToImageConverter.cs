using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PrintCostCalculator3d.Converters
{
    public sealed class ByteArrayToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] imageAsBytes = value as byte[];
            if (imageAsBytes == null)
                return null;

            BitmapImage image = new BitmapImage();
            using (MemoryStream memStream = new MemoryStream(imageAsBytes))
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = memStream;
                image.EndInit();
                //image.SetSource(memStream);
            }
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
