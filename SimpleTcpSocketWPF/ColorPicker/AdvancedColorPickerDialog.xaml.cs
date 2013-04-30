using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CustomWPFColorPicker
{
    public partial class AdvancedColorPickerDialog
    {
        #region Properties
        private byte[] _pixels;
        public Color CurrentColor = Colors.Gray;
        public event EventHandler DialogResultEvent;
        public event DragDeltaEventHandler Drag;

        private void OnDragDelta(DragDeltaEventArgs e)
        {
            var dragEventHandler = Drag;
            if (dragEventHandler != null) dragEventHandler(this, e);
        }

        public SolidColorBrush SelectedCurrentColor
        {
            get { return (SolidColorBrush)GetValue(SelectedCurrentColorProperty); }
            set { SetValue(SelectedCurrentColorProperty, value); }
        }

        public static DependencyProperty SelectedCurrentColorProperty = DependencyProperty.Register("SelectedCurrentColor", typeof(SolidColorBrush), typeof(ColorPickerControlView), new PropertyMetadata(Brushes.White));
        #endregion

        public AdvancedColorPickerDialog()
        {
            DataContext = this;
            InitializeComponent();
            brightnessSlider.ValueChanged += BrightnessSliderValueChanged;
            colorMarker.RenderTransform = _markerTransform;
            colorMarker.RenderTransformOrigin = new Point(1, 1);
            borderColorChart.Cursor = Cursors.Cross;
            brightnessSlider.Value = 0.5;
        }

        #region Dialog Members
        private void moveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            OnDragDelta(e);
        }

        
        public bool AllowsTransparency
        {
            set
            {
                if(value == false)
                {
                    Container.Margin = new Thickness(0);
                    Container.CornerRadius = new CornerRadius(0);
                }
            }
        }
        #endregion

        #region Advanced Picker Members
        
        private void OnDialogResultEvent(DialogEventArgs e)
        {
            var dialogResultEvent = DialogResultEvent;
            if (dialogResultEvent != null) 
                dialogResultEvent(this, e);
        }

        private readonly TranslateTransform _markerTransform = new TranslateTransform();
       
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var cb = new CroppedBitmap((BitmapSource)(((Image)e.Source).Source), new Int32Rect((int)Mouse.GetPosition(e.Source as Image).X, (int)Mouse.GetPosition(e.Source as Image).Y, 1, 1));
                _pixels = new byte[4];
                try
                {
                    cb.CopyPixels(_pixels, 4, 0);
                    UpdateCurrentColor();
                    UpdateMarkerPosition();
                }
                catch
                {
                }
                UpdateSlider();
            }
            catch (Exception)
            {
            }
        }

        private void UpdateCurrentColor()
        {
            CurrentColor = Color.FromRgb(_pixels[2], _pixels[1], _pixels[0]);
            currentColorBorder.Background = new SolidColorBrush(Color.FromRgb(_pixels[2], _pixels[1], _pixels[0]));
            brightnessSlider.Value = 0.5;
            SelectedCurrentColor = new SolidColorBrush(CurrentColor);
        }

        private void UpdateMarkerPosition()
        {
            _markerTransform.X = Mouse.GetPosition(borderColorChart).X - (borderColorChart.ActualWidth / 2);
            _markerTransform.Y = Mouse.GetPosition(borderColorChart).Y - (borderColorChart.ActualHeight / 2);
        }

        void BrightnessSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_pixels == null)
            {
                _pixels = new byte[3];
                _pixels[2] = CurrentColor.R;
                _pixels[1] = CurrentColor.G;
                _pixels[0] = CurrentColor.B;
            }

            var nc = Color.FromRgb(_pixels[2], _pixels[1], _pixels[0]);
            var f = (float)e.NewValue;
            float r, g, b;
            const float a = 1;

            if (f >= 0.5f)
            {
                r = nc.ScR + (1 - nc.ScR) * (f - 0.5f) * 2;
                g = nc.ScG + (1 - nc.ScG) * (f - 0.5f) * 2;
                b = nc.ScB + (1 - nc.ScB) * (f - 0.5f) * 2;
            }
            else
            {
                r = nc.ScR * f * 2;
                g = nc.ScG * f * 2;
                b = nc.ScB * f * 2;
            }

            CurrentColor = Color.FromScRgb(a, r, g, b);
            currentColorBorder.Background = new SolidColorBrush(CurrentColor);
        }

        private void UpdateSlider()
        {
            const float f = 1;
            const float a = 1;

            var nc = Color.FromRgb(_pixels[2], _pixels[1], _pixels[0]);
            var r = f * nc.ScR;
            var g = f * nc.ScG;
            var b = f * nc.ScB;

            // Update LGB for brightnessSlider
            var sb1 = brightnessSliderBorder;
            var lgb1 = sb1.Background as LinearGradientBrush;
            lgb1.StartPoint = new Point(0, 1);
            lgb1.EndPoint = new Point(0, 0);

            lgb1.GradientStops[0].Color = Color.FromScRgb(a, 0, 0, 0);
            lgb1.GradientStops[1].Color = Color.FromScRgb(a, r, g, b);
            lgb1.GradientStops[2].Color = Color.FromScRgb(a, 1, 1, 1);
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (e.Source.GetType().Equals(typeof(Image)))
                {
                    var cb = new CroppedBitmap((BitmapSource)(((Image)e.Source).Source), new Int32Rect((int)Mouse.GetPosition(e.Source as Image).X, (int)Mouse.GetPosition(e.Source as Image).Y, 1, 1));
                    _pixels = new byte[4];
                    try
                    {
                        cb.CopyPixels(_pixels, 4, 0);
                        UpdateMarkerPosition();
                        UpdateCurrentColor();
                        Mouse.Synchronize();
                    }
                    catch
                    {
                    }
                    UpdateSlider();
                }
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            OnDialogResultEvent(new DialogEventArgs(){SelectedColor = SelectedCurrentColor, DialogResult = DialogResult.Ok});
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            OnDialogResultEvent(new DialogEventArgs() { DialogResult = DialogResult.Cancel });
        }
        #endregion
    }
}