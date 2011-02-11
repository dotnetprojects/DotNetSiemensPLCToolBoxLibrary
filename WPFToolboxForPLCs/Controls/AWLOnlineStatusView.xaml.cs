using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;

namespace WPFToolboxForSiemensPLCs.Controls
{
    /// <summary>
    /// Interaction logic for AWLOnlineStatusView.xaml
    /// </summary>
    public partial class AWLOnlineStatusView : UserControl
    {
        public Network DisplayNetwork
        {
            get { return (Network)GetValue(DisplayNetworkProperty); }
            set { SetValue(DisplayNetworkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayNetwork.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayNetworkProperty =
            DependencyProperty.Register("DisplayNetwork", typeof(Network), typeof(AWLOnlineStatusView), new FrameworkPropertyMetadata(null));
        

        public ICSharpCode.AvalonEdit.TextEditor CorespondingTextEditor
        {
            get { return (ICSharpCode.AvalonEdit.TextEditor)GetValue(CorespondingTextEditorProperty); }
            set { SetValue(CorespondingTextEditorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CorespondingTextEditor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CorespondingTextEditorProperty = DependencyProperty.Register("CorespondingTextEditor", typeof (ICSharpCode.AvalonEdit.TextEditor), typeof (AWLOnlineStatusView), new UIPropertyMetadata(null, CorespondingTextEditorChanged));
        
        public static void CorespondingTextEditorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ICSharpCode.AvalonEdit.TextEditor textEditor = (ICSharpCode.AvalonEdit.TextEditor)e.NewValue;

            double top = textEditor.TextArea.TextView.GetVisualPosition(new TextViewPosition(1, 1), VisualYPosition.LineTop).Y;
            double bottom = textEditor.TextArea.TextView.GetVisualPosition(new TextViewPosition(1, 1), VisualYPosition.LineBottom).Y;
            double height = bottom - top;

        }

        public AWLOnlineStatusView()
        {
            InitializeComponent();

            this.DataContext = this;


            //Get heigth of a line
            //Implement a Row Status control
            //Display a row status control for each command
            //Diisplay a Parameter Status Control for each call parameter, also on the ucs

        }
    }
}
