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
using DotNetSiemensPLCToolBoxLibrary.DataTypes;

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
            //Verbindung konfigurieren
            DotNetSiemensPLCToolBoxLibrary.Communication.Configuration.ShowConfiguration("ExampleWPFVisualization", true);
        }

        private BackgroundWorker worker = null;

        private void cmdConnect_Click(object sender, RoutedEventArgs e)
        {
            cmdConnect.IsEnabled = false;
            cmdStop.IsEnabled = true;

            //Alle Tags in eine Liste packen
            List<PLCTag> Tags = new List<PLCTag>();
            foreach (DictionaryEntry dictionaryEntry in this.Resources)
            {
                if (dictionaryEntry.Value is PLCTag)
                    Tags.Add((PLCTag) dictionaryEntry.Value);
            }

            try
            {
                //Verbindungskonfig laden und Verbinden
                myConn = new PLCConnection("ExampleWPFVisualization");
                myConn.Connect();
            }
            catch (Exception ex)
            {
                lblStatus.Content = ex.Message;
            }
            //Tags im Hintergrund laden
            if (worker != null)
            {
                worker.Dispose();
            }


            
            //Uncomment this line, to use the VarTab Functions for Read
            //PLCConnection.VarTabData vtab = myConn.ReadValuesWithVarTabFunctions(Tags, PLCReadTriggerVarTab.EndOfCycle);

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += delegate(object s, DoWorkEventArgs args)
                                 {
                                     while (!worker.CancellationPending)
                                     {
                                         try
                                         {
                                             myConn.ReadValues(Tags);
                                             //Uncomment this line, to use the Vartab Functions for read, and comment the line above!
                                             //vtab.RequestData();                                             
                                         }
                                         catch (Exception ex)
                                         {
                                         }
                                     }
                                 };

            worker.RunWorkerAsync();
        }

        private void PLCTag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Bei Änderung des Controlvalue eines Tags, den Tag in die SPS schreiben
            if (myConn != null)
                if (e.PropertyName == "Controlvalue")
                    myConn.WriteValue((PLCTag) sender);
        }

        private void cmdStop_Click(object sender, RoutedEventArgs e)
        {            
            worker.CancelAsync();            
            cmdConnect.IsEnabled = true;
            cmdStop.IsEnabled = false;
            myConn.Disconnect();
        }


      

      
    }
}
