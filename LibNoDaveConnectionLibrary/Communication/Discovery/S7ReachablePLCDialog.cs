using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Discovery
{
    public partial class S7ReachablePLCDialog : Form
    {
        S7ReachablePLCScanner Discoverer = new S7ReachablePLCScanner();

        public S7ReachablePLCScanner.FoundPlc SelectedPlc
        {
            get
            {
                if (listViewFoundPlcs.SelectedItems.Count <= 0) return null;
                return (S7ReachablePLCScanner.FoundPlc)listViewFoundPlcs.SelectedItems[0].Tag;
            }
         }

        public S7ReachablePLCDialog()
        {
            InitializeComponent();
            Discoverer.NewPlcFound += Discoverer_NewPlcFound;
            Discoverer.ScanComplete += Discoverer_ScanComplete;
            Discoverer.ProgressChanged += Discoverer_ProgressChanged;
        }

        private void StartDiscovering()
        {
            if (Discoverer.isRunning)
                return;

            this.listViewFoundPlcs.Items.Clear();
            _LVGroups.Clear();
            toolStripButtonRefresh.Enabled = false;
            toolStripButtonAbort.Enabled = true;
            toolStripProgressBar.Visible = true;
            Discoverer.BeginScan(DotNetSiemensPLCToolBoxLibrary.Communication.LibNodaveConnectionTypes.ISO_over_TCP);
        }

        private void DiscoveringEnded()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(DiscoveringEnded));
            }
            else
            {
                toolStripButtonRefresh.Enabled = true;
                toolStripButtonAbort.Enabled = false;
                toolStripProgressBar.Visible = false;

                if (closeing)
                {
                    DialogResult = diaRes;
                    Close();
                }
            }
        }

        #region Events
        private void ToolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            StartDiscovering();
        }

        private void ToolStripButtonAbort_Click(object sender, EventArgs e)
        {
            Discoverer.AbortScan();
        }

        private DialogResult diaRes;
        private void ToolStripButtonOK_Click(object sender, EventArgs e)
        {
            diaRes = DialogResult.OK;
            DialogResult = diaRes;//Dialog result get reset when closing is canceld
            Close();
        }

        private void ToolStripButtonCancel_Click(object sender, EventArgs e)
        {
            diaRes = DialogResult.Cancel;
            DialogResult = diaRes;
            Close();
        }

        private void listViewFoundPlcs_SelectedIndexChanged(object sender, EventArgs e)
        {
            toolStripButtonOK.Enabled = this.listViewFoundPlcs.SelectedItems.Count >= 1;
        }

        private bool closeing;
        private void ReachablePlcDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Discoverer.isRunning)
            {
                closeing = true;
                e.Cancel = true;
                Discoverer.AbortScan();
            }
        }

        private void ReachablePlcDialog_Shown(object sender, EventArgs e)
        {
            StartDiscovering();
        }
        #endregion

        #region Discoverer Events
        private delegate void ProgressChangedDelegate(object sender, IPScanner.ProgressChangedEventArgs e);
        private void Discoverer_ProgressChanged(object sender, IPScanner.ProgressChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new ProgressChangedDelegate(Discoverer_ProgressChanged), sender, e);
            }
            else
            {
                toolStripProgressBar.Maximum = Math.Max(e.ProgressMax, e.Progress);
                toolStripProgressBar.Value = e.Progress;
            }
        }

        private void Discoverer_NewPlcFound(object sender, S7ReachablePLCScanner.PlcFoundEventArgs e)
        {
            Invoke(new NewPlcFoundDelegate(NewPlcFound), e.FoundPlc);
        }

        private void Discoverer_ScanComplete(object sender, EventArgs e)
        {
            DiscoveringEnded();
        }

        private delegate void NewPlcFoundDelegate(S7ReachablePLCScanner.FoundPlc plc);

        private Dictionary<string, ListViewGroup> _LVGroups = new Dictionary<string, ListViewGroup>();
        public void NewPlcFound(S7ReachablePLCScanner.FoundPlc plc)
        {
            var Item = this.listViewFoundPlcs.Items.Add(plc.S7ConnectionSettings.CpuIP);
            Item.SubItems.Add(plc.PlcName);
            Item.SubItems.Add(string.IsNullOrEmpty(plc.ModuleName) ? plc.ModuleTypeName : plc.ModuleName);
            Item.SubItems.Add(string.IsNullOrEmpty(plc.Manufacturer) ? plc.Copyright : plc.Manufacturer);
            Item.SubItems.Add(plc.S7ConnectionSettings.CpuSlot.ToString());

            Item.Tag = plc;

            //Group Item 
            if (!_LVGroups.ContainsKey(plc.S7ConnectionSettings.CpuIP))
            {
                _LVGroups.Add(plc.S7ConnectionSettings.CpuIP, listViewFoundPlcs.Groups.Add(plc.S7ConnectionSettings.CpuIP, plc.S7ConnectionSettings.CpuIP));
            }
            Item.Group = _LVGroups[plc.S7ConnectionSettings.CpuIP.ToString()];

            //Select first element, when first element was added
            if (this.listViewFoundPlcs.Items.Count == 1)
            {
                this.listViewFoundPlcs.SelectedIndices.Add(0);
            }
        }
        #endregion
    }
}
