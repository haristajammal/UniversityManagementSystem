using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using UniversityManagementSystem.Helpers;
namespace UniversityManagementSystem
{
    public partial class Departments : Form
    {
        private int selectedDeptId = 0;
        public Departments()
        {
            InitializeComponent();
        }

        private void Departments_Load(object sender, EventArgs e)
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;

            LoadDepartmentsIntoGrid();
            ClearFields();

        }

        private void LoadDepartmentsIntoGrid()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT dept_id, dept_name, hod_name, intake, fees FROM departments ORDER BY dept_name";
                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;

                        
                        dataGridView1.Columns["dept_id"].HeaderText = "ID";
                        dataGridView1.Columns["dept_name"].HeaderText = "Department Name";
                        dataGridView1.Columns["hod_name"].HeaderText = "HOD Name";
                        dataGridView1.Columns["intake"].HeaderText = "Intake";
                        dataGridView1.Columns["fees"].HeaderText = "Fees (PKR)";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading departments:\n" + ex.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
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
                    string sql = @"INSERT INTO departments (dept_name, hod_name, intake, fees) VALUES (@dept_name, @hod_name,
                                   @intake, @fees)";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@dept_name", textBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@hod_name", textBox3.Text.Trim());
                        cmd.Parameters.AddWithValue("@intake", string.IsNullOrWhiteSpace(textBox4.Text) ? (object)DBNull.Value : int.Parse(textBox4.Text.Trim()));
                        cmd.Parameters.AddWithValue("@fees", string.IsNullOrWhiteSpace(textBox2.Text) ? (object)DBNull.Value : decimal.Parse(textBox2.Text.Trim()));

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("New department added successfully.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadDepartmentsIntoGrid();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding department:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedDeptId == 0)
            {
                MessageBox.Show("Please select a department to edit.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInputs()) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string sql = @"UPDATE departments 
                           SET dept_name=@dept_name, hod_name=@hod_name,
                               intake=@intake, fees=@fees
                           WHERE dept_id=@id";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", selectedDeptId);
                        cmd.Parameters.AddWithValue("@dept_name", textBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@hod_name", textBox3.Text.Trim());
                        cmd.Parameters.AddWithValue("@intake",
                            string.IsNullOrWhiteSpace(textBox4.Text) ? DBNull.Value : int.Parse(textBox4.Text));
                        cmd.Parameters.AddWithValue("@fees",
                            string.IsNullOrWhiteSpace(textBox2.Text) ? DBNull.Value : decimal.Parse(textBox2.Text));

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Department updated successfully.", "Success");
                LoadDepartmentsIntoGrid();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (selectedDeptId == 0)
            {
                MessageBox.Show("Please select a department to delete.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                $"Are you sure you want to delete department '{textBox1.Text}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string sql = "DELETE FROM departments WHERE dept_id = @id";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", selectedDeptId);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Department deleted successfully.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadDepartmentsIntoGrid();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting department:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearFields()
        {
            selectedDeptId = 0;

            textBox1.Clear();   // Department Name
            textBox3.Clear();   // HOD Name
            textBox4.Clear();   // Intake
            textBox2.Clear();

        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Department Name is required.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("HOD Name is required.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                return false;
            }
            return true;
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dataGridView1.Rows[e.RowIndex];

            selectedDeptId = Convert.ToInt32(row.Cells["dept_id"].Value);

            textBox1.Text = row.Cells["dept_name"].Value.ToString();
            textBox3.Text = row.Cells["hod_name"].Value.ToString();
            textBox4.Text = row.Cells["intake"].Value?.ToString() ?? "";
            textBox2.Text = row.Cells["fees"].Value?.ToString() ?? "";
        }

        
    }
}