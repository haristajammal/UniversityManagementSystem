using MySql.Data.MySqlClient;
using UniversityManagementSystem.Helpers;

namespace UniversityManagementSystem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            new Students().ShowDialog();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            new Departments().ShowDialog();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            new Students().ShowDialog();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            new Courses().ShowDialog();
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            new Fees().ShowDialog();
        }
        private void pictureBox9_Click(object sender, EventArgs e)
        {
            new Colleges().ShowDialog();
        }
        private void pictureBox10_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
        "Do you really want to log out and close the application?",
        "Log Out",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            new Professors().ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadStatistics();
            LoadTotals();
        }
        private void LoadTotals()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sqlFinance = @"
                        SELECT COALESCE(SUM(amount), 0) 
                        FROM fees
                        WHERE status = 'Paid'";

                    using (var cmd = new MySqlCommand(sqlFinance, conn))
                    {
                        decimal totalFinance = Convert.ToDecimal(cmd.ExecuteScalar());
                        label27.Text = "Rs " + totalFinance.ToString("N0");
                    }

                    string sqlSalary = @"
                        SELECT COALESCE(SUM(amount), 0) 
                        FROM salary
                        WHERE status = 'Received'";

                    using (var cmd = new MySqlCommand(sqlSalary, conn))
                    {
                        decimal totalSalary = Convert.ToDecimal(cmd.ExecuteScalar());
                        label28.Text = "Rs " + totalSalary.ToString("N0");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading totals:\n" + ex.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Fallback display when error occurs
                label27.Text = "Rs —";
                label28.Text = "Rs —";
            }
        }
        private void LoadStatistics()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM students", conn))
                    {
                        label12.Text = cmd.ExecuteScalar().ToString();
                    }
                    using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM departments", conn))
                    {
                        label14.Text = cmd.ExecuteScalar().ToString();
                    }
                    using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM professors", conn))
                    {
                        label18.Text = cmd.ExecuteScalar().ToString();
                    }
                    using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM college", conn))
                    {
                        label16.Text = cmd.ExecuteScalar().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dashboard statistics:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox14_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click_1(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox7_Click_1(object sender, EventArgs e)
        {
            new Fees().ShowDialog();
        }

        private void label27_Click(object sender, EventArgs e)
        {

        }

        private void label28_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }
    }
}
