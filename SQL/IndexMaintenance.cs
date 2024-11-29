using System;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue;

public class IndexMaintenance
{
    private static string _connectionString = $"Server={Config.Settings.Server};Database={Config.Settings.Databas};User={Config.Settings.User};Password={Config.Settings.Password};TrustServerCertificate=True;MultipleActiveResultSets=True;";

    public IndexMaintenance( )
    {
    }

    public string OptimizeIndexes()
    {
        StringBuilder resultatLogg = new StringBuilder();
        int åtgärderUtförda = 0;

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            // Hämta fragmenteringsnivåer
            var hämtaFragmenteringQuery = @"
                SELECT 
                    OBJECT_NAME(i.OBJECT_ID) AS TabellNamn,
                    i.name AS IndexNamn,
                    ps.avg_fragmentation_in_percent AS Fragmentering
                FROM 
                    sys.dm_db_index_physical_stats(DB_ID(), OBJECT_ID('logValues'), NULL, NULL, 'DETAILED') AS ps
                INNER JOIN 
                    sys.indexes AS i ON ps.OBJECT_ID = i.OBJECT_ID AND ps.index_id = i.index_id
                WHERE 
                    ps.avg_fragmentation_in_percent > 10
                ORDER BY 
                    ps.avg_fragmentation_in_percent DESC;
            ";

            using (var command = new SqlCommand(hämtaFragmenteringQuery, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var tabellNamn = reader["TabellNamn"].ToString();
                    var indexNamn = reader["IndexNamn"].ToString();
                    var fragmentering = Convert.ToDecimal(reader["Fragmentering"]);

                    if (fragmentering > 30)
                    {
                        // Rebuild index om fragmentering är över 30%
                        RebuildIndex(connection, tabellNamn, indexNamn);
                        resultatLogg.AppendLine($"Index '{indexNamn}' i tabellen '{tabellNamn}' byggdes om. Fragmentering: {fragmentering:F2}%.");
                        åtgärderUtförda++;
                    }
                    else if (fragmentering > 10)
                    {
                        // Reorganize index om fragmentering är mellan 10% och 30%
                        ReorganizeIndex(connection, tabellNamn, indexNamn);
                        resultatLogg.AppendLine($"Index '{indexNamn}' i tabellen '{tabellNamn}' omorganiserades. Fragmentering: {fragmentering:F2}%.");
                        åtgärderUtförda++;
                    }
                }
            }
        }

        if (åtgärderUtförda == 0)
        {
            resultatLogg.AppendLine("Inga index behövde optimeras.");
        }

        return resultatLogg.ToString();
    }


    private void RebuildIndex(SqlConnection connection, string tableName, string indexName)
    {
        var rebuildQuery = $"ALTER INDEX [{indexName}] ON [{tableName}] REBUILD WITH (FILLFACTOR = 80);";
        using (var command = new SqlCommand(rebuildQuery, connection))
        {
            Console.WriteLine($"Rebuilding index: {indexName} on table: {tableName} with FILLFACTOR = 80");
            command.ExecuteNonQuery();
        }
    }


    private void ReorganizeIndex(SqlConnection connection, string tableName, string indexName)
    {
        var reorganizeQuery = $"ALTER INDEX [{indexName}] ON [{tableName}] REORGANIZE;";
        using (var command = new SqlCommand(reorganizeQuery, connection))
        {
            Console.WriteLine($"Reorganizing index: {indexName} on table: {tableName}");
            command.ExecuteNonQuery();
        }
    }

    public static async Task<string> KontrolleraIndexStorlek()
    {
        StringBuilder resultatLogg = new StringBuilder();

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // SQL för att hämta indexstorlek
            var hämtaIndexStorlekQuery = @"
            SELECT 
                    OBJECT_NAME(i.OBJECT_ID) AS TabellNamn,
                    i.name AS IndexNamn,
                    i.type_desc AS IndexTyp,
                    p.reserved_page_count * 8 / 1024 AS Storlek_MB
                FROM 
                    sys.dm_db_partition_stats AS p
                INNER JOIN 
                    sys.indexes AS i ON p.OBJECT_ID = i.OBJECT_ID AND p.index_id = i.index_id
                WHERE 
                    OBJECT_NAME(i.OBJECT_ID) = 'logValues' -- Filtrera för logValues-tabellen
                    AND i.type > 0 -- Uteslut heap
                ORDER BY 
                    Storlek_MB DESC;
        ";

            using (var command = new SqlCommand(hämtaIndexStorlekQuery, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var tabellNamn = reader["TabellNamn"].ToString();
                    var indexNamn = reader["IndexNamn"].ToString();
                    var indexTyp = reader["IndexTyp"].ToString();
                    var storlekMB = Convert.ToDecimal(reader["Storlek_MB"]);

                    resultatLogg.AppendLine($"Index '{indexNamn}' i tabellen '{tabellNamn}' ({indexTyp}) har en storlek på {storlekMB:F2} MB.");
                }
            }
        }

        return resultatLogg.ToString();
    }


}
