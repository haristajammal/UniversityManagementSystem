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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace UniversityManagementSystem
{
    public partial class Fees : Form
    {
        private int selectedFeeId = 0;
        private int selectedSalaryId = 0;
        public Fees()
        {
            InitializeComponent();
        }


        private void AddRow(PdfPTable table, string label, string value, iTextSharp.text.Font labelFont, iTextSharp.text.Font valueFont)
        {
            PdfPCell cell1 = new PdfPCell(new Phrase(label, labelFont))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                Padding = 8
            };

            PdfPCell cell2 = new PdfPCell(new Phrase(value, valueFont))
            {
                Border = iTextSharp.text.Rectangle.NO_BORDER,
                Padding = 8
            };

            table.AddCell(cell1);
            table.AddCell(cell2);
        }
        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {

        }

        private void Fees_Load(object sender, EventArgs e)
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;

            comboBox2.Items.AddRange(new string[] { "Paid", "Unpaid" });
            comboBox2.SelectedIndex = -1;

            LoadProfessorsIntoComboBox();
            LoadSalaryIntoGrid();
            LoadStudentsIntoComboBox();
            LoadFeesIntoGrid();
            ClearFields();
        }

        private void LoadStudentsIntoComboBox()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT student_id, student_name, dept_name FROM students ORDER BY student_name";
                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        comboBox1.DataSource = dt;
                        comboBox1.DisplayMember = "student_name";
                        comboBox1.ValueMember = "student_id";
                        comboBox1.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading students:\n" + ex.Message);
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            new Students().ShowDialog();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem is DataRowView row)
            {
                textBox1.Text = row["student_name"].ToString();
                textBox4.Text = row["dept_name"].ToString();
            }
            else
            {
                textBox1.Clear();
                textBox4.Clear();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
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
                    string sql = @"INSERT INTO fees
                        (student_id, student_name, dept_name, amount, fee_date, status)
                        VALUES (@sid, @sname, @dept, @amount, @date, @status)";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@sid", comboBox1.SelectedValue);
                        cmd.Parameters.AddWithValue("@sname", textBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@dept", textBox4.Text.Trim());
                        cmd.Parameters.AddWithValue("@amount", decimal.Parse(textBox2.Text));
                        cmd.Parameters.AddWithValue("@date", dateTimePicker2.Value.Date);
                        cmd.Parameters.AddWithValue("@status", comboBox2.Text);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Fee record saved successfully.", "Success");
                LoadFeesIntoGrid();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving fee:\n" + ex.Message);
            }
        }

        private void LoadFeesIntoGrid()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT * FROM fees ORDER BY fee_date DESC";
                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }

                dataGridView1.Columns["fee_id"].Visible = false;
                dataGridView1.Columns["student_id"].Visible = false;
                dataGridView1.AutoGenerateColumns = true;


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading fees:\n" + ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (selectedFeeId == 0)
            {
                MessageBox.Show("Please select a fee record first by clicking a row.", "Warning");
                return;
            }

            var result = MessageBox.Show(
                $"Delete fee for {textBox1.Text} - {textBox2.Text} PKR?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = "DELETE FROM fees WHERE fee_id = @id";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", selectedFeeId);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Fee record deleted.", "Success");
                LoadFeesIntoGrid();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting fee:\n" + ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (selectedFeeId == 0)
            {
                MessageBox.Show("Please select a fee record first.", "Warning");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                FileName = $"Fee_Receipt_{textBox1.Text}_{DateTime.Now:yyyyMMdd}.pdf"
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                Document doc = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                doc.Open();

                // Fonts
                iTextSharp.text.Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                iTextSharp.text.Font labelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
                iTextSharp.text.Font valueFont = FontFactory.GetFont(FontFactory.HELVETICA, 11);

                // Title
                Paragraph title = new Paragraph("UNIVERSITY FEE RECEIPT\n\n", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                doc.Add(title);

                // Table
                PdfPTable table = new PdfPTable(2);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 30f, 70f });

                AddRow(table, "Student ID", comboBox1.SelectedValue.ToString(), labelFont, valueFont);
                AddRow(table, "Student Name", textBox1.Text, labelFont, valueFont);
                AddRow(table, "Department", textBox4.Text, labelFont, valueFont);
                AddRow(table, "Amount (PKR)", textBox2.Text, labelFont, valueFont);
                AddRow(table, "Date", dateTimePicker2.Value.ToShortDateString(), labelFont, valueFont);
                AddRow(table, "Status", comboBox2.Text, labelFont, valueFont);

                doc.Add(table);

                // Footer
                doc.Add(new Paragraph("\n\nAuthorized Signature: ______________________"));
                doc.Add(new Paragraph("\nGenerated on: " + DateTime.Now.ToString("dd-MM-yyyy HH:mm")));

                doc.Close();

                MessageBox.Show("PDF Receipt generated successfully!", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating PDF:\n" + ex.Message, "Error");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dataGridView1.Rows[e.RowIndex];

            selectedFeeId = Convert.ToInt32(row.Cells["fee_id"].Value);
            comboBox1.SelectedValue = row.Cells["student_id"].Value;
            textBox1.Text = row.Cells["student_name"].Value.ToString();
            textBox4.Text = row.Cells["dept_name"].Value.ToString();
            textBox2.Text = row.Cells["amount"].Value.ToString();
            dateTimePicker2.Value = Convert.ToDateTime(row.Cells["fee_date"].Value);
            comboBox2.Text = row.Cells["status"].Value.ToString();
        }

        private void ClearFields()
        {
            selectedFeeId = 0;
            comboBox1.SelectedIndex = -1;
            textBox1.Clear();
            textBox2.Clear();
            textBox4.Clear();
            comboBox2.SelectedIndex = -1;
            dateTimePicker2.Value = DateTime.Today;
            dataGridView1.ClearSelection();
        }

        private bool ValidateInputs()
        {
            if (comboBox1.SelectedValue == null)
            {
                MessageBox.Show("Please select a student.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(textBox2.Text) || !decimal.TryParse(textBox2.Text, out _))
            {
                MessageBox.Show("Amount must be a valid number.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(comboBox2.Text))
            {
                MessageBox.Show("Please select payment status.");
                return false;
            }
            return true;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            new Form1().ShowDialog();
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

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox4.SelectedItem is DataRowView row)
                textBox6.Text = row["professor_name"].ToString();
            else
                textBox6.Clear();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!ValidateSalaryInputs()) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"INSERT INTO salary (professor_id, professor_name, salary_date, amount, status)
                VALUES (@pid, @pname, @date, @amount, @status)";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@pid", comboBox4.SelectedValue);
                        cmd.Parameters.AddWithValue("@pname", textBox6.Text.Trim());
                        cmd.Parameters.AddWithValue("@date", dateTimePicker1.Value.Date);
                        cmd.Parameters.AddWithValue("@amount", decimal.Parse(textBox5.Text));
                        cmd.Parameters.AddWithValue("@status", comboBox3.Text);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Salary record saved successfully!", "Success");
                LoadSalaryIntoGrid();
                ClearSalaryFields();
            }
            catch (Exception ex) { MessageBox.Show("Error saving salary:\n" + ex.Message); }
        }

        private void LoadSalaryIntoGrid()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT * FROM salary ORDER BY salary_date DESC";
                    using (var da = new MySqlDataAdapter(sql, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView2.DataSource = dt;
                    }
                }
                dataGridView2.Columns["salary_id"].Visible = false;
                dataGridView2.Columns["professor_id"].Visible = false;
            }
            catch (Exception ex) { MessageBox.Show("Error loading salaries:\n" + ex.Message); }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (selectedSalaryId == 0)
            {
                MessageBox.Show("Please select a salary record first by clicking a row.", "Warning");
                return;
            }

            var result = MessageBox.Show(
                $"Delete salary for {textBox6.Text} - {textBox5.Text} PKR?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = "DELETE FROM salary WHERE salary_id = @id";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", selectedSalaryId);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Salary record deleted successfully.", "Success");
                LoadSalaryIntoGrid();
                ClearSalaryFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting salary:\n" + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedSalaryId == 0) { MessageBox.Show("Select a salary first."); return; }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                FileName = $"Salary_Receipt_{textBox6.Text}_{DateTime.Now:yyyyMMdd}.pdf"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                Document doc = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                doc.Open();

                iTextSharp.text.Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                iTextSharp.text.Font labelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
                iTextSharp.text.Font valueFont = FontFactory.GetFont(FontFactory.HELVETICA, 11);

                doc.Add(new Paragraph("UNIVERSITY SALARY RECEIPT\n\n", titleFont) { Alignment = Element.ALIGN_CENTER });

                PdfPTable table = new PdfPTable(2) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 30f, 70f });

                AddRow(table, "Professor ID", comboBox4.SelectedValue.ToString(), labelFont, valueFont);
                AddRow(table, "Professor Name", textBox6.Text, labelFont, valueFont);
                AddRow(table, "Amount (PKR)", textBox5.Text, labelFont, valueFont);
                AddRow(table, "Date", dateTimePicker1.Value.ToShortDateString(), labelFont, valueFont);
                AddRow(table, "Status", comboBox3.Text, labelFont, valueFont);

                doc.Add(table);
                doc.Add(new Paragraph("\n\nAuthorized Signature: ______________________"));
                doc.Add(new Paragraph("\nGenerated on: " + DateTime.Now.ToString("dd-MM-yyyy HH:mm")));
                doc.Close();

                MessageBox.Show("PDF generated successfully!", "Success");
            }
            catch (Exception ex) { MessageBox.Show("Error generating PDF:\n" + ex.Message); }
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
                        comboBox4.DataSource = dt;
                        comboBox4.DisplayMember = "professor_name";
                        comboBox4.ValueMember = "professor_id";
                        comboBox4.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error loading professors:\n" + ex.Message); }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dataGridView2.Rows[e.RowIndex];

            selectedSalaryId = Convert.ToInt32(row.Cells["salary_id"].Value);
            comboBox4.SelectedValue = row.Cells["professor_id"].Value;
            textBox6.Text = row.Cells["professor_name"].Value.ToString();
            textBox5.Text = row.Cells["amount"].Value.ToString();
            dateTimePicker1.Value = Convert.ToDateTime(row.Cells["salary_date"].Value);
            comboBox3.Text = row.Cells["status"].Value.ToString();
        }

        private bool ValidateSalaryInputs()
        {
            if (comboBox4.SelectedValue == null) { MessageBox.Show("Select a professor."); return false; }
            if (!decimal.TryParse(textBox5.Text, out _)) { MessageBox.Show("Enter valid amount."); return false; }
            if (string.IsNullOrWhiteSpace(comboBox3.Text)) { MessageBox.Show("Select status."); return false; }
            return true;
        }

        private void ClearSalaryFields()
        {
            selectedSalaryId = 0;
            comboBox4.SelectedIndex = -1;
            textBox6.Clear();
            textBox5.Clear();
            comboBox3.SelectedIndex = -1;
            dateTimePicker1.Value = DateTime.Today;
            dataGridView2.ClearSelection();
        }

    }
}
