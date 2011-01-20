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
        
        public AWLOnlineStatusView()
        {
            InitializeComponent();

            this.DataContext = this;
        }
    }
}
