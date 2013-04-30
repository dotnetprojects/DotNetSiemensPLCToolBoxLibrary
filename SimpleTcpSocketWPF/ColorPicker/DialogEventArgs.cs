using System;
using System.Windows.Media;

namespace CustomWPFColorPicker
{
    public class DialogEventArgs : EventArgs
    {
        public DialogResult DialogResult { get; set; }
        public SolidColorBrush SelectedColor { get; set; }
    }
}