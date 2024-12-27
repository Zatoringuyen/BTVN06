using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAI5.DAL.Entities;


namespace BAI5.BUS
{
    public class StudentService
    {
        public List<Student> GetAll()
        {
            Model1 context = new Model1();
            return context.Student.ToList();
        }

        public List<Student> GetAllHasNoMajor()
        {
            Model1 context = new Model1();
            return context.Student.Where(p => p.MajorID == null).ToList();
        }

        public List<Student> GetAllHasNoMajor(int facultyID)
        {
            Model1 context = new Model1();
            return context.Student.Where(p => p.MajorID == null && p.FacultyID == facultyID).ToList();
        }

        public Student FindById(string studentId)
        {
            Model1 context = new Model1();
            return context.Student.FirstOrDefault(p => p.StudentID == studentId);
        }

        public void InsertUpdate(Student s)
        {
            Model1 context = new Model1();
            context.Student.AddOrUpdate(s);
            context.SaveChanges();
        }
        public void RegisterMajor(string studentId, int majorId)
        {
            using (var context = new Model1())
            {
                var student = context.Student.FirstOrDefault(s => s.StudentID == studentId);
                if (student != null)
                {
                    student.MajorID = majorId; // Cập nhật MajorID
                    context.SaveChanges(); // Lưu thay đổi vào database
                }
            }
        }


        public void AddOrUpdate(Student student)
        {
            using (var context = new Model1())
            {
                var existingStudent = context.Student.FirstOrDefault(s => s.StudentID == student.StudentID);

                if (existingStudent != null)
                {
                    // Cập nhật thông tin sinh viên
                    existingStudent.FullName = student.FullName;
                    existingStudent.FacultyID = student.FacultyID;
                    existingStudent.AverageScore = student.AverageScore;
                    existingStudent.Avatar = student.Avatar;
                }
                else
                {
                    // Thêm sinh viên mới
                    context.Student.Add(student);
                }

                context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            }
        }
    }

}
