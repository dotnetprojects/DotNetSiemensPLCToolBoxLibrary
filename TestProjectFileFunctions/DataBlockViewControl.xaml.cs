using System.Collections.Generic;
using System.Windows.Controls;

using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V11;
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

        public bool ExpandDataBlockArrays
        {
            get
            {
                return this.expandDataBlockArrays;
            }
            set
            {
                this.expandDataBlockArrays = value;

                if (_DataBlock != null)
                    if (DataBlock is TIADataBlock)
                    {
                        MyTree.ItemsSource = new List<IDataRow>() { ((IDataRow)_DataBlock.Structure) };
                    }
                    else
                    {
                        if (!value)
                        {
                            MyTree.ItemsSource = new List<S7DataRow>() { ((S7DataRow)_DataBlock.Structure) };

                        }
                        else
                        {
                            var expRow = ((S7DataBlock)_DataBlock).GetArrayExpandedStructure(new S7DataBlockExpandOptions() { ExpandCharArrays = false });
                            MyTree.ItemsSource = new List<S7DataRow>() { ((S7DataRow)expRow) };
                        }
                    }
            }
        }

        private IDataBlock _DataBlock;

        private bool expandDataBlockArrays;

        public IDataBlock DataBlock
        {
            get { return _DataBlock; }
            set
            {
                _DataBlock = value;
                if (_DataBlock != null)
                {
                    if (DataBlock is TIADataBlock)
                    {
                        MyTree.ItemsSource = new List<IDataRow>() { ((IDataRow)_DataBlock.Structure) };
                    }
                    else
                    {
                        MyTree.ItemsSource = new List<S7DataRow>() { ((S7DataRow)_DataBlock.Structure) };   
                    }                    
                }

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
