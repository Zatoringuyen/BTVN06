using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAI5.DAL.Entities;


namespace BAI5.BUS
{
    public class FacultyService
    {
        public List<Faculty> GetAll()
        {
           Model1 context = new Model1();
            return context.Faculty.ToList();
        }
    }
}


