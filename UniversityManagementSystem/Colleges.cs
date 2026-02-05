using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UniversityManagementSystem.Helpers;

namespace UniversityManagementSystem
{
    public partial class Colleges : Form
    {
        private int selectedCollegeId = 0;
        public Colleges()
        {
            InitializeComponent();
        }

        private void Colleges_Load(object sender, EventArgs e)
        {
            LoadCollegesIntoGrid();
            ClearFields();
        }

        private void LoadCollegesIntoGrid()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"SELECT id, college_name, city, principal_name, affiliated_on FROM college 
                        ORDER BY college_name";

                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;

                        // Nice column headers
                        dataGridView1.Columns["id"].HeaderText = "ID";
                        dataGridView1.Columns["college_name"].HeaderText = "College Name";
                        dataGridView1.Columns["city"].HeaderText = "City";
                        dataGridView1.Columns["principal_name"].HeaderText = "Principal Name";
                        dataGridView1.Columns["affiliated_on"].HeaderText = "Affiliated On";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading colleges:\n" + ex.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            selectedCollegeId = 0;
            textBox1.Clear();  // College Name
            textBox4.Clear();  // City
            textBox5.Clear();  // Principal
            dateTimePicker1.Value = DateTime.Today;
            dataGridView1.ClearSelection();
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Enter College Name");
                return false;
            }
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Enter City");
                return false;
            }
            if (string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Enter Principal Name");
                return false;
            }
            return true;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"INSERT INTO college (college_name, city, principal_name, affiliated_on) 
                                   VALUES (@name, @city, @principal, @affiliated)";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", textBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@city", textBox4.Text.Trim());
                        cmd.Parameters.AddWithValue("@principal", textBox5.Text.Trim());
                        cmd.Parameters.AddWithValue("@affiliated", dateTimePicker1.Value.Date);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("College added successfully!", "Success");
                LoadCollegesIntoGrid();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving college:\n" + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedCollegeId == 0)
            {
                MessageBox.Show("Select a college first!");
                return;
            }
            if (!ValidateInputs()) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"UPDATE college 
                                   SET college_name=@name, city=@city, principal_name=@principal, affiliated_on=@affiliated
                                   WHERE id=@id";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", selectedCollegeId);
                        cmd.Parameters.AddWithValue("@name", textBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@city", textBox4.Text.Trim());
                        cmd.Parameters.AddWithValue("@principal", textBox5.Text.Trim());
                        cmd.Parameters.AddWithValue("@affiliated", dateTimePicker1.Value.Date);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("College updated successfully!", "Success");
                LoadCollegesIntoGrid();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating college:\n" + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (selectedCollegeId == 0)
            {
                MessageBox.Show("Select a college first!");
                return;
            }

            var result = MessageBox.Show("Are you sure you want to delete this college?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = "DELETE FROM college WHERE id=@id";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", selectedCollegeId);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("College deleted successfully!", "Deleted");
                LoadCollegesIntoGrid();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting college:\n" + ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dataGridView1.Rows[e.RowIndex];
            selectedCollegeId = Convert.ToInt32(row.Cells["id"].Value);
            textBox1.Text = row.Cells["college_name"].Value.ToString();
            textBox4.Text = row.Cells["city"].Value.ToString();
            textBox5.Text = row.Cells["principal_name"].Value.ToString();
            dateTimePicker1.Value = Convert.ToDateTime(row.Cells["affiliated_on"].Value);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            new Form1().ShowDialog();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            new Students().ShowDialog();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            new Departments().ShowDialog();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            new Professors().ShowDialog();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            new Courses().ShowDialog();
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            new Fees().ShowDialog();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
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
    }
}
