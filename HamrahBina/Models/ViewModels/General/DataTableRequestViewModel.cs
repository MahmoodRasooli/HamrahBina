using HamrahBina.Common.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HamrahBina.Models.ViewModels.General
{

    /// <summary>
    /// Represents the search parameter of the column
    /// </summary>
    public class DataTableColumnSearchParam
    {
        /// <summary>
        /// The search value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Shows if the search argument is a regex or not
        /// </summary>
        public bool Regex { get; set; }
    }

    /// <summary>
    /// Represents the columns of the DataTable on the client side
    /// </summary>
    public class DataTableColumn
    {
        /// <summary>
        /// The index of the column
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Name of the column
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The binded field name of the dataset
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Shows if the column is searchable or not
        /// </summary>
        public bool Searchable { get; set; }

        /// <summary>
        /// Shows if the column is orderable or not
        /// </summary>
        public bool Orderable { get; set; }

        /// <summary>
        /// The search arguments of the column
        /// </summary>
        public DataTableColumnSearchParam Search { get; set; }
    }

    /// <summary>
    /// Represents the sort parameters of the DataTable
    /// </summary>
    public class DataTableSortParam
    {
        /// <summary>
        /// The index of the sorted column
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// The name of the sorted column
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// The direction of sort(asc or desc)
        /// </summary>
        public string Dir { get; set; }
    }

    /// <summary>
    /// This is the schema of the DataTable request, which contains the collected data from the client side DataTable(including pageSize, pageNumber, filters, ...)
    /// </summary>
    public class DataTableRequestViewModel
    {
        /// <summary>
        /// The draw property of the DataTable
        /// </summary>
        public int Draw { get; set; }

        /// <summary>
        /// The pageSize of the DataTable
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// The pageNumber of the DataTable
        /// </summary>
        public int Start { get; set; }

        private List<DataTableColumn> _columns;
        /// <summary>
        /// The columns of the DataTable
        /// </summary>
        public IList<DataTableColumn> Columns
        {
            get
            {
                return _columns ?? (_columns = new List<DataTableColumn>());
            }
            set
            {
                _columns = value != null ? value.ToList() : new List<DataTableColumn>();
                var index = 0;
                foreach (var column in _columns)
                    column.Index = index++;
            }
        }

        private List<DataTableSortParam> _order;
        /// <summary>
        /// The sorting parameters of the DataTable
        /// </summary>
        public IList<DataTableSortParam> Order
        {
            get { return _order ?? (_order = new List<DataTableSortParam>()); }
            set
            {
                _order = value != null ? value.ToList() : new List<DataTableSortParam>();
                foreach (var order in _order)
                    order.ColumnName = (Columns?.Any() ?? false) && order.Column < (Columns?.Count() ?? 0) ?
                        (!string.IsNullOrEmpty(Columns[order.Column].Name) ? Columns[order.Column].Name : Columns[order.Column].Data) : "";
            }
        }

        /// <summary>
        /// Gets the combined sort phrase as a string to use in Linq.OrderBy method
        /// </summary>
        public string SortPhrase
        {
            get
            {
                if (Order.Any())
                    return string.Join(",", Order.Select(p => $"{p.ColumnName} {p.Dir}"));
                else
                    return "";
            }
        }

        /// <summary>
        /// The text of the search textbox(if the form contains one)
        /// </summary>
        public DataTableColumnSearchParam Search { get; set; }
    }

    /// <summary>
    /// This class is the same as DataTableRequestViewModel but returns the search object as an Entity or Dto, which will ease the search.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataTableRequestViewModel<T> : DataTableRequestViewModel where T : class
    {
        /// <summary>
        /// Returns an object of type T which it's properties is filled with Columns.Search objects.
        /// So if a Column.Search.Name = "userName" the UserName property of this property will be filled with Column.Search.Value
        /// </summary>
        public T SearchObject
        {
            get
            {
                var result = (T)Activator.CreateInstance(typeof(T));

                foreach (var column in Columns)
                    if (column.Searchable && column.Search != null && !string.IsNullOrEmpty((column.Search.Value ?? "").Trim()))
                    {
                        var columnName = !string.IsNullOrEmpty(column.Name) ? column.Name : column.Data;
                        if (!string.IsNullOrEmpty(columnName))
                        {
                            try
                            {
                                var prop = result.GetType().GetProperty(columnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                                if (prop != null && prop.CanWrite)
                                {
                                    prop.SetValue(result, Convert.ChangeType(column.Search.Value, prop.PropertyType));
                                }
                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                        }
                    }
                return result;
            }
        }
    }
}