using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HamrahBina.Models.ViewModels.General
{
    /// <summary>
    /// The main class for representing grids data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GridDataResponseViewModel<T>
    {
        /// <summary>
        /// The draw count
        /// </summary>
        public int Draw { get; set; }

        /// <summary>
        /// The total records count of the result
        /// </summary>
        public int RecordsTotal { get; set; }

        /// <summary>
        /// The filtered(based on the search parameters sent by client) records count
        /// </summary>
        public int RecordsFiltered { get; set; }

        /// <summary>
        /// The collection of data which will be passed to client
        /// </summary>
        public IEnumerable<T> Data { get; set; }
    }
}