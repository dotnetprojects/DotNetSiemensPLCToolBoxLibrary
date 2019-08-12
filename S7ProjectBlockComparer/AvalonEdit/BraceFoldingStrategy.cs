// Copyright (c) 2009 Daniel Grunwald
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace WPFToolboxForSiemensPLCs.AvalonEdit
{
	/// <summary>
	/// Allows producing foldings from a document based on braces.
	/// </summary>
	public class BraceFoldingStrategy
	{
	
        
		/// <summary>
		/// Create <see cref="NewFolding"/>s for the specified document.
		/// </summary>
		public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
		{
			firstErrorOffset = -1;
			return CreateNewFoldings(document);
		}
		
		/// <summary>
		/// Create <see cref="NewFolding"/>s for the specified document.
		/// </summary>
		public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
		{
			List<NewFolding> newFoldings = new List<NewFolding>();
			
			Stack<int> startOffsets = new Stack<int>();
			int lastNewLineOffset = 0;
			
			for (int i = 0; i < document.TextLength-2; i++) {
				char c = document.GetCharAt(i);
                char c1 = document.GetCharAt(i+1);
                char c2 = document.GetCharAt(i+2);
                if ((c1 == '(' && (c == 'U' || c == 'O' || c == 'X')) || (c2 == '(' && c1=='N' && (c == 'U' || c == 'O' || c == 'X')))
                {
                    startOffsets.Push(i);
				} else if (c == ')' && startOffsets.Count > 0) {
					int startOffset = startOffsets.Pop();
					// don't fold if opening and closing brace are on the same line
					if (startOffset < lastNewLineOffset) {
						newFoldings.Add(new NewFolding(startOffset, i + 1));
					}
				} else if (c == '\n' || c == '\r') {
					lastNewLineOffset = i + 1;
				}
			}
		    int wrt = document.Text.IndexOf("Netzwerk");
            //try
            {
                while (wrt >= 0)
                {
                    int old = wrt;
                    wrt = document.Text.IndexOf("Netzwerk", wrt + 1);
                    if (wrt > old)
                        newFoldings.Add(new NewFolding(old, wrt - 2));
                    else
                    {
                        newFoldings.Add(new NewFolding(old, document.Text.Length));
                        break;                     
                    }
                }
            }
            //catch (Exception)
            {}


		    newFoldings.Sort((a,b) => a.StartOffset.CompareTo(b.StartOffset));
			return newFoldings;
		}
	}
}
