using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;

namespace TiaExImPorter
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

        private void cmdExport_Click(object sender, RoutedEventArgs e)
        {
            Step7ProjectV11 prj = null;
            try
            {
                prj = new Step7ProjectV11(Properties.Settings.Default.TiaProject, CultureInfo.CurrentCulture);
                ExportStructureItem(prj.ProjectStructure, "");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (prj != null)
                    prj.Dispose();
            }
        }


        private void ExportStructureItem(ProjectFolder fld, string targetFolder)
        {
            if (fld is IBlocksFolder)
            {
                var pfd = System.IO.Path.Combine(Properties.Settings.Default.TargetDirectory, targetFolder.Substring(1));

                var bl = fld as IBlocksFolder;
                foreach (var i in bl.BlockInfos)
                {
                    if (i is ITiaProjectBlockInfo)
                    {
                        var ti = i as ITiaProjectBlockInfo;
                        try
                        {
                            var wrt = ti.ExportToString();
                            File.WriteAllText(System.IO.Path.Combine(pfd, ti.Name + ".exp"), ti.ExportToString());
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Error Exporting: " + ti.Name + "   Error:" + ex.Message);
                        }
                    }
                }                
            }


            foreach (var projectFolder in fld.SubItems)
            {
                var path = targetFolder + "\\" + projectFolder.Name;
                var pfd = System.IO.Path.Combine(Properties.Settings.Default.TargetDirectory, path.Substring(1));
                Directory.CreateDirectory(pfd);
                ExportStructureItem(projectFolder, path);
            }
        }


    }
}
