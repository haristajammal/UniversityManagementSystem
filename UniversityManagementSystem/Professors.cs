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
    public partial class Professors : Form
    {
        private int selectedProfessorId = 0;
        public Professors()
        {
            InitializeComponent();
        }

        private void Professors_Load(object sender, EventArgs e)
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ClearSelection();
            LoadDepartmentsIntoComboBox();
            LoadProfessorsIntoGrid();
            SetupGenderComboBox();
            ClearFields();
        }

        private void LoadDepartmentsIntoComboBox()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT dept_id, dept_name FROM departments ORDER BY dept_name";
                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        comboBox4.DataSource = dt;
                        comboBox4.DisplayMember = "dept_name";
                        comboBox4.ValueMember = "dept_id";
                        comboBox4.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load departments:\n" + ex.Message, "Error");
            }
        }


        private void LoadProfessorsIntoGrid()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"SELECT professor_id, professor_name, date_of_birth, gender, address, qualification, dept_id,
                                    dept_name, salary, experience_years FROM professors ORDER BY professor_name";
                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;

                        // Nice column headers
                        dataGridView1.Columns["professor_id"].HeaderText = "ID";
                        dataGridView1.Columns["professor_name"].HeaderText = "Professor Name";
                        dataGridView1.Columns["date_of_birth"].HeaderText = "Date of Birth";
                        dataGridView1.Columns["dept_id"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load professors:\n" + ex.Message, "Error");
            }
        }

        private void SetupGenderComboBox()
        {
            comboBox2.Items.AddRange(new string[] { "Male", "Female", "Rather Not to Say" });
            comboBox2.SelectedIndex = -1;
        }
        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dataGridView1.Rows[e.RowIndex];

            selectedProfessorId = Convert.ToInt32(row.Cells["professor_id"].Value);

            textBox1.Text = row.Cells["professor_name"].Value?.ToString() ?? "";

            dateTimePicker1.Value =
                row.Cells["date_of_birth"].Value != DBNull.Value
                ? Convert.ToDateTime(row.Cells["date_of_birth"].Value)
                : DateTime.Today;

            comboBox2.Text = row.Cells["gender"].Value?.ToString() ?? "";
            textBox3.Text = row.Cells["address"].Value?.ToString() ?? "";
            comboBox1.Text = row.Cells["qualification"].Value?.ToString() ?? "";

            comboBox4.SelectedValue = row.Cells["dept_id"].Value;
            textBox4.Text = row.Cells["dept_name"].Value?.ToString() ?? "";

            textBox2.Text = row.Cells["salary"].Value?.ToString() ?? "";
            textBox5.Text = row.Cells["experience_years"].Value?.ToString() ?? "";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox4.SelectedIndex >= 0)
            {
                textBox4.Text = comboBox4.Text;
            }
            else
            {
                textBox4.Clear();
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
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
                    string sql = @"INSERT INTO professors (professor_name, date_of_birth, gender, address, qualification, 
                                   dept_id, dept_name, salary, experience_years) VALUES (@name, @dob, @gender, 
                                   @address, @qual, @deptid, @deptname, @salary, @exp)";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", textBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@dob", dateTimePicker1.Value.Date);
                        cmd.Parameters.AddWithValue("@gender", comboBox2.Text);
                        cmd.Parameters.AddWithValue("@address", textBox3.Text.Trim());
                        cmd.Parameters.AddWithValue("@qual", comboBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@deptid", comboBox4.SelectedValue);
                        cmd.Parameters.AddWithValue("@deptname", textBox4.Text.Trim());
                        cmd.Parameters.AddWithValue("@salary", decimal.Parse(textBox2.Text.Trim()));
                        cmd.Parameters.AddWithValue("@exp", string.IsNullOrWhiteSpace(textBox5.Text) ? 0 : int.Parse(textBox5.Text.Trim()));

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Professor added successfully.", "Success");
                    LoadProfessorsIntoGrid();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving professor:\n" + ex.Message, "Error");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedProfessorId == 0)
            {
                MessageBox.Show("Please select a professor first by clicking a row.", "Warning");
                return;
            }

            if (!ValidateInputs()) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"UPDATE professors SET professor_name = @name,date_of_birth = @dob,gender = @gender,
                            address = @address,qualification = @qual,dept_id = @deptid,dept_name = @deptname,salary = @salary,
                            experience_years = @exp WHERE professor_id = @id";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", selectedProfessorId);
                        cmd.Parameters.AddWithValue("@name", textBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@dob", dateTimePicker1.Value.Date);
                        cmd.Parameters.AddWithValue("@gender", comboBox2.Text);
                        cmd.Parameters.AddWithValue("@address", textBox3.Text.Trim());
                        cmd.Parameters.AddWithValue("@qual", comboBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@deptid", comboBox4.SelectedValue);
                        cmd.Parameters.AddWithValue("@deptname", textBox4.Text.Trim());
                        cmd.Parameters.AddWithValue("@salary", decimal.Parse(textBox2.Text.Trim()));
                        cmd.Parameters.AddWithValue("@exp", string.IsNullOrWhiteSpace(textBox5.Text) ? 0 : int.Parse(textBox5.Text.Trim()));

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Professor updated successfully.", "Success");
                    LoadProfessorsIntoGrid();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating professor:\n" + ex.Message, "Error");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (selectedProfessorId == 0)
            {
                MessageBox.Show("Please select a professor to delete.", "Warning");
                return;
            }

            DialogResult confirm = MessageBox.Show(
                $"Are you sure you want to delete professor '{textBox1.Text}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string sql = "DELETE FROM professors WHERE professor_id = @id";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", selectedProfessorId);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Professor deleted successfully.", "Success");
                LoadProfessorsIntoGrid();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting professor:\n" + ex.Message);
            }
        }

        private void ClearFields()
        {
            selectedProfessorId = 0;

            textBox1.Clear();
            dateTimePicker1.Value = DateTime.Today;
            comboBox2.SelectedIndex = -1;
            textBox3.Clear();
            comboBox1.SelectedIndex = -1;
            comboBox4.SelectedIndex = -1;
            textBox4.Clear();
            textBox2.Clear();
            textBox5.Clear();

            dataGridView1.ClearSelection();
        }
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Professor Name is required.");
                return false;
            }
            if (comboBox4.SelectedValue == null)
            {
                MessageBox.Show("Please select a department.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(textBox2.Text) || !decimal.TryParse(textBox2.Text, out _))
            {
                MessageBox.Show("Salary must be a valid number.");
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
    }
}
