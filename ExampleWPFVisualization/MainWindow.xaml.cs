using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
using DotNetSiemensPLCToolBoxLibrary.Communication;

namespace ExampleWPFVisualization
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DotNetSiemensPLCToolBoxLibrary.Communication.PLCConnection myConn = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void cmdConfig_Click(object sender, RoutedEventArgs e)
        {
            DotNetSiemensPLCToolBoxLibrary.Communication.Configuration.ShowConfiguration("ExampleWPFVisualization", true);
        }

        private void cmdConnect_Click(object sender, RoutedEventArgs e)
        {
            List<PLCTag> Tags=new List<PLCTag>();
            foreach (DictionaryEntry dictionaryEntry in this.Resources)
            {
                if (dictionaryEntry.Value is PLCTag)
                    Tags.Add((PLCTag) dictionaryEntry.Value);
            }
            
            myConn = new PLCConnection("ExampleWPFVisualization");
            myConn.Connect();

            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += delegate(object s, DoWorkEventArgs args)
                                 {
                                     while (true)
                                     {
                                         myConn.ReadValues(Tags);
                                     }
                                 };

            worker.RunWorkerAsync();
        }

        private void PLCTag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Controlvalue")
                myConn.WriteValue((PLCTag) sender);
        }

      
    }
}
