using System.Data.SqlClient;
using System.Threading.Tasks;

namespace OptimaValue
{
    public static class Optima
    {
        /// <summary>
        /// Test connection to Database<para></para>
        /// Timeout after 2 sec <para></para>
        /// Returns true if successfull
        /// </summary>
        /// <param name="sqlConnectionString"></param>
        /// <returns></returns>
        public static async Task<bool> TestSqlConnectionAsync(this string sqlConnectionString)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(sqlConnectionString))
                {
                    try
                    {
                        await con.OpenAsync();
                        return true;
                    }
                    catch (SqlException)
                    {
                        return false;
                    }
                    finally
                    {
                        con.Dispose();
                    }
                }
            }
            catch (SqlException)
            {
                return false;
            }
        }

        /// <summary>
        /// Builds an <see cref="SQL"/> Connection-string
        /// </summary>
        /// <param name="Server"></param>
        /// <param name="Database"></param>
        /// <param name="User"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static string BuildSqlConnectionString(string Server, string Database, string User, string Password)
        {
            return $"Data Source={Server};Initial Catalog={Database};User ID={User};Password={Password}";
        }
    }
}
