using ConsoleApp1.ModelOfJM;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsoleApp1.Uilt;
namespace ConsoleApp1
{
    class Program
    {
        #region 0.原生sql 有问题
        public Student QueryStudentSql(string sid, string password)
        {
            Student student = new Student();
            using (var context = new LSSDataContext())
            {
                student = context.Student.FromSqlRaw("SELECT * FROM LSSData.Student")
    .ToList().FirstOrDefault();
            }
            return student;
        }

        #endregion
        #region 1.根据ID密码查找
        public Student QueryStudentByPassword(string sid, string password)
        {
            Student student = new Student();
            using (var context = new LSSDataContext())
            {
                //student = context.Student
                // .Single(b => b.Sid == sid);
                student = (from stu in context.Student
                           where stu.Sid == sid && stu.Spassword == password
                           select stu).ToList().FirstOrDefault();
            }
            return student;
        }
        #endregion
        #region 2.某个字段包含什么查找
        public List<Student> QueryStudentsByPassword(string sid)
        {
            List<Student> student;
            using (var context = new LSSDataContext())
            {
                student = context.Student.Where(b => b.Sid.Contains(sid)).ToList();
                //var student = context.Student.Where(p => p.Sid.Contains("16")).ToList().GetEnumerator();
                //while (student.MoveNext()) students.Add(student.Current);
            }
            return student;
        }
        #endregion
        #region 3.异步查找所有学生,没实现
        public async Task<List<Student>> AllStudent()
        {
            List<Student> student = new List<Student>();
            using (var context = new LSSDataContext())
            {
                var students = await context.Student.ToListAsync();
            }
            return student;
        }
        #endregion
        #region 4. 升序排序查询 
        public List<Student> QueryStudents()
        {
            List<Student> students = new List<Student>();
            using (var context = new LSSDataContext())
            {
                var stu = (from st in context.Set<Student>()
                           orderby st.Sid ascending
                           select st).GetEnumerator();
                while (stu.MoveNext()) students.Add(stu.Current);
                return students;
                /*        var stu = (from st in context.Set<Student>()
                                   orderby st.Sid ascending
                                   select st).Cast<object>().ToList();//装箱
                        students = stu.Cast<Student>.ToList();//拆箱
                        return stu;*/

            }
        }


        #endregion
        #region 5.修改数据 找不到表
        public bool UpdataStudentFen(string sid)
        {

            using (var dbContext = new LSSDataContext())
            {
                bool flag = false;
                //修改数据库信息最好有一些事务操作
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var sql = @"Update Student SET Svalue =  {0} WHERE Sid =  {1}";
                        dbContext.Database.ExecuteSqlCommand(sql, 2, sid);
                        dbContext.SaveChanges();
                        transaction.Commit();
                        flag = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        transaction.Rollback();
                    }
                    return flag;
                }
            }
        }

        #endregion
        #region 删除某数据
        public bool deleteSb(string sid)
        {
            bool flag = false;
            using (var dbContext = new LSSDataContext())
            {
                //修改数据库信息最好有一些事务操作
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var student = dbContext.Student.Where(x => x.Sid == sid).ToList().First();
                        dbContext.Student.Remove(student);
                        dbContext.SaveChanges();
                        transaction.Commit();
                        flag = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        transaction.Rollback();
                    }
                    return flag;
                }
            }
        }
        #endregion
        #region 6.更新某字段
        public bool updateLibrary(int lid, string name)
        {
            bool flag = false;
            using (var dbContext = new LSSDataContext())
            {
                //修改数据库信息最好有一些事务操作
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var library = dbContext.Library.Where(x => x.Lid == lid).ToList().First();
                        library.Lname = name;
                        dbContext.Library.Update(library);
                        dbContext.SaveChanges();
                        transaction.Commit();
                        flag = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        transaction.Rollback();
                    }
                    return flag;
                }
            }
        }
        #endregion

        #region 正式测试
        #region 1.查询密码  已完成
        /// <summary>
        /// 
        /// 查询密码
        /// </summary>
        /// <param name="user">邮箱或学号</param>
        /// <returns>密文密码</returns>
        public string GetPassword(string user)
        {
            //正则匹配（区分邮箱和学号）IsStudentId（）
            //根据对应的数据查询密码
            string stu = null;
            Student student = new Student();
            using (var context = new LSSDataContext())
            {
                Help h = new Help();
                if (h.IsStudentId(user)) student = context.Student.Single(b => b.Sid == user);
                else student = context.Student.Single(b => b.Semail == user);
                if (student != null)
                    stu = student.Spassword;
            }
            return stu;
        }
        #endregion
        #region 2.修改密码 这里修改了返回类型 已完成
        /// <summary>
        /// 修改数据库中的对应密码
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public bool ChangePassword(string user, string password)
        {
            //使用工具方法判断学号/邮箱IsStudentId（）
            //修改对应密码
            Student student = null;
            bool flag = false;
            Help h = new Help();
            using (var dbContext = new LSSDataContext())
            {
                //修改数据库信息最好有一些事务操作
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {//学号
                        if (h.IsStudentId(user)) student = dbContext.Student.Where(x => x.Sid == user).ToList().First();
                        else student = dbContext.Student.Where(x => x.Semail == user).ToList().First();
                        if (student != null)
                        {
                            student.Spassword = password;
                            dbContext.Student.Update(student);
                            dbContext.SaveChanges();
                            transaction.Commit();
                            flag = true;
                        }
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        return false;
                    }
                    return flag;
                }
            }
        }
        #endregion
        #region 3.根据邮箱或学号，姓名返回学生 已完成
        public Student StudentInformation(string username)
        {
            //使用工具方法判断学号/邮箱IsStudentId（）
            //使用username查询出学生对象；使用框架查询
            Student student = null;
            using (var dbContext = new LSSDataContext())
            {
                Help h = new Help();
                if (h.IsStudentId(username)) student = dbContext.Student.Where(x => x.Sid == username).ToList().First();
                else student = dbContext.Student.Where(x => x.Semail == username).ToList().First();
            }
            return student;
        }
        #endregion
        #region 6.查询座位状态 已完成
        public string SeatState(int date, string seatid)
        {
            using (var context = new LSSDataContext())
            {
                //查询状态字段，
                string str = null;
                string str3 = null;
                Seat seat = new Seat();
                seat = context.Seat.FirstOrDefault(x => x.Tid == seatid);
                if (seat != null)
                {
                    str = seat.Tstate;
                    char[] str2 = new char[24];
                    if (date == 0)
                    {
                        str2 = str.Substring(0, 24).ToCharArray();
                    }
                    else
                    {
                        str2 = str.Substring(24, 24).ToCharArray();
                    }
                    str3 = new string(str2);
                }
                //判断是否是当天的状态
                return str3;
            }
        }
        #endregion
        #region 7.修改个人的lock锁 已完成 修改了传入参数operation为char类型，去掉ad(原来获取的锁的状态）
        /// <summary>
        /// 修改个人的lock锁
        /// </summary>
        /// <param name="operation">状态修改的目标值这里为char类型，只传一个字符就可以例如‘1’</param>
        /// <param name="date">日期</param>
        /// <param name="username">个人id</param>
        public bool ModifyInfor(char operation, int date, string username)
        {
            Student student = new Student();
            bool flag = false;
            Help h = new Help();
            using (var context = new LSSDataContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (h.IsStudentId(username))
                            student = context.Student.FirstOrDefault(x => x.Sid == username);
                        else
                            student = context.Student.FirstOrDefault(x => x.Semail == username);
                        if (student != null)
                        {
                            char[] old = { student.Slock.ElementAt(0), student.Slock.ElementAt(1) };
                            if (date == 0) old[0] = operation;//修改第一天的状态
                            else old[1] = operation;//修改第二天的状态
                            string str = new string(old);
                            student.Slock = str;
                            context.Update(student);
                            context.SaveChanges();
                            transaction.Commit();
                            flag = true;
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                    return flag;
                }
            }
        }
        #endregion
        #region 8.查询lock锁状态     已完成
        /// <summary>
        /// 查询lock字段，返回byte[0]
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string getLock(string username)
        {
            using (var dbContext = new LSSDataContext())
            {
                Student state = new Student();
                Help h = new Help();
                string ss = null;
                if (h.IsStudentId(username))
                    state = dbContext.Student.Where(x => x.Sid == username).FirstOrDefault();
                else
                    state = dbContext.Student.Where(x => x.Semail == username).FirstOrDefault();
                if (state != null)
                {
                    ss = state.Slock;
                }
                return ss;
            }
            //调用正则匹配，进行查询lock字段
        }
        #endregion
        #region 9.修改座位状态【函数 返回值 有差异】 返回类型给为bool  已完成
        /// <summary>
        /// 修改座位状态
        /// </summary>
        /// <param name="date">第几天</param>
        /// <param name="str">座位状态信息</param>
        /// <param name="seatid">座位id</param>
        /// <param name="num">处理后需要修改的状态的开始坐标</param>
        /// <param name="duration">时长</param>
        /// <param name="operation">目标值</param>
        public bool SeatInfor(int date, string seatid, int num, int duration, char operation)
        {
            Seat seat = new Seat();
            bool flag = false;
            using (var context = new LSSDataContext())
            {
                seat = context.Seat.FirstOrDefault(x => x.Tid == seatid);
                if (seat != null)
                {
                    char[] c = new char[48];
                    c = seat.Tstate.ToCharArray();
                    if (date == 1)
                    {
                        for (int i = 23 + num; i < 23 + num + duration; i++)
                            c[i] = operation;
                    }
                    else
                    {
                        for (int i = num; i < num + duration; i++)
                            c[i] = operation;
                    }
                    string s = new string(c);
                    seat.Tstate = s;
                    context.Seat.Update(seat);
                    context.SaveChanges();
                    flag = true;
                }
            }
            return flag;
            //将字符串修改之后直接插入座位表
        }
        #endregion
        #region 10.订单增加一条数据【已完成】
        /// <summary>
        /// 在订单表中插入一条记录
        /// </summary>
        /// <param name="order"></param>
        public bool SetOrder(Order order)
        {
            //插入一条订单记录到订单表（id,学号,座位号，订单开始时间，结束时间，打卡时间，订单状态）
            //订单id（图书馆号+层号+座位号+系统时间）
            bool flag = false;
            using (var context = new LSSDataContext())
            {
                context.Order.Add(order);
                context.SaveChanges();
                flag = true;
            }
            return flag;
        }
        #endregion
        #region 11.根据Seat的ID获取Seat对象 已完成
        /// <summary>
        /// 根据seatid获取该seat对象
        /// </summary>
        /// <param name="seatid"></param>
        /// <returns>一个seat对象</returns>
        public Seat GetSeat(string seatid)
        {
            Seat seat = new Seat();
            using (var context = new LSSDataContext())
            {
                seat = (from s in context.Seat
                        where s.Tid == seatid
                        select s).ToList().FirstOrDefault();
            }
            return seat;
        }
        #endregion
        #region 12.获取所有正在使用的订单 已完成
        //获取所有正在使用的订单
        public List<Order> GetUsingOrder()
        {
            using (var dbContext = new LSSDataContext())
            {
                List<Order> Olist = new List<Order>();
                DateTime dtToday = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                var queryable = (from list in dbContext.Order
                                 where dtToday >= list.Ostime && dtToday <= list.Oetime
                                 select list).GetEnumerator();
                while (queryable.MoveNext()) Olist.Add(queryable.Current);
                return Olist;
            }
        }
        #endregion
        #region 13.根据订单改变订单状态【已完成】，修改传入参数传入状态为string,修改传入id为string，原来为object。修改返回类型为bool
        /// <summary>
        /// 根据ID直接改变订单状态
        /// </summary>
        /// <param name="oid">订单ID</param>
        public bool ChangeOrderState(string oid, string operation)
        {
            //根据ID直接改变订单状态
            bool flag = false;
            using (var context = new LSSDataContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var order = context.Order.Where(x => x.Oid == oid).FirstOrDefault();
                        if (order != null)
                        {
                            order.Ostate = operation;
                            context.Order.Update(order);
                            context.SaveChanges();
                            transaction.Commit();
                            flag = true;
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
            return flag;
        }
        #endregion
        #region 15.根据座位 ID 在座位表内查询对应的图书馆号 已完成
        /// <summary>
        /// 根据座位 ID 在座位表内查询对应的图书馆号
        /// </summary>
        /// <param name="seatId">座位 ID</param>
        /// <returns>返回图书馆 ID</returns>
        public int GetLIdBySId(string seatId)
        {
            Seat seat = new Seat();
            int ss = 0;
            using (var context = new LSSDataContext())
            {
                seat = (from s in context.Seat
                        where s.Tid == seatId
                        select s).ToList().FirstOrDefault();
                if (seat != null)
                {
                    ss = seat.Lid;
                }
            }
            return ss;
        }
        #endregion
        #region 16.根据图书馆 ID 返回一个图书馆对象   已完成
        /// <summary>
        /// 根据图书馆 ID 返回一个图书馆对象
        /// </summary>
        /// <param name="libraryId">图书馆 ID</param>
        /// <returns>图书馆对象</returns>
        public Library GetLibraryById(int libraryId)
        {
            Library library = new Library();
            using (var context = new LSSDataContext())
            {
                library = (from s in context.Library
                           where s.Lid == libraryId
                           select s).ToList().FirstOrDefault();
            }
            return library;
        }
        #endregion
        #region 17.根据id查询订单 已完成
        /// <summary>
        /// 根据id查询订单
        /// </summary>
        /// <param name="orderid">订单id</param>
        /// <returns>返回订单对象</returns>
        public Order GetOrder(string orderid)
        {
            //根据id查询一个订单记录，并返回Order对象
            using (var context = new LSSDataContext())
            {
                Order order = new Order();
                order = context.Order.FirstOrDefault(x => x.Oid == orderid);
                return order;
            }
        }
        #endregion
        #region 18.修改学生邮箱 已完成 
        /// <summary>
        /// 修改学生邮箱
        /// </summary>
        /// <param name="username"></param>
        /// <param name="newemail"></param>
        /// <returns></returns>
        public bool ChangeEmail(string username, string newemail)
        {
            Help h = new Help();
            bool flags = false;
            //正则表达匹配用户名，学号true
            bool flag = h.IsStudentId(username);
            Student s = new Student();
            using (var context = new LSSDataContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (!flag)
                        {
                            s = context.Student.FirstOrDefault(x => x.Sid == username);
                            if (s != null)
                            {
                                s.Semail = newemail;
                                context.Student.Update(s);
                                context.SaveChanges();
                                transaction.Commit();
                                flags = true;
                            }
                            //修改对应邮箱，返回true
                        }
                        else
                        {
                            s = context.Student.FirstOrDefault(x => x.Semail == username);
                            if (s != null)
                            {
                                s.Semail = newemail;
                                context.Student.Update(s);
                                context.SaveChanges();
                                transaction.Commit();
                                flags = true;
                            }
                            //修改邮箱，返回false
                        }
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        return false;
                    }
                    return flags;
                }
            }

        }
        #endregion
        #region 18.5修改学生的积分
        /// <summary>
        /// 修改学生的积分
        /// </summary>
        /// <param name="a">目标值</param>
        public bool ResetGlory(int a)
        {
            //将所有学生的积分值都修改为a
            List<Student> student = new List<Student>();
            using (var context = new LSSDataContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        student = (from s in context.Student
                                   select s
                                     ).ToList();
                        for (int i = 0; i < student.LongCount(); i++)
                        {
                            student[i].Svalue = a;
                            context.Student.Update(student[i]);
                        }
                        context.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion
        #region 19.根据学号获取学生的信誉积分 已完成
        /// <summary>
        /// 根据学号获取学生的信誉积分
        /// </summary>
        /// <param name="stuId">学号</param>
        /// <returns>信誉积分</returns>
        public int GetGlory(string stuId)
        {
            using (var context = new LSSDataContext())
            {
                int level = 0;
                Student student = new Student();
                student = context.Student.FirstOrDefault(x => x.Sid == stuId);
                if (student != null)
                    level = (int)student.Svalue;
                return level;
            }
        }
        #endregion
        #region 20.根据学生学号以及传入的代表第二天的参数 data（1），获取第二天的订单 已完成
        /// <summary>
        /// 根据学生学号以及传入的代表第二天的参数 data（1），获取第二天的订单
        /// </summary>
        /// <param name="stuId">学号</param>
        /// <param name="data">代表第二天的参数</param>
        /// <returns>第二天的订单（如果不存在，返回 null）</returns>
        public Order GetSecondOrder(string stuId, int data = 1)
        {
            //查询并封装为order对象
            string s2 = DateTime.Now.AddDays(1).Date.ToString("yyyy-MM-dd") + " 00:00:00";
            DateTime day2 = Convert.ToDateTime(s2);
            string s1 = DateTime.Now.Date.ToString("yyyy-MM-dd") + " 00:00:00";
            DateTime day1 = Convert.ToDateTime(s1);
            List<Order> orders = new List<Order>();
            using (var dbContext = new LSSDataContext())
            {
                if (data == 0)
                {
                    var order = (from o in dbContext.Order
                                 where o.Sid == stuId && day1 <= o.Ostime && o.Oetime <= day2
                                 select o).GetEnumerator();
                    while (order.MoveNext()) orders.Add(order.Current);
                }
                else
                {
                    var order = (from o in dbContext.Order
                                 where o.Sid == stuId && day2 <= o.Ostime
                                 select o).GetEnumerator();
                    while (order.MoveNext()) orders.Add(order.Current);
                }
                return orders.Last();
            }
        }
        #endregion
        #region 21.修改下次打卡时间 已完成
        /// <summary>
        /// 修改下次打卡时间
        /// </summary>
        /// <param name="oid">订单id</param>
        /// <param name="v">打卡时间的string类型</param>
        public bool ChangeClockTime(string oid, string v)
        {
            bool flag = false;
            using (var dbContext = new LSSDataContext())
            {
                Order order = new Order();
                order = dbContext.Order.FirstOrDefault(x => x.Oid == oid);
                if (order != null)
                {
                    DateTime ka2 = Convert.ToDateTime(v);
                    order.Octime = ka2;
                    dbContext.Update(order);
                    flag = true;
                }
            }
            return flag;
        }
        #endregion
        #region 22.根据用户名查询该用户目前正在使用的订单 已完成
        /// <summary>
        /// 根据用户名查询该用户目前正在使用的订单，（判断依据：系统时间大于开始时间小于下次打卡时间（后延半小时的误差））
        /// </summary>
        /// <param name="studentid"></param>
        /// <returns></returns>
        public List<Order> GetUsingOrderBySid(string studentid)
        {
            using (var dbContext = new LSSDataContext())
            {
                List<Order> Olist = new List<Order>();
                DateTime dtToday = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                var queryable = (from list in dbContext.Order
                                 where dtToday >= list.Ostime && dtToday <= list.Oetime && list.Sid == studentid
                                 select list).GetEnumerator();
                while (queryable.MoveNext()) Olist.Add(queryable.Current);
                return Olist;
            }
        }
        #endregion
        #region 23.修改学生得信誉积分，每次修改减1 已完成
        /// <summary>
        /// 修改学生得信誉积分，每次修改减1
        /// </summary>
        /// <param name="sid">学生学号</param>
        /// <returns></returns>
        public bool DuctGlory(string sid)
        {
            bool flag = false;
            using (var dbContext = new LSSDataContext())
            {
                Student student = new Student();
                student = dbContext.Student.FirstOrDefault(x => x.Sid == sid);
                if (student != null)
                {
                    student.Svalue = student.Svalue + 1;
                    dbContext.Update(student);
                    dbContext.SaveChanges();
                    flag = true;
                }
            }
            return flag;
        }
        #endregion
        #region 24.根据学号查询该学生的email 已完成
        /// <summary>
        /// 根据学号查询该学生的email
        /// </summary>
        /// <param name="id">学号</param>
        /// <returns>返回email</returns>
        public string getEmailById(string id)
        {
            string str = null;
            using (var dbContext = new LSSDataContext())
            {
                Student student = dbContext.Student.FirstOrDefault(x => x.Sid == id);
                if (student != null)
                {
                    str = student.Semail;
                }
            }
            return str;
        }
        #endregion

        #endregion

        static void Main(string[] args)
        {
            Student student = new Student();
            List<Student> students = new List<Student>();
            Program program = new Program();
            Admin admin = new Admin();
            #region 初始测试
            //          //0.
            ///*          program.QueryStudentSql("1606020002", "1606020002");
            //          Console.WriteLine(student.Sname);*/
            //          //1.
            //     /*     student = program.QueryStudentByPassword("1606020002", "1606020002");
            //          Console.WriteLine(student.Sname);*/
            //          //3.

            //          //2.
            //          program.QueryStudentsByPassword("16");
            //          //students = program.AllStudent();
            //          //4.
            //     /*     students = program.QueryStudents();
            //          foreach (var item in students)
            //          {
            //              Console.WriteLine(item.Sid);
            //          }*/

            //          //5.
            //          //program.UpdataStudentFen("1606020002");
            //          6.
            //          /* program.deleteSb("1606020097");
            //           program.updateLibrary(3, "厕所图书馆");*/
            #endregion




            #region 1.学生正式测试
            #region 1.查询密码 已完成
            //program.GetPassword("1606020002");
            #endregion
            #region 2.修改密码 已完成
            //program.ChangePassword("1606020002", "1606020002");
            #endregion
            #region 3.根据邮箱，姓名返回学生 已完成
            //program.StudentInformation("1606020002");
            #endregion
            #region 4.空闲座位查询
            #endregion
            #region 5.不懂
            #endregion
            #region 6.查询座位状态
            /*   program.SeatState(1, "T003");*/
            #endregion
            #region 7.修改个人的lock锁

            /*    program.ModifyInfor('1', 0, "1606020002");*/
            #endregion
            #region 8.查询lock锁状态
            //program.getLock("1606020002");
            #endregion

            #region 9.修改座位状态
            /*program.SeatInfor(1, "T003", 8, 4, '0');*/
            #endregion
            #region 10.订单增加一条数据
            /*       Order order = new Order();
                   order.Oid ="O10011";
                   order.Ostate = "使用";
                   order.Sid = "1606020002";
                   order.Tid = "T003";
                   program.SetOrder(order);*/
            #endregion
            #region 11.根据Seat的ID获取Seat对象

            #endregion
            #region 12.获取所有正在使用的订单 已完成
            /*program.GetUsingOrder();*/
            #endregion
            #region 13.根据订单改变订单状态
            /*  program.ChangeOrderState("O10008","已用");*/
            #endregion
            #region 14. 返回打卡时间是否在指定时间范围内

            #endregion
            #region 15.根据座位 ID 在座位表内查询对应的图书馆号

            #endregion
            #region 16.根据图书馆 ID 返回一个图书馆对象

            #endregion
            #region 18.
            //program.ChangeEmail("168935.@qq.com", "923749851@qq.com");
            /*    program.ResetGlory(9);*/
            #endregion
            #region 17.根据id查询订单
            /*            program.GetOrder("O1004");*/
            #endregion
            #region 19.根据学号获取学生的信誉积分
            /*  program.GetGlory("1606020002");*/
            #endregion
            #region 20. 根据学生学号以及传入的代表第二天的参数 data（1），获取第二天的订单
            /*            program.GetSecondOrder("1606020003");
                        program.GetSecondOrder("1606020003", 0);*/
            #endregion

            #region 
            /*program.DuctGlory("1606020002");*/

            /* program.getEmailById("1606020098");*/
            #endregion

            #endregion

            #region 2.管理员正式测试
            #region 1.获取管理员密码
            /*  admin.GetPassword("16001");*/

            #endregion
            #region 2.向管理员表中写入该管理员的新密码 已完成 修改返回类型为bool，传入参数为管理员ID

            //admin.ChangePassword("16001", "16001");
            #endregion
            #region 3.按座位号查询座位封装为对象 已完成
            //admin.ReferSeatById("T001");
            #endregion
            #region 4.查询所有的符合条件的座位 已完成 输入的两个参数这里处理的是且的关系
            /*   admin.ReferSeatViewModel("学府城", 2);*/
            #endregion
            #region 5.设置座位状态  已完成 修改返回类型为bool，传入的seat状态应为string类型
            /*admin.SetSeatState("T001", "1");*/
            #endregion
            #region 6.根据座位号查询目前未使用的订单对应的学号 已完成
            //admin.GetSIdByOrder("T004");
            #endregion
            #region 7.与5函数一样
            /* admin.SeatInfor("T003", "100001100101100101000101000010101000101010101000");*/
            //admin.GetAllLibraryName();
            /* admin.SeatInfor("T003", 1);*/
            #endregion
            /*  Console.WriteLine(admin.QueryAllEmptySeat("南山", 2, 1, 2, 0).Count());
              Console.WriteLine(admin.QueryAllEmptySeat("南山", 2, 1, 3, 0).Count());
              Console.WriteLine(admin.QueryAllEmptySeat("南山", 2, 1, 4, 0).Count());
              Console.WriteLine(admin.QueryAllEmptySeat("南山", 2, 1, 5, 0).Count());

              Console.WriteLine(admin.QueryAllEmptySeat("南山", 2, 8, 12, 1).Count());
              Console.WriteLine(admin.QueryAllEmptySeat("南山", 2, 8, 11, 1).Count());
              Console.WriteLine(admin.QueryAllEmptySeat("南山", 2, 8, 10, 1).Count());
              Console.WriteLine(admin.QueryAllEmptySeat("南山", 2, 8, 9, 1).Count());
           *//*   Console.WriteLine(admin.QueryAllEmptySeat("南山", 2, 8, 13, 1).Count());*//*
            Console.WriteLine(admin.QueryAllBookNotSeat("南山", 2, 1, 2, 0).Count());
            Console.WriteLine(admin.QueryAllBookNotSeat("南山", 2, 1, 3, 0).Count());
            Console.WriteLine(admin.QueryAllBookNotSeat("南山", 2, 1, 4, 0).Count());
            Console.WriteLine(admin.QueryAllBookNotSeat("南山", 2, 1, 5, 0).Count());

            Console.WriteLine(admin.QueryAllBookNotSeat("南山", 2, 8, 12, 1).Count());
            Console.WriteLine(admin.QueryAllBookNotSeat("南山", 2, 8, 11, 1).Count());
            Console.WriteLine(admin.QueryAllBookNotSeat("南山", 2, 8, 10, 1).Count());
            Console.WriteLine(admin.QueryAllBookNotSeat("南山", 2, 8, 9, 1).Count());
            Console.WriteLine(admin.QueryAllBookNotSeat("南山", 2, 8, 13, 1).Count());*/
            #endregion
            //admin.QueryAllLibrarySeats("南山", 2);
            admin.StatisticalCount();
        }
    }
}
