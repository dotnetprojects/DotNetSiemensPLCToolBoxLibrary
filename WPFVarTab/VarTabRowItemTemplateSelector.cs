using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace WPFVarTab
{
    public class VarTabRowItemTemplateSelector : DataTemplateSelector 
    {
        public DataTemplate NormalRowsTemplate { get; set; }
        public DataTemplate CommentRowsTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            S7VATRow vRow = item as S7VATRow;
            if (vRow == null || string.IsNullOrEmpty(vRow.Comment))
                return NormalRowsTemplate;
            return CommentRowsTemplate;
        }
    }
}
