using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptimaValue.Config;

namespace OptimaValue.Wpf
{

    internal class SqlHelper
    {
        public static async Task<List<Tag>> GetDistinctTags()
        {
            var tbl = new DataTable();
            var query = $"SELECT * FROM {Settings.Databas}.dbo.tagConfig";
            using var con = new SqlConnection(Settings.ConnectionString);
            using var cmd = new SqlCommand(query, con);
            con.Open();
            using var reader = await cmd.ExecuteReaderAsync();
            tbl.Load(reader);
            return tbl.AsEnumerable().Select(row => new Tag
            {
                TagId = row.Field<int>("id"),
                Name = row.Field<string>("name"),
                Description = row.Field<string>("description") == null ? string.Empty : row.Field<string>("description"),
                Unit = row.Field<string>("tagUnit") == null ? string.Empty : row.Field<string>("tagUnit"),
                PlcName = row.Field<string>("plcName"),
                UpdateTime = LogFrequencyEnumToTimeSpan(row.Field<string>("logFreq")),
            }).OrderBy(x => x.Name).ToList();
        }

        private static TimeSpan LogFrequencyEnumToTimeSpan(string logFrequency)
        {
            switch (logFrequency)
            {
                case "_50ms":
                    return TimeSpan.FromMilliseconds(50);
                case "_100ms":
                    return TimeSpan.FromMilliseconds(100);
                case "_250ms":
                    return TimeSpan.FromMilliseconds(250);
                case "_500ms":
                    return TimeSpan.FromMilliseconds(500);
                case "_1s":
                    return TimeSpan.FromSeconds(1);
                case "_2s":
                    return TimeSpan.FromSeconds(2);
                case "_5s":
                    return TimeSpan.FromSeconds(5);
                case "_10s":
                    return TimeSpan.FromSeconds(10);
                case "_30s":
                    return TimeSpan.FromSeconds(30);
                case "_1m":
                    return TimeSpan.FromMinutes(1);
                case "_5m":
                    return TimeSpan.FromMinutes(5);
            }
            return TimeSpan.FromSeconds(1);
        }
    }
}
