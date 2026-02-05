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
    public partial class Courses : Form
    {
        private int selectedCourseId = 0;
        public Courses()
        {
            InitializeComponent();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void Courses_Load(object sender, EventArgs e)
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            LoadDepartmentsIntoComboBox();
            LoadProfessorsIntoComboBox();
            LoadCoursesIntoGrid();
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

                        comboBox1.DataSource = dt;
                        comboBox1.DisplayMember = "dept_name";
                        comboBox1.ValueMember = "dept_id";
                        comboBox1.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load departments:\n" + ex.Message, "Error");
            }
        }

        private void LoadProfessorsIntoComboBox()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT professor_id, professor_name FROM professors ORDER BY professor_name";
                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        comboBox3.DataSource = dt;
                        comboBox3.DisplayMember = "professor_name";
                        comboBox3.ValueMember = "professor_id";
                        comboBox3.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load professors:\n" + ex.Message, "Error");
            }
        }

        private void LoadCoursesIntoGrid()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"SELECT course_id, course_name, credit_hours, dept_id, dept_name, professor_id, professor_name
                        FROM courses ORDER BY course_name";
                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;
                        dataGridView1.AutoGenerateColumns = false;
                        dataGridView1.Columns["course_id"].HeaderText = "ID";
                        dataGridView1.Columns["course_name"].HeaderText = "Course Name";
                        dataGridView1.Columns["credit_hours"].DataPropertyName = "credit_hours";
                        dataGridView1.Columns["dept_id"].DataPropertyName = "dept_id";
                        dataGridView1.Columns["dept_name"].DataPropertyName = "dept_name";
                        dataGridView1.Columns["professor_id"].DataPropertyName = "professor_id";
                        dataGridView1.Columns["professor_name"].DataPropertyName = "professor_name";
                        dataGridView1.Columns["course_id"].Visible = false;
                        dataGridView1.Columns["dept_id"].Visible = false;
                        dataGridView1.Columns["professor_id"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load courses:\n" + ex.Message, "Error");
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            new Departments().ShowDialog();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dataGridView1.Rows[e.RowIndex];

            selectedCourseId = Convert.ToInt32(row.Cells["course_id"].Value);

            textBox1.Text = row.Cells["course_name"].Value?.ToString() ?? "";
            textBox4.Text = row.Cells["credit_hours"].Value?.ToString() ?? "";

            comboBox1.SelectedValue = row.Cells["dept_id"].Value;
            textBox5.Text = row.Cells["dept_name"].Value?.ToString() ?? "";

            if (row.Cells["professor_id"].Value != DBNull.Value)
            {
                comboBox3.SelectedValue = row.Cells["professor_id"].Value;
                textBox2.Text = row.Cells["professor_name"].Value?.ToString() ?? "";
            }
            else
            {
                comboBox3.SelectedIndex = -1;
                textBox2.Clear();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0)
            {
                textBox5.Text = comboBox1.Text;
            }
            else
            {
                textBox5.Clear();
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex >= 0)
            {
                textBox2.Text = comboBox3.Text;
            }
            else
            {
                textBox2.Clear();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
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
                    string sql = @"INSERT INTO courses (course_name, credit_hours, dept_id, dept_name, professor_id,
                                   professor_name) VALUES (@name, @credits, @deptid, @deptname, @profid, @profname)";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", textBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@credits", int.Parse(textBox4.Text.Trim()));
                        cmd.Parameters.AddWithValue("@deptid", comboBox1.SelectedValue);
                        cmd.Parameters.AddWithValue("@deptname", textBox5.Text.Trim());
                        cmd.Parameters.AddWithValue("@profid", comboBox3.SelectedValue ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@profname", textBox2.Text.Trim());

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Course added successfully.", "Success");
                    LoadCoursesIntoGrid();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding course:\n" + ex.Message, "Error");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedCourseId == 0)
            {
                MessageBox.Show("Please select a course first by clicking a row.", "Warning");
                return;
            }

            if (!ValidateInputs()) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"UPDATE courses SET course_name = @name, credit_hours = @credits, dept_id = @deptid,
                            dept_name = @deptname, professor_id = @profid, professor_name = @profname WHERE course_id = @id";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", selectedCourseId);
                        cmd.Parameters.AddWithValue("@name", textBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@credits", int.Parse(textBox4.Text.Trim()));
                        cmd.Parameters.AddWithValue("@deptid", comboBox1.SelectedValue);
                        cmd.Parameters.AddWithValue("@deptname", textBox5.Text.Trim());
                        cmd.Parameters.AddWithValue("@profid", comboBox3.SelectedValue ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@profname", textBox2.Text.Trim());

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Course updated successfully.", "Success");
                    LoadCoursesIntoGrid();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating course:\n" + ex.Message, "Error");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (selectedCourseId == 0)
            {
                MessageBox.Show("Please select a course first.", "Warning");
                return;
            }

            var result = MessageBox.Show(
                $"Delete course '{textBox1.Text}'?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = "DELETE FROM courses WHERE course_id = @id";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", selectedCourseId);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Course deleted successfully.", "Success");
                    LoadCoursesIntoGrid();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting course:\n" + ex.Message, "Error");
            }
        }
        private void ClearFields()
        {
            selectedCourseId = 0;

            textBox1.Clear();
            textBox4.Clear();
            comboBox1.SelectedIndex = -1;
            textBox5.Clear();
            comboBox3.SelectedIndex = -1;
            textBox2.Clear();

            dataGridView1.ClearSelection();
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Course Name is required.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(textBox4.Text) || !int.TryParse(textBox4.Text, out _))
            {
                MessageBox.Show("Credit Hours must be a valid number.");
                return false;
            }
            if (comboBox1.SelectedValue == null)
            {
                MessageBox.Show("Please select a department.");
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

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            new Professors().ShowDialog();
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
