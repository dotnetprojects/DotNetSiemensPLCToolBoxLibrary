using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using AvalonDock;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using WPFToolboxForSiemensPLCs.AvalonEdit;

namespace WPFToolboxForSiemensPLCs.DockableWindows
{
    public partial class ContentWindowDataBlockEditor : DocumentContent
    {

        public ContentWindowDataBlockEditor(object myBlock)
        {
            InitializeComponent();
            
            if (myBlock is PLCDataBlock)
            {
                PLCDataBlock blk = (PLCDataBlock) myBlock;
                dtaViewControl.DataBlockRows = blk.Structure;
            }
        }      
    }
}
