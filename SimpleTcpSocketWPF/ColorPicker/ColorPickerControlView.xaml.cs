using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace CustomWPFColorPicker
{
    public partial class ColorPickerControlView : UserControl, INotifyPropertyChanged
    {
        public SolidColorBrush CurrentBrush
        {
            get { return new SolidColorBrush(CurrentColor); }
            set { CurrentColor = value.Color; }
        }

        public Color CurrentColor
        {
            get { return (Color)GetValue(CurrentColorProperty); }
            set { SetValue(CurrentColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentColorProperty =
            DependencyProperty.Register("CurrentColor", typeof(Color), typeof(ColorPickerControlView), new PropertyMetadata(Colors.LightGray, OnCurrentColorChanged));
        
        private static void OnCurrentColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cp = d as ColorPickerControlView;
            if (e.NewValue != e.OldValue)
                cp.OnPropertyChanged("CurrentBrush");
        }
        
        public static RoutedUICommand SelectColorCommand = new RoutedUICommand("SelectColorCommand","SelectColorCommand", typeof(ColorPickerControlView));
        private Window _advancedPickerWindow;

        public ColorPickerControlView()
        {
            DataContext = this;
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(SelectColorCommand, SelectColorCommandExecute));
        }

        private void SelectColorCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            CurrentBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(e.Parameter.ToString()));
        }

        private static void ShowModal(Window advancedColorWindow)
        {
            advancedColorWindow.Owner = Application.Current.MainWindow;
            advancedColorWindow.ShowDialog();
        }

        void AdvancedPickerPopUpKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                _advancedPickerWindow.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = false;
            e.Handled = false;
        }

        private void MoreColorsClicked(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = false;
            var advancedColorPickerDialog = new AdvancedColorPickerDialog();
            _advancedPickerWindow = new Window
                                        {
                                            AllowsTransparency = true,
                                            Content = advancedColorPickerDialog,
                                            WindowStyle = WindowStyle.None,
                                            ShowInTaskbar = false,
                                            Background = new SolidColorBrush(Colors.Transparent),
                                            Padding = new Thickness(0),
                                            Margin = new Thickness(0),
                                            WindowState = WindowState.Normal,
                                            WindowStartupLocation = WindowStartupLocation.CenterOwner,
                                            SizeToContent = SizeToContent.WidthAndHeight
                                        };
            _advancedPickerWindow.DragMove();
            _advancedPickerWindow.KeyDown += AdvancedPickerPopUpKeyDown;
            advancedColorPickerDialog.DialogResultEvent += AdvancedColorPickerDialogDialogResultEvent;
            advancedColorPickerDialog.Drag += AdvancedColorPickerDialogDrag;
            ShowModal(_advancedPickerWindow);
        }

        void AdvancedColorPickerDialogDrag(object sender, DragDeltaEventArgs e)
        {
            _advancedPickerWindow.DragMove();
        }

        void AdvancedColorPickerDialogDialogResultEvent(object sender, EventArgs e)
        {
            _advancedPickerWindow.Close();
            var dialogEventArgs = (DialogEventArgs)e;
            if (dialogEventArgs.DialogResult == DialogResult.Cancel)
                return;
            CurrentBrush = dialogEventArgs.SelectedColor;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}