using System;

namespace DotNetSiemensPLCToolBoxLibrary
{
#if !IPHONE
    /// <summary>
    /// Stellt einen UI Type Editor der für einen String aus einer Liste von Werten auswählen lässt!
    /// </summary>
    public class LibNoDaveValueUITypeEditor : System.Drawing.Design.UITypeEditor
    {
        System.Windows.Forms.Design.IWindowsFormsEditorService m_objService;

        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return System.Drawing.Design.UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {

            if (context != null && context.Instance != null && provider != null)
            {
                m_objService = (System.Windows.Forms.Design.IWindowsFormsEditorService)provider.GetService(typeof(System.Windows.Forms.Design.IWindowsFormsEditorService));

                if (m_objService != null)
                {
                    LibNoDaveValueEditor akVal = new LibNoDaveValueEditor((LibNoDaveValue)value);
                    akVal.ShowDialog();
                }
            }
            return value;
        }       
    }
#endif
}
