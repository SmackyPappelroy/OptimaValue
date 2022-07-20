using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue.Wpf
{
    public class TagListControlModel
    {


        public TagListControlModel()
        {
        }

        public List<GridItem> GetGridItems()
        {
            var query = $"SELECT id,name,description FROM {Config.Settings.Databas}.dbo.tagConfig";
            using var con = new SqlConnection(Config.SqlMethods.ConnectionString);
            using var cmd = new SqlCommand(query, con);
            con.Open();
            var result = cmd.ExecuteReader();
            return
                result.Cast<IDataRecord>()
                    .Select(x => new GridItem
                    {
                        id = x.GetInt32(0),
                        name = x.GetString(1),
                        description = x.IsDBNull(2) ? string.Empty : x.GetString(2)
                    })
                    .ToList();

        }

    }
}
