using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OptimaValue.Wpf
{
    public class GridItem
    {
        public int id { get; set; }
        [ColumnName("Namn")]
        public string name { get; set; }
        [ColumnName("Beskrivning")]
        public string? description { get; set; }

    }

    public class ColumnNameAttribute : System.Attribute
    {
        public ColumnNameAttribute(string Name) { this.Name = Name; }
        public string Name { get; set; }
    }


}
