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

    public partial class Students : Form
    {
        private int selectedStudentId = 0;
        public Students()
        {
            InitializeComponent();
        }
        private void Students_Load(object sender, EventArgs e)
        {
            LoadDepartmentsIntoComboBox();
            LoadStudentsIntoGrid();
            SetupComboBoxes();
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

                        depID.DataSource = dt;
                        depID.DisplayMember = "dept_name";
                        depID.ValueMember = "dept_id";
                        depID.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load departments:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadStudentsIntoGrid()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"SELECT student_id, student_name, phone, dob, gender, address, dept_id, dept_name, semester
                        FROM students ORDER BY student_name";
                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;
                        dataGridView1.Columns["student_id"].HeaderText = "ID";
                        dataGridView1.Columns["dept_id"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load students:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupComboBoxes()
        {

            gendercomboBox2.Items.AddRange(new string[] { "Male", "Female", "Rather Not to Say" });
            gendercomboBox2.SelectedIndex = -1;


            semcomboBox3.Items.AddRange(new string[]
            {
                "1st Semester", "2nd Semester", "3rd Semester", "4th Semester",
                "5th Semester", "6th Semester", "7th Semester", "8th Semester"
            });
            semcomboBox3.SelectedIndex = -1;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0) return;
            var row = dataGridView1.SelectedRows[0];
            selectedStudentId = Convert.ToInt32(row.Cells["student_id"].Value ?? 0);

            stuName.Text = row.Cells["student_name"].Value?.ToString() ?? "";
            phone.Text = row.Cells["phone"].Value?.ToString() ?? "";
            dateTimePicker1.Value = row.Cells["dob"].Value != DBNull.Value
                                    ? Convert.ToDateTime(row.Cells["dob"].Value)
                                    : DateTime.Today;
            gendercomboBox2.Text = row.Cells["gender"].Value?.ToString() ?? "";
            Address.Text = row.Cells["address"].Value?.ToString() ?? "";
            depID.SelectedValue = row.Cells["dept_id"].Value ?? -1;
            depname.Text = row.Cells["dept_name"].Value?.ToString() ?? "";
            semcomboBox3.Text = row.Cells["semester"].Value?.ToString() ?? "";
        }
        private void cmbDepID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (depID.SelectedIndex >= 0)
            {
                depname.Text = depID.Text;
            }
            else
            {
                depname.Clear();
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }



        private void stuName_TextChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void gendercomboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void depID_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void depname_TextChanged(object sender, EventArgs e)
        {

        }

        private void semcomboBox3_SelectedIndexChanged(object sender, EventArgs e)
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
                    string sql;
                    MySqlCommand cmd;
                    if (selectedStudentId == 0)
                    {
                        sql = @"INSERT INTO students (student_name, phone, dob, gender, address, dept_id, semester, dept_name)
                            VALUES (@name, @phone, @dob, @gender, @address, @dept_id, @semester, @dept_name)";
                        cmd = new MySqlCommand(sql, conn);
                    }
                    else
                    {
                        sql = @"UPDATE students SET student_name = @name, phone = @phone, dob = @dob, gender = @gender,
                                address = @address, dept_id = @dept_id, semester = @semester, dept_name = @dept_name]
                            WHERE student_id = @id";
                        cmd = new MySqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@id", selectedStudentId);
                    }

                    cmd.Parameters.AddWithValue("@name", stuName.Text.Trim());
                    cmd.Parameters.AddWithValue("@phone", phone.Text.Trim());
                    cmd.Parameters.AddWithValue("@dob", dateTimePicker1.Value.Date);
                    cmd.Parameters.AddWithValue("@gender", gendercomboBox2.Text);
                    cmd.Parameters.AddWithValue("@address", Address.Text.Trim());
                    cmd.Parameters.AddWithValue("@dept_id", depID.SelectedValue);
                    cmd.Parameters.AddWithValue("@semester", semcomboBox3.Text);
                    cmd.Parameters.AddWithValue("@dept_name", depname.Text.Trim());

                    cmd.ExecuteNonQuery();

                    MessageBox.Show(
                        selectedStudentId == 0 ? "Student added successfully." : "Student updated successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadStudentsIntoGrid();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving student:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql;
                    MySqlCommand cmd;
                    sql = @"UPDATE students SET student_name = @name, phone = @phone, dob = @dob, gender = @gender,
                                address = @address, dept_id = @dept_id, semester = @semester, dept_name = @dept_name]
                            WHERE student_id = @id";
                    cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", selectedStudentId);
                    cmd.Parameters.AddWithValue("@name", stuName.Text.Trim());
                    cmd.Parameters.AddWithValue("@phone", phone.Text.Trim());
                    cmd.Parameters.AddWithValue("@dob", dateTimePicker1.Value.Date);
                    cmd.Parameters.AddWithValue("@gender", gendercomboBox2.Text);
                    cmd.Parameters.AddWithValue("@address", Address.Text.Trim());
                    cmd.Parameters.AddWithValue("@dept_id", depID.SelectedValue);
                    cmd.Parameters.AddWithValue("@semester", semcomboBox3.Text);
                    cmd.Parameters.AddWithValue("@dept_name", depname.Text.Trim());

                    cmd.ExecuteNonQuery();

                    MessageBox.Show(
                        selectedStudentId == 0 ? "Student updated successfully." : "Student updated successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    LoadStudentsIntoGrid();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving student:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearFields()
        {
            selectedStudentId = 0;
            stuName.Clear();
            phone.Clear();
            dateTimePicker1.Value = DateTime.Today;
            gendercomboBox2.SelectedIndex = -1;
            Address.Clear();
            depID.SelectedIndex = -1;
            depname.Clear();
            semcomboBox3.SelectedIndex = -1;
        }
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(stuName.Text))
            {
                MessageBox.Show("Student name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(phone.Text))
            {
                MessageBox.Show("Phone number is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (depID.SelectedValue == null)
            {
                MessageBox.Show("Please select a department.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (semcomboBox3.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a semester.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            new Departments().ShowDialog();
        }
    }
}