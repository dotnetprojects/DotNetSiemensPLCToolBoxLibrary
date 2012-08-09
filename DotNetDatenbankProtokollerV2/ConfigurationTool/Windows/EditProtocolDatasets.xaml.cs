using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSimaticDatabaseProtokollerLibrary;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Connections;
using DotNetSimaticDatabaseProtokollerLibrary.SettingsClasses.Datasets;

namespace DotNetSimaticDatabaseProtokollerConfigurationTool.Windows
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class EditProtocolDatasets : UserControl
    {
        public EditProtocolDatasets()
        {
            InitializeComponent();
        }

        //Helper property to get the DatGrid as Property (for use within the DataTemplate...)
        public DataGrid GetGrdDatsetsObject { get { return grdDatasets; } }

        private void cmdAddDataset_Click(object sender, RoutedEventArgs e)
        {
            string val = "Table_" + (ProtokollerConfiguration.ActualConfigInstance.Datasets.Count + 1);
            if (DotNetSiemensPLCToolBoxLibrary.General.InputBox.Show("Tablename", "Name of the Dataset (used as Tablename)", ref val) == System.Windows.Forms.DialogResult.OK)
            {
                DatasetConfig myConfig = new DatasetConfig();
                myConfig.Name = val;
                ProtokollerConfiguration.ActualConfigInstance.Datasets.Add(myConfig);
            }
        }

        private void cmdRemoveDataset_Click(object sender, RoutedEventArgs e)
        {
            if (grdDatasets.SelectedItem != null)
                ProtokollerConfiguration.ActualConfigInstance.Datasets.Remove((DatasetConfig) grdDatasets.SelectedItem);
        }

        private void cmdSelectReadBitFromProject_Click(object sender, RoutedEventArgs e)
        {
            if (grdDatasets.SelectedItem != null)
            {
                DatasetConfig conf = grdDatasets.SelectedItem as DatasetConfig;
                var tmp = DotNetSiemensPLCToolBoxLibrary.Projectfiles.SelectProjectPart.SelectTAG("");
                if (tmp != null)
                {
                    tmp.LibNoDaveDataType = DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Bool;
                    conf.TriggerReadBit = tmp;
                }
            }
        }

        private void cmdEditReadBit_Click(object sender, RoutedEventArgs e)
        {
            if (grdDatasets.SelectedItem != null)
            {
                DatasetConfig conf = grdDatasets.SelectedItem as DatasetConfig;
                var tmp = DotNetSiemensPLCToolBoxLibrary.Communication.PLCTagEditor.ShowPLCTagEditor(conf.TriggerReadBit);
                if (tmp != null)
                {
                    tmp.LibNoDaveDataType = DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Bool;
                    conf.TriggerReadBit = tmp;
                }
            }
        }

        private void cmdAddDatasetRow_Click(object sender, RoutedEventArgs e)
        {            
            DatasetConfig conf = grdDatasets.SelectedItem as DatasetConfig;

            ConnectionConfig akConn = null;
            string FieldType = "";
            if (conf.DatasetConfigRows.Count > 0)
            {
                akConn = conf.DatasetConfigRows[conf.DatasetConfigRows.Count - 1].Connection;
                FieldType = conf.DatasetConfigRows[conf.DatasetConfigRows.Count - 1].DatabaseFieldType;
            }

            conf.DatasetConfigRows.Add(new DatasetConfigRow() {DatabaseField = "Row_" + (conf.DatasetConfigRows.Count + 1).ToString(), Connection = akConn, DatabaseFieldType = FieldType});
        }

        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            if (cell != null && !cell.IsEditing && !cell.IsReadOnly)
            {
                if (!cell.IsFocused)
                {
                    cell.Focus();
                }
                DataGrid dataGrid = FindVisualParent<DataGrid>(cell);
                if (dataGrid != null)
                {
                    if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
                    {
                        if (!cell.IsSelected)
                            cell.IsSelected = true;
                    }
                    else
                    {
                        DataGridRow row = FindVisualParent<DataGridRow>(cell);
                        if (row != null && !row.IsSelected)
                        {
                            row.IsSelected = true;
                        }
                    }
                }
            }
        }

        static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }


        private void cmdEditBitInDataRowsList_Click(object sender, RoutedEventArgs e)
        {
            if (grdDatasetFields.SelectedItem != null)
            {
                DatasetConfigRow conf = grdDatasetFields.SelectedItem as DatasetConfigRow;
                var tmp = DotNetSiemensPLCToolBoxLibrary.Communication.PLCTagEditor.ShowPLCTagEditor(conf.PLCTag);
                if (tmp != null)
                {
                    conf.PLCTag = tmp;
                }
            }
        }

        private void cmdSelectBitFromProjectInDataRowsList_Click(object sender, RoutedEventArgs e)
        {
            if (grdDatasetFields.SelectedItem != null)
            {
                DatasetConfigRow conf = grdDatasetFields.SelectedItem as DatasetConfigRow;
                var tmp = DotNetSiemensPLCToolBoxLibrary.Projectfiles.SelectProjectPart.SelectTAG("");
                if (tmp != null)
                {
                    conf.PLCTag = tmp;
                }
            }
        }

        private void cmdEditQuittBit_Click(object sender, RoutedEventArgs e)
        {
            if (grdDatasets.SelectedItem != null)
            {
                DatasetConfig conf = grdDatasets.SelectedItem as DatasetConfig;
                var tmp = DotNetSiemensPLCToolBoxLibrary.Communication.PLCTagEditor.ShowPLCTagEditor(conf.TriggerQuittBit);
                if (tmp != null)
                {
                    tmp.LibNoDaveDataType = DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Bool;
                    conf.TriggerQuittBit = tmp;
                }
            }
        }

        private void cmdSelectQuitBitFromProject_Click(object sender, RoutedEventArgs e)
        {
            if (grdDatasets.SelectedItem != null)
            {
                DatasetConfig conf = grdDatasets.SelectedItem as DatasetConfig;
                var tmp = DotNetSiemensPLCToolBoxLibrary.Projectfiles.SelectProjectPart.SelectTAG("");
                if (tmp != null)
                {
                    tmp.LibNoDaveDataType = DotNetSiemensPLCToolBoxLibrary.DataTypes.TagDataType.Bool;
                    conf.TriggerQuittBit = tmp;
                }
            }
        }

        private void cmdRemoveDatasetRow_Click(object sender, RoutedEventArgs e)
        {
            DatasetConfig conf = grdDatasets.SelectedItem as DatasetConfig;
            if (grdDatasetFields.Visibility == System.Windows.Visibility.Visible)
            {
                if (grdDatasetFields.SelectedItem != null)
                    conf.DatasetConfigRows.Remove((DatasetConfigRow)grdDatasetFields.SelectedItem);
            }
            else
            {
                if (grdDatasetFieldsEthernet.SelectedItem != null)
                    conf.DatasetConfigRows.Remove((DatasetConfigRow) grdDatasetFieldsEthernet.SelectedItem);
            }

        }

        private void txt_Enter_ValueUpdater(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BindingExpression be = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
                be.UpdateSource();
                ((TextBox)sender).SelectAll();
                e.Handled = true;
                return;
            }
            e.Handled = false;
        }

        private void cmdCopyDataset_Click(object sender, RoutedEventArgs e)
        {
            if (grdDatasets.SelectedItem != null)
            {
                var newDS = ((DatasetConfig) grdDatasets.SelectedItem).Clone();
                newDS.Name += "_" + (ProtokollerConfiguration.ActualConfigInstance.Datasets.Count + 1);
                ProtokollerConfiguration.ActualConfigInstance.Datasets.Add(newDS);
                ProtokollerConfiguration.ReReferenceProtokollerConfiguration(ProtokollerConfiguration.ActualConfigInstance);
            }            
        }

        private void grdDatasets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (grdDatasets.SelectedItem != null)
                {
                    var DS = ((DatasetConfig) grdDatasets.SelectedItem);
                    int sz = 0;
                    foreach (var datasetConfigRow in DS.DatasetConfigRows)
                    {
                        sz += datasetConfigRow.PLCTag.ReadByteSize;
                    }
                    lblSize.Content = sz.ToString();
                }
            }
            catch (Exception)
            { }
        }

        private void cmdAddDatasetRowsFromProjectFile_Click(object sender, RoutedEventArgs e)
        {
            string DataBaseTyp;
            string DataBlockTyp;
            //Anpassungen von Henning Göpfert-Dürwald & Christoph Reinshaus

            var tags = DotNetSiemensPLCToolBoxLibrary.Projectfiles.SelectProjectPart.SelectTAGs("");
            if (tags != null)
            {
                foreach (PLCTag tag in tags)
                {
                    #region Create new row
                    DatasetConfig conf = grdDatasets.SelectedItem as DatasetConfig;

                    ConnectionConfig akConn = null;
                    string FieldType = "";
                    if (conf.DatasetConfigRows.Count > 0)
                    {
                        akConn = conf.DatasetConfigRows[conf.DatasetConfigRows.Count - 1].Connection;
                        FieldType = conf.DatasetConfigRows[conf.DatasetConfigRows.Count - 1].DatabaseFieldType;
                    }

                    conf.DatasetConfigRows.Add(new DatasetConfigRow() { DatabaseField = "Row_" + (conf.DatasetConfigRows.Count + 1).ToString(), Connection = akConn, DatabaseFieldType = FieldType });
                    #endregion

                    DatasetConfigRow confRow = grdDatasetFields.Items[grdDatasetFields.Items.Count - 1] as DatasetConfigRow;
                    confRow.PLCTag = tag;
                    
                    confRow.DatabaseField = tag.ValueName;

                    if (conf.Storage != null)
                    {
                        DataBaseTyp = conf.Storage.GetDefaultDatabaseFieldTypeForLibNoDaveTag(tag);

                        confRow.DatabaseFieldType = DataBaseTyp;
                    }
                }
            }
        }

        private void cmdMoveUp_Click(object sender, RoutedEventArgs e)
        {
            DataGrid grd = grdDatasetFields;
            if (grdDatasetFieldsEthernet.Visibility==Visibility.Visible)
                grd = grdDatasetFieldsEthernet;

            if (grd.SelectedItems.Count > 1)
            {
                if (grd.SelectedItems != null && grd.SelectedIndex >= 1)
                {
                    ProtokollerConfiguration.ActualConfigInstance.Datasets[grdDatasets.SelectedIndex].DatasetConfigRows.Move(grd.SelectedIndex - 1, grd.SelectedIndex + grd.SelectedItems.Count - 1);
                }
            }
            else
            {
                if (grd.SelectedItem != null && grd.SelectedIndex >= 1)
                {
                    ProtokollerConfiguration.ActualConfigInstance.Datasets[grdDatasets.SelectedIndex].DatasetConfigRows.Move(grd.SelectedIndex, grd.SelectedIndex - 1);                    
                }
            }
        }

        private void cmdMoveDown_Click(object sender, RoutedEventArgs e)
        {
            DataGrid grd = grdDatasetFields;
            if (grdDatasetFieldsEthernet.Visibility == Visibility.Visible)
                grd = grdDatasetFieldsEthernet;

            if (grd.SelectedItems.Count > 1)
            {
                if (grd.SelectedItems != null && (grd.SelectedIndex + grd.SelectedItems.Count) <= (grd.Items.Count - 1))
                {
                    ProtokollerConfiguration.ActualConfigInstance.Datasets[grdDatasets.SelectedIndex].DatasetConfigRows.Move(grd.SelectedIndex + grd.SelectedItems.Count, grd.SelectedIndex);                    
                }
            }
            else
            {
                if (grd.SelectedItem != null && grd.SelectedIndex < grd.Items.Count - 1)
                {
                    ProtokollerConfiguration.ActualConfigInstance.Datasets[grdDatasets.SelectedIndex].DatasetConfigRows.Move(grd.SelectedIndex, grd.SelectedIndex + 1);
                }
            }
        }

        private void cmdMoveToFirst_Click(object sender, RoutedEventArgs e)
        {
            DataGrid grd = grdDatasetFields;
            if (grdDatasetFieldsEthernet.Visibility == Visibility.Visible)
                grd = grdDatasetFieldsEthernet;

            if (grd.SelectedItems.Count > 1)
            {
                if (grd.SelectedItem != null && grd.SelectedIndex >= 1)
                {
                    int si = grd.SelectedIndex;
                    for (int i = 1; i <= grd.SelectedItems.Count; i++)
                    {
                        ProtokollerConfiguration.ActualConfigInstance.Datasets[grdDatasets.SelectedIndex].DatasetConfigRows.Move(si + i - 1, 0 + i - 1);
                    }
                }
            }
            else
            {
                if (grd.SelectedItem != null && grd.SelectedIndex >= 1)
                {
                    ProtokollerConfiguration.ActualConfigInstance.Datasets[grdDatasets.SelectedIndex].DatasetConfigRows.Move(grd.SelectedIndex, 0);
                }
            }
        }

        private void cmdMoveToLast_Click(object sender, RoutedEventArgs e)
        {
            DataGrid grd = grdDatasetFields;
            if (grdDatasetFieldsEthernet.Visibility == Visibility.Visible)
                grd = grdDatasetFieldsEthernet;

            if (grd.SelectedItems.Count > 1)
            {
                if ((grd.SelectedIndex + grd.SelectedItems.Count) < grd.Items.Count)
                {
                    int j = 1;
                    int si = grd.SelectedIndex;
                    int mi = grd.Items.Count - 1;
                    for (int i = grd.SelectedItems.Count; i >= 1; i--)
                    {
                        ProtokollerConfiguration.ActualConfigInstance.Datasets[grdDatasets.SelectedIndex].DatasetConfigRows.Move(si + i - 1, mi - j + 1);                        
                        j++;
                    }
                }
            }
            else
            {
                if (grd.SelectedItem != null && grd.SelectedIndex < grd.Items.Count - 1)
                {
                    ProtokollerConfiguration.ActualConfigInstance.Datasets[grdDatasets.SelectedIndex].DatasetConfigRows.Move(grd.SelectedIndex, grd.Items.Count - 1);
                }
            }
        }

        private void btn_UseSameConnection_Click(object sender, RoutedEventArgs e)
        {
            if (chkUseSameConnection.IsChecked.Value)
            {
                ConnectionConfig newConnection = cmb_UseSameConnection.SelectedValue as ConnectionConfig;
                if (newConnection != null)
                {
                    foreach (DatasetConfigRow item in ProtokollerConfiguration.ActualConfigInstance.Datasets[grdDatasets.SelectedIndex].DatasetConfigRows)
                    {
                        item.Connection = cmb_UseSameConnection.SelectedValue as ConnectionConfig;
                    }
                }
                chkUseSameConnection.IsChecked = false;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (chkConvertTypes.IsChecked.Value)
            {
                if (cmb_PLCType.SelectedValue.ToString() != "" && cmb_DBType.SelectedValue.ToString() != "")
                {
                    foreach (DatasetConfigRow item in ProtokollerConfiguration.ActualConfigInstance.Datasets[grdDatasets.SelectedIndex].DatasetConfigRows)
                    {
                        if (item.PLCTag.LibNoDaveDataType.ToString() == cmb_PLCType.SelectedValue.ToString())
                            item.DatabaseFieldType = cmb_DBType.SelectedValue.ToString();
                    }
                }
                chkConvertTypes.IsChecked = false;
            }
        }
    }
}
