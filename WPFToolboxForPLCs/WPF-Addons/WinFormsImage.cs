using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WPFToolboxForSiemensPLCs
{
    /// <summary>
    /// This class can directly display a WinForms Image
    /// </summary>
    class WinFormsImage : Image
    {
        /// <summary>
        /// Bind this to the WinForms Image
        /// </summary>
        public System.Drawing.Image ImageSource
        {
            get { return (System.Drawing.Image)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(System.Drawing.Image), typeof(WinFormsImage), new FrameworkPropertyMetadata(OnImageSourceChanged));
        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WinFormsImage)d).Source = FromDrawingImage((System.Drawing.Image)e.NewValue);
        }

        private static BitmapImage FromDrawingImage(System.Drawing.Image img)
        {
            using (var ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                var bImg = new BitmapImage();
                bImg.BeginInit();
                bImg.StreamSource = new MemoryStream(ms.ToArray());
                bImg.EndInit();
                return bImg;
            }
        }
    }
}
