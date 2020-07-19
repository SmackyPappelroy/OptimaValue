using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace OptimaValue
{
    public static class DataTableExtensions
    {
        /// <summary>
        /// Få ut rader som uppfyller specifika fillkor
        /// <para></para>
        /// <example> Exempel:
        /// <code>
        /// DataTable selection = dt.SelectRows("ID > 10", "Name");
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="whereExpression">Villkor att bli uppfyllda</param>
        /// <param name="orderByExpression">Sortering</param>
        /// <returns>Rader som upp fyller villkoren</returns>
        public static DataTable SelectRows(this DataTable tbl, string whereExpression, string orderByExpression)
        {
            tbl.DefaultView.RowFilter = whereExpression;
            tbl.DefaultView.Sort = orderByExpression;
            return tbl.DefaultView.ToTable();
        }

        /// <summary>
        /// Finds a cell value at a specific row index<para></para>
        /// </summary>
        /// <typeparam name="T">The output type</typeparam>
        /// <param name="tbl">The <see cref="DataTable"/></param>
        /// <param name="rowIndex">The row index</param>
        /// <param name="columnName">The name of the column</param>
        /// <returns></returns>
        public static T FindCellValueAtRowIndex<T>(this DataTable tbl, int rowIndex, string columnName)
        {
            if (tbl.Rows.Count == 0)
                return default;
            return (tbl.AsEnumerable().ElementAt(rowIndex).Field<T>(columnName));
        }

        /// <summary>
        /// Hittar de lägsta värdet i en kolumn
        /// </summary>
        /// <typeparam name="T">Ett värde av typen <see cref="IComparable"/></typeparam>
        /// <param name="tbl"></param>
        /// <param name="columnName"></param>
        /// <returns>T som måste vara en <see cref="IComparable"/></returns>
        public static T FindLowestNumber<T>(this DataTable tbl, string columnName) where T : IComparable
        {
            // Om tabellen inte innhåller några rader, returnera "default" värde av T.
            if (tbl.Rows.Count == 0)
                return default;

            // Skapar en lista av T
            List<T> lowestNumberId = new List<T>();

            // Itererar av över alla rader 
            foreach (DataRow rad in tbl.AsEnumerable())
            {
                // Lägger till alla värden i kolumnen "columnName"
                lowestNumberId.Add(rad.Field<T>(columnName));
            }
            // Returnerar de lägsta värdet i listan
            return (lowestNumberId.Min());
        }

        /// <summary>
        /// Hittar de högsta värdet i en kolumn
        /// </summary>
        /// <typeparam name="T">Ett värde av typen <see cref="IComparable"/></typeparam>
        /// <param name="tbl"></param>
        /// <param name="columnName"></param>
        /// <returns>T som måste vara en <see cref="IComparable"/></returns>
        public static T FindHighestNumber<T>(this DataTable tbl, string columnName) where T : IComparable
        {
            // Om tabellen inte innhåller några rader, returnera "default" värde av T.
            if (tbl.Rows.Count == 0)
                return default;

            // Skapar en lista av T
            List<T> highestNumberId = new List<T>();

            // Itererar av över alla rader 
            foreach (DataRow rad in tbl.AsEnumerable())
            {
                // Lägger till alla värden i kolumnen "columnName"
                highestNumberId.Add(rad.Field<T>(columnName));
            }
            // Returnerar de högsta värdet i listan
            return (highestNumberId.Max());
        }

        /// <summary>
        /// Hittar de nästa högre värde i en tabell.<para></para>
        /// Om de bara finns ett värde returneras samma värde
        /// </summary>
        /// <typeparam name="T">Ett värde av typen <see cref="IComparable"/></typeparam>
        /// <param name="tbl"></param>
        /// <param name="currentNumber"></param>
        /// <param name="columnName"></param>
        /// <returns>T som måste vara en <see cref="IComparable"/></returns>
        public static T FindNextNumber<T>(this DataTable tbl, T currentNumber, string columnName) where T : IComparable
        {
            // Om tabellen inte innhåller några rader, returnera "default" värde av T.
            if (tbl.Rows.Count == 0)
                return default;

            // Hittar de högsta värdet i kolumnen
            var highest = FindHighestNumber<T>(tbl, columnName);

            // Hittar de lägsta värdet i kolumnen
            var lowest = FindLowestNumber<T>(tbl, columnName);

            // Om högsta = lägsta returnera lägsta
            if (currentNumber.CompareTo(highest) == 0)
                return lowest;

            // Skapar en lista av T
            List<T> idList = new List<T>();

            // Itererar av över alla rader 
            foreach (DataRow rad in tbl.AsEnumerable())
            {
                idList.Add(rad.Field<T>(columnName));
            }

            // Ta bort alla rader som är lägre eller lika med currentNumber
            idList.RemoveAll(item => item.CompareTo(currentNumber) == -1 || item.CompareTo(currentNumber) == 0);

            // Returnera det minsta värdet
            return idList.Min();
        }

        /// <summary>
        /// Converts a <see cref="IList{T}"/> object to DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns><see cref="DataTable"/></returns>
        public static DataTable ConvertToDataTable<T>(this IList<T> list)
        {
            DataTable table = CreateTable<T>();
            Type entityType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (T item in list)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table;
        }

        private static DataTable CreateTable<T>()
        {
            Type entityType = typeof(T);
            DataTable table = new DataTable(entityType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(
            prop.PropertyType) ?? prop.PropertyType);
            }

            return table;
        }
    }
}
