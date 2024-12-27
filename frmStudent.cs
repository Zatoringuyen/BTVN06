using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BAI5.BUS;
using BAI5.DAL.Entities;



namespace BAI5
{
    public partial class frmStudent : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        public frmStudent()
        {
            InitializeComponent();
        }

        private void frmStudent_Load(object sender, EventArgs e)
        {
            try
            {
                setGridViewStyle(dgvStudent);
                var listFacultys = facultyService.GetAll();
                var listStudents = studentService.GetAll();
                FillFalcultyCombobox(listFacultys);
                BindGrid(listStudents);
            }
            catch (Exception ex)

            {
                MessageBox.Show(ex.Message);
            }
        }
       
        private void FillFalcultyCombobox(List<Faculty> listFacultys)
        {
            this.cmbFaculty.DataSource = listFacultys;
            this.cmbFaculty.DisplayMember = "FacultyName";
            this.cmbFaculty.ValueMember = "FacultyID";

        }
        //Hàm binding gridView từ list sinh viên
        private void BindGrid(List<Student> listStudent)
        {
            dgvStudent.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[0].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[1].Value = item.FullName;
                if (item.Faculty != null)

                    dgvStudent.Rows[index].Cells[2].Value =
                    item.Faculty.FacultyName;
                dgvStudent.Rows[index].Cells[3].Value = item.AverageScore + "";
                if (item.MajorID != null)
                    dgvStudent.Rows[index].Cells[4].Value = item.Major.Name + "";
                ShowAvatar(item.Avatar);
            }
        }
        private void ShowAvatar(string ImageName)
        {
            if (string.IsNullOrEmpty(ImageName))
            {
                picAvatar.Image = null;
            }
            else
            {
                string imagesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                string imagePath = Path.Combine(imagesDirectory, ImageName);

                if (File.Exists(imagePath))
                {
                    picAvatar.Image = Image.FromFile(imagePath);
                    picAvatar.Refresh();
                }
                else
                {
                    picAvatar.Image = null;
                }
            }
        }

        public void setGridViewStyle(DataGridView dgview)
        {
            dgview.BorderStyle = BorderStyle.None;
            dgview.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgview.CellBorderStyle =
            DataGridViewCellBorderStyle.SingleHorizontal;
            dgview.BackgroundColor = Color.White;
            dgview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void chkUnregisterMajor_CheckedChanged(object sender, EventArgs e)
        {
            var listStudents = new List<Student>();
            if (this.chkUnregisterMajor.Checked)

                listStudents = studentService.GetAllHasNoMajor();
            else
                listStudents = studentService.GetAll();
            BindGrid(listStudents);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Chọn ảnh đại diện"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string extension = Path.GetExtension(openFileDialog.FileName); // Lấy phần mở rộng
                string studentID = txtStudentID.Text.Trim(); // Lấy StudentID từ TextBox
                if (string.IsNullOrEmpty(studentID))
                {
                    MessageBox.Show("Vui lòng nhập mã sinh viên trước khi chọn ảnh.");
                    return;
                }

                string imagesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                if (!Directory.Exists(imagesDirectory))
                {
                    Directory.CreateDirectory(imagesDirectory);
                }

                string newFileName = $"{studentID}{extension}";
                string newFilePath = Path.Combine(imagesDirectory, newFileName);

                File.Copy(openFileDialog.FileName, newFilePath, true); // Ghi đè nếu đã tồn tại

                picAvatar.Image = Image.FromFile(newFilePath);
                picAvatar.ImageLocation = newFilePath;
            }
        }

        private void btnAddUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                var student = new Student
                {
                    StudentID = txtStudentID.Text.Trim(),
                    FullName = txtFullName.Text.Trim(),
                    FacultyID = (cmbFaculty.SelectedItem as Faculty)?.FacultyID ?? 0,
                    AverageScore = float.Parse(txtDiem.Text),
                    Avatar = Path.GetFileName(picAvatar.ImageLocation)
                }; 

                if (string.IsNullOrEmpty(student.StudentID) || string.IsNullOrEmpty(student.FullName) || student.FacultyID == 0)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                    return;
                }

                studentService.AddOrUpdate(student);
                MessageBox.Show("Thêm hoặc cập nhật sinh viên thành công!");

                var listStudents = studentService.GetAll();
                BindGrid(listStudents);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        private void dgvStudent_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                txtStudentID.Text = dgvStudent.Rows[e.RowIndex].Cells[0].Value.ToString();
                txtFullName.Text = dgvStudent.Rows[e.RowIndex].Cells[1].Value.ToString();
                cmbFaculty.Text = dgvStudent.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtDiem.Text = dgvStudent.Rows[e.RowIndex].Cells[3].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi chọn dòng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new Model1())
                {

                    string studentID = txtStudentID.Text;
                    Student studentToDelete = context.Student.FirstOrDefault(s => s.StudentID == studentID);

                    if (studentToDelete != null)
                    {
                        context.Student.Remove(studentToDelete);
                        context.SaveChanges();
                        BindGrid(context.Student.ToList());
                        MessageBox.Show("Xóa sinh viên thành công!", "Thông báo");
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa sinh viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    
}
