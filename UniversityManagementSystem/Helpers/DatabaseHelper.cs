using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace UniversityManagementSystem.Helpers
{
    public static class DatabaseHelper
    {

        private static readonly string connectionString ="Server=localhost;" +"Database=university_db;" +"Uid=root;" +            
            "Pwd=Harishaziq99;";      
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

      
        public static bool TestConnection()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database connection failed!\n\n{ex.Message}",
                    "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
