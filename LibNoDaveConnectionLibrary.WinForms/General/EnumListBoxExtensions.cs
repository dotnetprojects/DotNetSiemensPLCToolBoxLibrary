/*
 This implements a high level Wrapper between libnodave.dll and applications written
 in MS .Net languages.
 
 This ConnectionLibrary was written by Jochen Kuehner
 * http://jfk-solutuions.de/
 * 
 * Thanks go to:
 * Steffen Krayer -> For his work on MC7 decoding and the Source for his Decoder
 * Zottel         -> For LibNoDave

 WPFToolboxForSiemensPLCs is free software; you can redistribute it and/or modify
 it under the terms of the GNU Library General Public License as published by
 the Free Software Foundation; either version 2, or (at your option)
 any later version.

 WPFToolboxForSiemensPLCs is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU Library General Public License
 along with Libnodave; see the file COPYING.  If not, write to
 the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.  
*/

#if !IPHONE
using System;
using System.Windows.Forms;

namespace DotNetSiemensPLCToolBoxLibrary.General
{
    public static class EnumListBoxExtensions
    {
        public static void SelectEnumListItem(/* this */ ComboBox myList, int number)
        {
            foreach (EnumListItem myItm in myList.Items)
            {
                if (myItm.Value == number)
                {
                    myList.SelectedItem = myItm;
                    return;
                }
            }
            myList.SelectedItem = null;
        }

        public static void SelectEnumListItem(/* this */ ListBox myList, int number)
        {
            foreach (EnumListItem myItm in myList.Items)
            {
                if (myItm.Value == number)
                {
                    myList.SelectedItem = myItm;
                    return;
                }
            }
            myList.SelectedItem = null;
        }

        public static void AddEnumToList(/* this */ ListBox myList,Type myEnum)
        {
            foreach (string myType in Enum.GetNames(myEnum))
                myList.Items.Add(new EnumListItem(myType, (int)Enum.Parse(myEnum, myType)));
        }

        public static void AddEnumToList(/* this */ ComboBox myList, Type myEnum)
        {
            foreach (string myType in Enum.GetNames(myEnum))
                myList.Items.Add(new EnumListItem(myType, (int)Enum.Parse(myEnum, myType)));
        }
    }

    public class EnumListItem
    {
        private string _name;
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private int _value;
        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public EnumListItem(string name, int value)
        {
            _value = value;
            _name = name;
        }

        public override string ToString()
        {
            return Name.Replace('_', ' ');
        }
    }
}
#endif