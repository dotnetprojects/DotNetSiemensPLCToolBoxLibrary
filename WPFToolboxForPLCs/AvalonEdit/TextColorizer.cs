using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace WPFToolboxForSiemensPLCs.AvalonEdit
{
    public class TextColorizer : DocumentColorizingTransformer
    {
         private int _start;

        private int _stop;

        private Brush _color;

        public TextColorizer(int start, int stop, Brush color)
        {
            this._start = start;
            this._stop = stop;
            this._color = color;
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            if (_start < line.Offset + line.Length && _stop > line.Offset)
            {

                base.ChangeLinePart(_start < line.Offset ? line.Offset : _start, _stop > line.Offset + line.Length ? line.Offset + line.Length : _stop, (VisualLineElement element) =>
                    {
                        // This lambda gets called once for every VisualLineElement
                        // between the specified offsets.
                        Typeface tf = element.TextRunProperties.Typeface;
                        // Replace the typeface with a modified version of
                        // the same typeface
                        element.TextRunProperties.SetBackgroundBrush(_color);
                    });
            }
        }
    }
}
