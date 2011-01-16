using System;
using System.Globalization;
using System.Windows.Data;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace WPFToolboxForSiemensPLCs.DockableWindows.DataBlockViewControl
{
    public class TreeviewDepthWidthConverter : IValueConverter
    {
        public static int GetDepth(S7DataRow item)
        {
            int cnt = 0;
            while (item.Parent != null)
            {
                cnt++;
                item = item.Parent;
            }
            return cnt;
        }

            public double Width { get; set; }
            public double preWidth { get; set; }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var item = value as S7DataRow;
                if (item == null)
                    return 0; // new Thickness(0);

                if (preWidth==0.0)
                    return GetDepth(item) * Width;//Thickness(Length * item.GetDepth(), 0, 0, 0);
                else
                    return Width-(GetDepth(item) * preWidth);//Thickness(Length * item.GetDepth(), 0, 0, 0);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new System.NotImplementedException();
            }        
    }
}
