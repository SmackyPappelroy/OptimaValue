using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue.Trend
{
    public class MainWindowModel
    {
        public MainWindowModel()
        {

        }
        //public List<GridItem> GetGridItems(System.Windows.Media.Color backgroundColor)
        public List<GridItem> GetGridItems()
        {
            var query = $"SELECT id,name,description,scaleMin,scaleMax,scaleOffset FROM {Config.Settings.Databas}.dbo.tagConfig";
            using var con = new SqlConnection(Config.SqlMethods.ConnectionString);
            using var cmd = new SqlCommand(query, con);
            con.Open();
            var result = cmd.ExecuteReader();
            return
                result.Cast<IDataRecord>()
                    //.Select(x => new GridItem(backgroundColor)
                    .Select(x => new GridItem()
                    {
                        id = x.GetInt32(0),
                        name = x.GetString(1),
                        description = x.IsDBNull(2) ? string.Empty : x.GetString(2),
                        scaleMin = (float)(x.IsDBNull(3) ? 0 : x.GetDouble(3)),
                        scaleMax = (float)(x.IsDBNull(4) ? 0 : x.GetDouble(4)),
                        scaleOffset = (float)(x.IsDBNull(5) ? 0 : x.GetDouble(5)),
                    })
                    .ToList();

        }

    }
}
