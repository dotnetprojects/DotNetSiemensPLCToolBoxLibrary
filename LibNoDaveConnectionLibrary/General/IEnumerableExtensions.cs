using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotNetSiemensPLCToolBoxLibrary.General
{
    public static class IEnumerableExtensions
    {
        #region Methoden

        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> e, Func<T, IEnumerable<T>> f)
        {
            return e.SelectMany(c => f(c).Flatten(f)).Concat(e);
        }

        public static int Count(this IEnumerable list)
        {
            if (list is ICollection) return ((ICollection)list).Count;

            int result = 0;

            IEnumerator enumerator = list.GetEnumerator();

            while (enumerator.MoveNext()) result++;

            return result;
        }

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

        #endregion Methoden
    }
}