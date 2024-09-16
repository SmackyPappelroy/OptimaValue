using FileLogger;
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
            try
            {
                DataView view = new DataView(tbl)
                {
                    RowFilter = whereExpression,
                    Sort = orderByExpression
                };
                return view.ToTable();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Fel vid filtrering/sortering: {ex.Message}");
                return null; 
            }
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
            if (tbl.Rows.Count == 0)
                return default;

            // Använd LINQ för att hitta minsta värdet direkt
            return tbl.AsEnumerable().Min(row => row.Field<T>(columnName));
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
            if (tbl.Rows.Count == 0)
                return default;

            // Använd LINQ för att hitta högsta värdet direkt
            return tbl.AsEnumerable().Max(row => row.Field<T>(columnName));
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
            if (tbl.Rows.Count == 0)
                return default;

            var highest = FindHighestNumber<T>(tbl, columnName);
            var lowest = FindLowestNumber<T>(tbl, columnName);

            // Om högsta = nuvarande nummer, returnera lägsta
            if (currentNumber.CompareTo(highest) == 0)
                return lowest;

            // Hitta nästa nummer som är större än currentNumber
            return tbl.AsEnumerable()
                .Select(row => row.Field<T>(columnName))
                .Where(val => val.CompareTo(currentNumber) > 0)
                .Min();
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
                    try
                    {
                        // Hanterar null-värden och potentiella konverteringsproblem
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    }
                    catch (Exception ex)
                    {
                        // Logga felet eller hantera det på annat sätt
                        Logger.LogError($"Fel vid konvertering av egenskap: {prop.Name}. Felmeddelande: {ex.Message}",ex);
                        row[prop.Name] = DBNull.Value; // Sätt standardvärde vid fel
                    }
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
