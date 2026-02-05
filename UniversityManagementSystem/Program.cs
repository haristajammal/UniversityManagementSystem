using UniversityManagementSystem.Helpers;

namespace UniversityManagementSystem
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            if (!DatabaseHelper.TestConnection())
            {
                MessageBox.Show("Cannot connect to the database. Application will close.",
                    "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.Run(new Form1()); 
        }
    }
    }
