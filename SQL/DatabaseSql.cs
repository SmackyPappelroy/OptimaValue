using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        string conString;
        if (serverString == "")
        {
            conString = ConnectionString;
        }
        else
        {
            conString = serverString;
        }
        using SqlConnection con = new(conString);

        try
        {
            using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(timeOut);

            await con.OpenAsync(cancellationTokenSource.Token);

            DatabaseStatus.isConnected = true;
            isConnected = true;
            return true;
        }
        catch (TaskCanceledException ex1)
        {
            DatabaseStatus.isConnected = false;
            isConnected = false;
            return false;
        }
        catch (SqlException ex)
        {
            DatabaseStatus.isConnected = false;
            isConnected = false;
            return false;
        }
    }


    /// <summary>
    /// Does the tag exist?
    /// </summary>
    /// <param name="tagId"></param>
    /// <returns></returns>
    public static bool TagExist(int tagId)
    {
        object result = new object();
        var query = $"SELECT TOP 1 name FROM {Settings.Databas}.dbo.tagConfig ";
        query += $"WHERE id = {tagId}";
        try
        {
            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new(query, con);
            con.Open();
            result = cmd.ExecuteScalar();
        }
        catch (SqlException ex)
        {
            Apps.Logger.Log(string.Empty, Severity.Error, ex);
        }
        if (result != null)
            return true;
        return false;
    }

    /// <summary>
    /// Gets all tags from sql or tags for one or all <see cref="ExtendedPlc"/>
    /// </summary>
    /// <param name="plcName"></param>
    /// <returns></returns>
    public static DataTable GetTags(string plcName)
    {
        var tbl = new DataTable();
        var query = $"SELECT * FROM {Settings.Databas}.dbo.tagConfig";
        try
        {
            using SqlConnection con = new(ConnectionString);
            if (plcName == "")
            {
                using SqlCommand cmd = new(query, con);

                con.Open();

                using (SqlDataAdapter da = new(cmd))
                {
                    if (tbl != null)
                        tbl.Clear();
                    da.Fill(tbl);
                }
                return tbl;
            }
            else
            {
                query = $"SELECT * FROM {Settings.Databas}.dbo.tagConfig WHERE plcName = '{plcName}'";
                using SqlCommand cmd = new(query, con);

                con.Open();
                using (SqlDataAdapter da = new(cmd))
                {
                    da.Fill(tbl);
                }
                return tbl;
            }
        }
        catch (SqlException ex)
        {
            Apps.Logger.Log(string.Empty, Severity.Error, ex);
            return null;
        }
        catch (Exception ex)
        {
            Apps.Logger.Log(string.Empty, Severity.Error, ex);
            return null;
        }
    }

    public static bool UpdateTag(string query)
    {
        try
        {
            using (SqlConnection con = new(ConnectionString))
            {
                using SqlCommand cmd = new(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
            }
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
        var query = $"DELETE FROM {Settings.Databas}.dbo.logValues ";
        query += $"WHERE tag_id = {tagId}";
        try
        {
            using SqlConnection con = new SqlConnection(ConnectionString);
            using SqlCommand cmd = new SqlCommand(query, con);

            con.Open();
            cmd.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            "Lyckas ej ta bort tag".SendStatusMessage(Severity.Error, ex);
        }


        query = $"DELETE FROM {Settings.Databas}.dbo.tagConfig ";
        query += $"WHERE id = {tagId}";
        try
        {
            using SqlConnection con = new SqlConnection(ConnectionString);
            using SqlCommand cmd = new SqlCommand(query, con);

            con.Open();
            cmd.ExecuteNonQuery();
        }
        catch (SqlException ex2)
        {
            "Lyckas ej ta bort tag".SendStatusMessage(Severity.Error, ex2);
        }
    }

    public static bool CheckForDuplicateTagNames(string tagName, string plcName)
    {
        object result = new object();
        var query = $"SELECT TOP 1 name FROM {Settings.Databas}.dbo.tagConfig ";
        query += $"WHERE name = '{tagName}' AND plcName = '{plcName}'";
        try
        {
            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new(query, con);
            con.Open();
            result = cmd.ExecuteScalar();
        }
        catch (SqlException ex)
        {
            "Lyckas ej kolla om det finns dubletter av taggar".SendStatusMessage(Severity.Error, ex);
        }
        if (result != null)
            return true;
        return false;
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
            using SqlDataAdapter adp = new SqlDataAdapter(cmd);
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
        var tbl = new DataTable();
        string query = "SELECT * FROM " + Settings.Databas + ".dbo.plcConfig";
        using SqlConnection con = new SqlConnection(ConnectionString);
        using SqlCommand cmd = new SqlCommand(query, con);
        try
        {
            con.Open();
            using SqlDataAdapter da = new SqlDataAdapter(cmd);

            tbl.Clear();
            da.Fill(tbl);
            return tbl;
        }
        catch (SqlException ex)
        {
            "Lyckas ej hämta PLC tabell".SendStatusMessage(Severity.Error, ex);
            return null;
        }
    }

    /// <summary>
    /// Delete PLC from SQL
    /// </summary>
    /// <param name="id"></param>
    public static void DeletePlc(int id)
    {
        var query = $"DELETE FROM {Settings.Databas}.dbo.plcConfig ";
        query += $"WHERE id ='{id}'";
        try
        {
            using SqlConnection con = new SqlConnection(ConnectionString);
            using SqlCommand cmd = new SqlCommand(query, con);

            con.Open();
            cmd.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            $"Misslyckades ta bort PLC från SQL\r\n{ex.Message}".SendStatusMessage(Severity.Error);
        }
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
        string query;
        query = $"UPDATE {Settings.Databas}.dbo.plcConfig SET ";
        query += $"syncTimeDbNr={syncDb},syncTimeOffset={syncByte},syncBoolAddress='{syncBool}',syncActive='{activeString}'";
        query += $" WHERE id = {plcId}";

        using SqlConnection con = new SqlConnection(ConnectionString);
        using SqlCommand cmd = new SqlCommand(query, con);
        con.Open();
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Check if PLC exist in database
    /// </summary>
    /// <param name="plcName"></param>
    /// <returns></returns>
    public static bool DoesPlcExist(string plcName)
    {
        object result = new object();
        var query = $"Select top 1 name FROM {Settings.Databas}.dbo.plcConfig WHERE name ='{plcName}'";
        try
        {
            using SqlConnection con = new SqlConnection(ConnectionString);
            using SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            result = cmd.ExecuteScalar();

        }
        catch (SqlException ex)
        {
            $"Misslyckades att läsa från SQL\r\n{ex.Message}".SendStatusMessage(Severity.Error);
        }

        if (result != null)
            return true;
        else
            return false;
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
            string query;
            query = $"UPDATE {Settings.Databas}.dbo.plcConfig SET active='{activeString}',name='{name}'";
            query += $",ipAddress='{ip}',cpuType='{cpu}',rack={rack},slot={slot}";
            query += $" WHERE id = {id}";

            using (SqlConnection con = new(ConnectionString))
            {
                using SqlCommand cmd = new(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            query = $"UPDATE {Settings.Databas}.dbo.tagConfig SET plcName='{name}' ";
            query += $"WHERE plcName = '{plcName}'";
            using (SqlConnection con = new(ConnectionString))
            {
                using SqlCommand cmd = new(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        else
        {
            string query;
            query = $"INSERT INTO {Settings.Databas}.dbo.plcConfig (active,name,ipAddress,cpuType,rack,slot,";
            query += $"syncTimeDbNr,syncTimeOffset,syncActive,syncBoolAddress,lastSyncTime)";
            query += $"VALUES ('{activeString}','{name}','{ip}','{cpu}',{rack},{slot},";
            query += $"0,0,'False','DBX0.0','{DateTime.UtcNow - TimeSpan.FromDays(1)}')";
            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new(query, con);
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// Writes PLC-time synchronization time to SQL
    /// </summary>
    /// <param name="tid"></param>
    /// <param name="plcName"></param>
    public static void SaveSyncTime(DateTime tid, string plcName)
    {
        var query = $"UPDATE {Settings.Databas}.dbo.plcConfig SET lastSyncTime = '{tid}' WHERE name = '{plcName}'";
        try
        {
            using SqlConnection con = new(ConnectionString);
            using SqlCommand cmd = new(query, con);
            con.Open();
            cmd.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            $"Lyckades ej skriva till databas vid tids-synkning: {plcName}".SendStatusMessage(Severity.Error, ex);
        }
    }

    /// <summary>
    /// Sends logvalues in bulk to SQL
    /// </summary>
    /// <param name="tbl"></param>
    public static void SendLogValuesToSql(DataTable tbl)
    {

        using SqlConnection con = new SqlConnection(ConnectionString);
        using SqlBulkCopy objBulk = new SqlBulkCopy(ConnectionString);

        objBulk.DestinationTableName = "logValues";
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
}
