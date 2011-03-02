using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.General
{
    public static class IEnumerableExtensions
    {
        #region Methoden
        /// <summary>
        /// Konvertiert eine allgemeine Liste in ein Array aus Objekten
        /// </summary>
        /// <param name="list">Liste</param>
        /// <returns>Liste als Array</returns>        
        public static object[] ToArray(/* this */ IEnumerable list)
        {
            return ToArray<object>(list);
        }

        /// <summary>
        /// Konvertiert eine allgemeine Liste in ein Array eines bestimmten Typs
        /// </summary>
        /// <typeparam name="T">Zieltyp des Rückgabearrays</typeparam>
        /// <param name="list">Liste</param>
        /// <returns>Liste als Array</returns>        
        public static T[] ToArray<T>(/* this */ IEnumerable list)
        {
            if (list is T[])
            {
                // muss nicht umgewandelt werden
                return (T[])list;
            }

            // neue Liste erzeugen
            List<T> result = new List<T>();
            {
                foreach (object item in list)
                {
                    result.Add((T)item);
                }
            }
            return result.ToArray();
        }
        #endregion

    }
}
