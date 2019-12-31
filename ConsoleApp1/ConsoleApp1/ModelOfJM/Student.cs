using System;
using System.Collections.Generic;

namespace ConsoleApp1.ModelOfJM
{
    public partial class Student
    {
        public Student()
        {
            Order = new HashSet<Order>();
        }

        public string Sid { get; set; }
        public string Sname { get; set; }
        public string Spassword { get; set; }
        public string Semail { get; set; }
        public int? Ssex { get; set; }
        public int? Svalue { get; set; }
        public int? Slock { get; set; }

        public virtual ICollection<Order> Order { get; set; }
    }
}
