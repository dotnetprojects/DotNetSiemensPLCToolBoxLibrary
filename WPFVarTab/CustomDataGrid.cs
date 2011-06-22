using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace WPFVarTab
{
    public class CustomDataGrid:DataGrid
    {
        public CustomDataGrid()
        {
            
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is CustomDataGridRow);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomDataGridRow();
        }
       
        protected override void PrepareContainerForItemOverride(System.Windows.DependencyObject element, object item)
        {
            S7VATRow tmp = item as S7VATRow;
            
            base.PrepareContainerForItemOverride(element, item);

            
            //base.PrepareContainerForItemOverride(element, item);
            //DataGridRow dataGridRow = (DataGridRow)element;
            //if (dataGridRow.DataGridOwner != this)
            //{
            //    dataGridRow.Tracker.StartTracking(ref this._rowTrackingRoot);
            //    this.EnsureInternalScrollControls();
            //}
            //dataGridRow.PrepareRow(item, this);
            //this.OnLoadingRow(new DataGridRowEventArgs(dataGridRow));
        }
    }

    public class CustomDataGridCellsPanel : DataGridCellsPanel
    {
        public CustomDataGridCellsPanel()
            : base()
        {
        }
    }

    public class CustomDataGridCellsPresenter : DataGridCellsPresenter
    {
    }

    public class CustomDataGridRow:DataGridRow
    {
        
    }
}
