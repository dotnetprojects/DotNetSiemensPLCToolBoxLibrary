using System.Collections.Generic;
using System.Windows.Controls;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace WPFToolboxForSiemensPLCs.DockableWindows.DataBlockViewControl
{
    /// <summary>
    /// Interaction logic for DatabBlockViewControl.xaml
    /// </summary>
    public partial class DataBlockViewControl : UserControl
    {
        private S7DataRow _DataBlockRows;
        public S7DataRow DataBlockRows
        {
            get { return _DataBlockRows; }
            set
            {
                _DataBlockRows = value;
                MyTree.ItemsSource = new List<S7DataRow>() {value};
                
            }
        }     
        
        public DataBlockViewControl()
        {
            InitializeComponent();
        }
    }
}
