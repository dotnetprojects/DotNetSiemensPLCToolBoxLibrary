using System.Collections.Generic;
using System.Windows.Controls;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace TestProjectFileFunctions
{
    /// <summary>
    /// Interaction logic for DatabBlockViewControl.xaml
    /// </summary>
    public partial class DataBlockViewControl : UserControl
    {
        private PLCDataRow _DataBlockRows;
        public PLCDataRow DataBlockRows
        {
            get { return _DataBlockRows; }
            set
            {
                _DataBlockRows = value;
                MyTree.ItemsSource = new List<PLCDataRow>() {value};
                
            }
        }     
        
        public DataBlockViewControl()
        {
            InitializeComponent();
        }
    }
}
