using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HamrahBina.Models.ViewModels.General
{
    /// <summary>
    /// Used in selectable items in a list, for instance dropdowns
    /// </summary>
    public class SelectItem
    {
        /// <summary>
        /// Gets or sets if the item is selected
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Gets or sets the title(text) of the item
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the value of the item
        /// </summary>
        public string Value { get; set; }
    }
}
