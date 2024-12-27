using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BAI5.BUS;
using BAI5.DAL.Entities;


namespace BAI5
{
    public partial class frmRegister : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        private readonly MajorService majorService = new MajorService();
        public frmRegister()
        {
            InitializeComponent();
        }
        private void frmRegister_Load(object sender, EventArgs e)
        {
            try
            {
                var listFacultys = facultyService.GetAll();
                FillFalcultyCombobox(listFacultys);
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

        private void cmbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            Faculty selectedFaculty = cmbFaculty.SelectedItem as Faculty;
            if (selectedFaculty != null)
            {
                // Hiển thị danh sách chuyên ngành trong khoa
                var listMajor = majorService.GetAllByFaculty(selectedFaculty.FacultyID);
                FillMajorCombobox(listMajor);

                // Hiển thị danh sách sinh viên chưa có chuyên ngành
                var listStudents = studentService.GetAllHasNoMajor(selectedFaculty.FacultyID);
                BindGrid(listStudents);
            }
        }

        private void FillMajorCombobox(List<Major> listMajors)
        {
            this.cmbMajor.DataSource = listMajors;
            this.cmbMajor.DisplayMember = "Name";
            this.cmbMajor.ValueMember = "MajorID";
        }

        private void BindGrid(List<Student> listStudent)
        {
            dgvStudent.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells["colMSSV"].Value = item.StudentID ?? "N/A";
                dgvStudent.Rows[index].Cells["colFullName"].Value = item.FullName ?? "N/A";
                dgvStudent.Rows[index].Cells["colFaculty"].Value = item.Faculty?.FacultyName ?? "N/A";
                dgvStudent.Rows[index].Cells["colGPA"].Value = item.AverageScore.ToString();
            }
        }



        private void cmbMajor_SelectedIndexChanged(object sender, EventArgs e)
        {
            Faculty selectedFaculty = cmbFaculty.SelectedItem as Faculty;
            if (selectedFaculty != null)
            {
                var listMajor = majorService.GetAllByFaculty(selectedFaculty.FacultyID);
                FillMajorCombobox(listMajor);
                var listStudents = studentService.GetAllHasNoMajor(selectedFaculty.FacultyID);
                BindGrid(listStudents);
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {

            try
            {
                int majorId = (cmbMajor.SelectedItem as Major)?.MajorID ?? 0; // Lấy MajorID
                if (majorId == 0)
                {
                    MessageBox.Show("Vui lòng chọn chuyên ngành trước khi đăng ký.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (DataGridViewRow row in dgvStudent.Rows)
                {
                    // Đảm bảo sử dụng đúng tên cột
                    bool isSelected = Convert.ToBoolean(row.Cells["colSelect"].Value);
                    if (isSelected)
                    {
                        string studentID = row.Cells["colMSSV"].Value.ToString(); // Sử dụng đúng tên cột
                        studentService.RegisterMajor(studentID, majorId); // Gọi service để đăng ký
                    }
                }

                // Reload lại danh sách sinh viên
                Faculty selectedFaculty = cmbFaculty.SelectedItem as Faculty;
                if (selectedFaculty != null)
                {
                    var listStudents = studentService.GetAllHasNoMajor(selectedFaculty.FacultyID);
                    BindGrid(listStudents);
                }

                MessageBox.Show("Đăng ký chuyên ngành thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}