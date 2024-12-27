using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BAI5
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text = "Main Form - Chọn Form";
            this.Size = new System.Drawing.Size(300, 200);

            // Button mở frmStudent
            Button btnStudent = new Button();
            btnStudent.Text = "Quản lý Sinh Viên";
            btnStudent.Size = new System.Drawing.Size(150, 40);
            btnStudent.Location = new System.Drawing.Point(75, 30);
            btnStudent.Click += (s, ev) =>
            {
                frmStudent studentForm = new frmStudent();
                studentForm.Show(); // Hiển thị frmStudent
            };
            this.Controls.Add(btnStudent);

            // Button mở frmRegister
            Button btnRegister = new Button();
            btnRegister.Text = "Đăng ký Chuyên ngành";
            btnRegister.Size = new System.Drawing.Size(150, 40);
            btnRegister.Location = new System.Drawing.Point(75, 90);
            btnRegister.Click += (s, ev) =>
            {
                frmRegister registerForm = new frmRegister();
                registerForm.Show(); // Hiển thị frmRegister
            };
            this.Controls.Add(btnRegister);
        }
    }
    
}
