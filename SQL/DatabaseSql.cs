using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileLogger;
using OptimaValue.Config;

namespace OptimaValue;

public static class DatabaseSql
{
    public static bool isConnected;


    /// <summary>
    /// Creates a SQL connection string
    /// </summary>
    /// <returns>A connection string</returns>
    public static string ConnectionString => Settings.ConnectionString;


    /// <summary>
    /// Test connection asynchronously to the <see cref="Microsoft.SqlServer"/>
    /// </summary>
    /// <returns><code>True</code>If Successfull</returns>
    public static async Task<bool> TestConnectionAsync(int timeOut = 1000, string serverString = "")
    {
        string conString = string.IsNullOrEmpty(serverString) ? ConnectionString : serverString;
        using SqlConnection con = new(conString);

        try
        {
            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(timeOut);

            await con.OpenAsync(cancellationTokenSource.Token);

            DatabaseStatus.isConnected = true;
            isConnected = true;
        }
        catch (TaskCanceledException ex1)
        {
            DatabaseStatus.isConnected = false;
            isConnected = false;
        }
        catch (SqlException ex)
        {
            DatabaseStatus.isConnected = false;
            isConnected = false;
        }
        return isConnected;
    }

    // Does the database exist?
    public static bool DatabaseExist()
    {
        var databaseName = Settings.Databas;
        object result;
        string query = $"SELECT TOP 1 name FROM master.dbo.sysdatabases WHERE name = '{databaseName}'";

        try
        {
            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new(query, con);
            con.Open();
            result = cmd.ExecuteScalar();
        }
        catch (SqlException ex)
        {
            Logger.LogError(string.Empty, ex);
            return false;
        }
        return result != null;
    }

    // Check if table exists
    public static bool TableExist()
    {
        var tableName = "tagConfig";
        object result;
        string query = $"SELECT TOP 1 name FROM {Settings.Databas}.sys.tables WHERE name = '{tableName}'";

        try
        {
            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new(query, con);
            con.Open();
            result = cmd.ExecuteScalar();
        }
        catch (SqlException ex)
        {
            Logger.LogError(string.Empty, ex);
            return false;
        }
        return result != null;
    }

    /// <summary>
    /// Does the tag exist?
    /// </summary>
    /// <param name="tagId"></param>
    /// <returns></returns>
    public static bool TagExist(int tagId)
    {
        object result;
        var query = $"SELECT TOP 1 name FROM {Settings.Databas}.dbo.tagConfig WHERE id = {tagId}";

        try
        {
            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new(query, con);
            con.Open();
            result = cmd.ExecuteScalar();
        }
        catch (SqlException ex)
        {
            Logger.LogError(string.Empty, ex);
            return false;
        }
        return result != null;
    }

    /// <summary>
    /// Gets all tags from sql or tags for one or all <see cref="ExtendedPlc"/>
    /// </summary>
    /// <param name="plcName"></param>
    /// <returns></returns>
    public static DataTable GetTags(string plcName)
    {
        DataTable tbl = new DataTable();
        var query = string.IsNullOrEmpty(plcName)
            ? $"SELECT * FROM {Settings.Databas}.dbo.tagConfig"
            : $"SELECT * FROM {Settings.Databas}.dbo.tagConfig WHERE plcName = '{plcName}'";

        try
        {
            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new(query, con);
            con.Open();
            using SqlDataAdapter da = new(cmd);
            if (tbl != null)
                tbl.Clear();
            da.Fill(tbl);
        }
        catch (SqlException ex)
        {
            Logger.LogError(string.Empty, ex);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(string.Empty, ex);
            return null;
        }

        return tbl;
    }

    public static bool UpdateTag(string query)
    {
        try
        {
            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new(query, con);
            con.Open();
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (SqlException ex)
        {
            "Lyckas ej uppdatera tag".SendStatusMessage(Severity.Error, ex);
            return false;
        }
    }

    /// <summary>
    /// Delete one tag from Sql
    /// </summary>
    /// <param name="tagId"></param>
    public static void DeleteTag(int tagId)
    {
        string[] queries = new[]
        {
        $"DELETE FROM {Settings.Databas}.dbo.logValues WHERE tag_id = {tagId}",
        $"DELETE FROM {Settings.Databas}.dbo.tagConfig WHERE id = {tagId}"
    };

        foreach (string query in queries)
        {
            try
            {
                using SqlConnection con = new(ConnectionString);
                using SqlCommand cmd = new(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                "Lyckas ej ta bort tag".SendStatusMessage(Severity.Error, ex);
            }
        }
    }

    public static bool CheckForDuplicateTagNames(string tagName, string plcName)
    {
        string query = $"SELECT TOP 1 name FROM {Settings.Databas}.dbo.tagConfig WHERE name = '{tagName}' AND plcName = '{plcName}'";
        try
        {
            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new(query, con);
            con.Open();
            object result = cmd.ExecuteScalar();
            return result != null;
        }
        catch (SqlException ex)
        {
            "Lyckas ej kolla om det finns dubletter av taggar".SendStatusMessage(Severity.Error, ex);
            return false;
        }
    }

    /// <summary>
    /// Add tag to SQL
    /// </summary>
    /// <param name="query"></param>
    public static void AddTag(string query)
    {
        try
        {
            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new(query, con);
            con.Open();
            cmd.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            "Lyckas ej lägga till tag".SendStatusMessage(Severity.Error, ex);
        }
    }

    /// <summary>
    /// Get the last added tag from SQL
    /// </summary>
    /// <returns></returns>
    public static DataTable GetLastTag()
    {
        DataTable tbl = new DataTable();
        string query = $"SELECT TOP 1 * FROM {Settings.Databas}.dbo.tagConfig ORDER BY id DESC";
        try
        {
            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new(query, con);
            using SqlDataAdapter adp = new(cmd);
            con.Open();
            adp.Fill(tbl);
        }
        catch (SqlException ex)
        {
            "Lyckas ej hämta senaste tillagda tag".SendStatusMessage(Severity.Error, ex);
        }
        return tbl;
    }

    /// <summary>
    /// Retrieves the configured <see cref="Plc"/>s from the Sql-server
    /// </summary>
    /// <returns><see cref="DataTable"/></returns>
    public static DataTable GetPlcDataTable()
    {
        DataTable tbl = new DataTable();
        string query = $"SELECT * FROM {Settings.Databas}.dbo.plcConfig";
        try
        {
            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new(query, con);
            con.Open();
            using SqlDataAdapter da = new(cmd);
            tbl.Clear();
            da.Fill(tbl);
        }
        catch (SqlException ex)
        {
            "Lyckas ej hämta PLC tabell".SendStatusMessage(Severity.Error, ex);
            return null;
        }
        return tbl;
    }

    /// <summary>
    /// Delete PLC from SQL
    /// </summary>
    /// <param name="id"></param>
    public static void DeletePlc(int id)
    {
        string query = $"DELETE FROM {Settings.Databas}.dbo.plcConfig WHERE id ='{id}'";
        ExecuteNonQuery(query, "Misslyckades ta bort PLC från SQL");
    }

    /// <summary>
    /// Saves PLC parameters for syncing time
    /// </summary>
    /// <param name="syncDb"></param>
    /// <param name="syncByte"></param>
    /// <param name="syncBool"></param>
    /// <param name="activeString"></param>
    /// <param name="plcId"></param>
    public static void SavePlcSyncParameters(string syncDb, string syncByte, string syncBool, string activeString, int plcId)
    {
        string query = $"UPDATE {Settings.Databas}.dbo.plcConfig SET syncTimeDbNr={syncDb},syncTimeOffset={syncByte},syncBoolAddress='{syncBool}',syncActive='{activeString}' WHERE id = {plcId}";
        ExecuteNonQuery(query);
    }

    /// <summary>
    /// Check if PLC exist in database
    /// </summary>
    /// <param name="plcName"></param>
    /// <returns></returns>
    public static bool DoesPlcExist(string plcName)
    {
        string query = $"Select top 1 name FROM {Settings.Databas}.dbo.plcConfig WHERE name ='{plcName}'";
        object result = ExecuteScalar(query, "Misslyckades att läsa från SQL");
        return result != null;
    }

    /// <summary>
    /// Saves PLC to SQL
    /// </summary>
    /// <param name="activeString"></param>
    /// <param name="name"></param>
    /// <param name="ip"></param>
    /// <param name="cpu"></param>
    /// <param name="rack"></param>
    /// <param name="slot"></param>
    /// <param name="id"></param>
    /// <param name="plcName"></param>
    public static void SavePlcConfig(string activeString, string name, string ip, string cpu, string rack, string slot, int id, string plcName)
    {
        if (DoesPlcExist(name))
        {
            string query = $"UPDATE {Settings.Databas}.dbo.plcConfig SET active='{activeString}',name='{name}',ipAddress='{ip}',cpuType='{cpu}',rack={rack},slot={slot} WHERE id = {id}";
            ExecuteNonQuery(query);

            query = $"UPDATE {Settings.Databas}.dbo.tagConfig SET plcName='{name}' WHERE plcName = '{plcName}'";
            ExecuteNonQuery(query);
        }
        else
        {
            string query = $"INSERT INTO {Settings.Databas}.dbo.plcConfig (active,name,ipAddress,cpuType,rack,slot,syncTimeDbNr,syncTimeOffset,syncActive,syncBoolAddress,lastSyncTime) VALUES ('{activeString}','{name}','{ip}','{cpu}',{rack},{slot},0,0,'False','DBX0.0','{DateTime.UtcNow - TimeSpan.FromDays(1)}')";
            ExecuteNonQuery(query);
        }
    }

    /// <summary>
    /// Writes PLC-time synchronization time to SQL
    /// </summary>
    /// <param name="tid"></param>
    /// <param name="plcName"></param>
    public static void SaveSyncTime(DateTime tid, string plcName)
    {
        string query = $"UPDATE {Settings.Databas}.dbo.plcConfig SET lastSyncTime = '{tid}' WHERE name = '{plcName}'";
        ExecuteNonQuery(query, $"Lyckades ej skriva till databas vid tids-synkning: {plcName}");
    }

    /// <summary>
    /// Sends logvalues in bulk to SQL
    /// </summary>
    /// <param name="tbl"></param>
    public static void SendLogValuesToSql(DataTable tbl)
    {
        // Replace float.NaN values with DBNull.Value
        foreach (DataRow row in tbl.Rows)
        {
            if (row["numericValue"] is float numericValue && float.IsNaN(numericValue))
            {
                row["numericValue"] = DBNull.Value;
            }
        }

        using SqlConnection con = new(ConnectionString);
        using SqlBulkCopy objBulk = new(ConnectionString)
        {
            DestinationTableName = "logValues"
        };

        objBulk.ColumnMappings.Add("logTime", "logTime");
        objBulk.ColumnMappings.Add("value", "value");
        objBulk.ColumnMappings.Add("numericValue", "numericValue");
        objBulk.ColumnMappings.Add("opcQuality", "opcQuality");
        objBulk.ColumnMappings.Add("tag_id", "tag_id");

        try
        {
            con.Open();
            objBulk.WriteToServer(tbl);
        }
        catch (SqlException ex)
        {
            $"Problem vid lagring till Sql \n\r{ex.Message}".SendStatusMessage(Severity.Error);
        }
    }


    private static void ExecuteNonQuery(string query, string errorMessage = null)
    {
        try
        {
            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new(query, con);
            con.Open();
            cmd.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            if (errorMessage != null)
            {
                $"{errorMessage}\r\n{ex.Message}".SendStatusMessage(Severity.Error);
            }
        }
    }

    private static object ExecuteScalar(string query, string errorMessage = null)
    {
        object result = new object();
        try
        {
            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new(query, con);
            con.Open();
            result = cmd.ExecuteScalar();
        }
        catch (SqlException ex)
        {
            if (errorMessage != null)
            {
                $"{errorMessage}\r\n{ex.Message}".SendStatusMessage(Severity.Error);
            }
        }

        return result;
    }
}
