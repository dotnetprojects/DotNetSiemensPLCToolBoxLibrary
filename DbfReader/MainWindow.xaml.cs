using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DotNetSiemensPLCToolBoxLibrary.DBF;
using DotNetSiemensPLCToolBoxLibrary.General;
using Binding = System.Windows.Data.Binding;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace DbfReader
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            if (dlg.ShowDialog().Value == true)
            {
                var dbf = ParseDBF.ReadDBF(dlg.FileName);
                table.ItemsSource = dbf.DefaultView;
            }
        }

        private void table_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof (byte[]))
            {
                var column = e.Column as DataGridTextColumn;
                var binding = column.Binding as Binding;
                binding.Converter = new ByteArrayConverter();
            }
        }

        private class ByteArrayConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value != null)
                {
                    var array = value as byte[];

                    var strg = Encoding.Default.GetString(array);
                    return "0x" + BitConverter.ToString(array).Replace("-", string.Empty) + Environment.NewLine + strg;
                }

                return null;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            txtFile.Text = "";
            string txt = "";
            InputBox.Show("Search", "Text to search...", ref txt);

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] files = Directory.GetFiles(folderBrowserDialog.SelectedPath, "*.dbf",
                    SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    var dbf = ParseDBF.ReadDBF(file);

                    if (dbf != null)
                    {
                        foreach (DataColumn column in dbf.Columns)
                        {
                            foreach (DataRow row in dbf.Rows)
                            {
                                var wrt = row[column];
                                if (wrt != null && wrt.ToString().Contains(txt))
                                {
                                    txtFile.Text = file;
                                    table.ItemsSource = dbf.DefaultView;
                                    goto finish;
                                }
                            }
                        }
                    }
                }

                txtFile.Text = "finish...";
            }

            finish:
            ;
        }
    }
}
