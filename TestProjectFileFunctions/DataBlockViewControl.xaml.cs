using System.Collections.Generic;
using System.Windows.Controls;

using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace TestProjectFileFunctions
{
    /// <summary>
    /// Interaction logic for DatabBlockViewControl.xaml
    /// </summary>
    public partial class DataBlockViewControl : UserControl
    {
        /*private S7DataRow _DataBlockRows;
        public S7DataRow DataBlockRows
        {
            get { return _DataBlockRows; }
            set
            {
                _DataBlockRows = value;
                MyTree.ItemsSource = new List<S7DataRow>() {value};
                
            }
        }*/

        private IDataBlock _DataBlock;
        public IDataBlock DataBlock
        {
            get { return _DataBlock; }
            set
            {
                _DataBlock = value;
                MyTree.ItemsSource = new List<S7DataRow>() { _DataBlock.Structure };

            }
        } 
        
        public DataBlockViewControl()
        {
            InitializeComponent();
        }

        private void chkMC7Code_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_DataBlock is S7DataBlock) 
                MyTree.ItemsSource = new List<S7DataRow>() { ((S7DataBlock)_DataBlock).StructureFromMC7 };
        }

        private void chkProjectString_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_DataBlock is S7DataBlock)
                MyTree.ItemsSource = new List<S7DataRow>() { ((S7DataBlock)_DataBlock).StructureFromString };
        }
    }
}
