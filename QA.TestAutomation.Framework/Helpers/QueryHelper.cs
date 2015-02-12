using System.Configuration;
using System.Data.SqlClient;

namespace QA.TestAutomation.Framework.Helpers
{
    public class QueryHelper
    {
        public static void RunQuery(string queryString, string connectionString = "Main")
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionString].ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(queryString, connection);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
