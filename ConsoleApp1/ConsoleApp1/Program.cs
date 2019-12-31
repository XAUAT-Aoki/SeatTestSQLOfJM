using ConsoleApp1.ModelOfJM;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            //foreach (var item in students)
            {
                //student.Add(item);
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
                /*           var stu = (from st in context.Set<Student>()
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
        public bool updateLibrary(string lid, string name)
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
        #region 1.查询密码
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
            return "";
        }
        #endregion
        #region 2.修改密码
        /// <summary>
        /// 修改数据库中的对应密码
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public void ChangePassword(string user, string password)
        {

            //使用工具方法判断学号/邮箱IsStudentId（）
            //修改对应密码
        }
        #endregion
        #region 3.根据邮箱，姓名返回学生
        public Student StudentInformation(string username)
        {
            //使用工具方法判断学号/邮箱IsStudentId（）
            //使用username查询出学生对象；使用框架查询
            return new Student();
        }
        #endregion
        /*       #region 4.空闲座位查询
               /// <summary>
               /// 空闲座位查询
               /// </summary>
               /// <param name="condition"></param>
               /// <returns></returns>
               public List<Seat> Leisure(Condition condition)
               {
                   Condition StartCondition = LeisureDeal(condition);

                   //判断是否是当天的日期
                   //如果开始时间和结束时间为null，则查询开始时间不小于6点，结束时间不大于23点
                   //如果输入condition.TimeBulk是0，则使用“111111111111111”
                   //对condition.data进行日期判断

                   return new List<Seat>();
               }
               #endregion*/
        /* #region 5.不懂
         /// <summary>
         /// 处理含有空字符的数据
         /// </summary>
         /// <param name="condition">返回处理结果用于可直接拼装sql的对象</param>
         /// <returns>返回处理过的对象</returns>
         public Condition LeisureDeal(Condition condition)
         {

             //使用try{]catch(){}语句处理转换异常
             return new Condition();
         }
         #endregion*/
        #region 6.查询座位状态
        public string SeatState(int date, string seatid)
        {

            //查询状态字段，



            //判断是否是当天的状态

            //截取所需当天的24位字符并返回

            return "";
        }
        #endregion
        #region 7.修改个人的lock锁
        /// <summary>
        /// 修改个人的lock锁
        /// </summary>
        /// <param name="operation">状态修改的目标值</param>
        /// <param name="date">日期</param>
        /// <param name="username">座位ID</param>
        /// <param name="">lock字段</param>
        public void ModifyInfor(int operation, int date, string username, byte ad)
        {

        }
        #endregion
        #region 8.查询lock锁状态
        /// <summary>
        /// 查询lock字段，返回byte[0]
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public byte getLock(string username)
        {

            //调用正则匹配，进行查询lock字段

            return 1;
        }
        #endregion
        #region 9.修改座位状态
        /// <summary>
        /// 修改座位状态
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="str">座位状态信息</param>
        /// <param name="seatid">座位id</param>
        /// <param name="num">处理后需要修改的状态的开始坐标</param>
        /// <param name="duration">时长</param>
        /// <param name="operation">目标值</param>
        public void SeatInfor(int date, string str, string seatid, int num, int duration, int operation)
        {
            //将字符串修改之后直接插入座位表
        }
        #endregion
        #region 10.订单增加一条数据
        /// <summary>
        /// 在订单表中插入一条记录
        /// </summary>
        /// <param name="order"></param>
        public void SetOrder(Order order)
        {
            //插入一条订单记录到订单表（id,学号,座位号，订单开始时间，结束时间，打卡时间，订单状态）
            //订单id（图书馆号+层号+座位号+系统时间）
        }
        #endregion
        #region 11.根据Seat的ID获取Seat对象
        /// <summary>
        /// 根据seatid获取该seat对象
        /// </summary>
        /// <param name="seatid"></param>
        /// <returns>一个seat对象</returns>
        public Seat GetSeat(string seatid)
        {
            return new Seat();
        }
        #endregion
        #region 12.获取所有正在使用的订单
        #region 
        //获取所有正在使用的订单
        public List<Order> GetUsingOrder()
        {
            return new List<Order>();
        }
        #endregion
        #endregion
        #region 13.根据订单改变订单状态
        /// <summary>
        /// 根据ID直接改变订单状态
        /// </summary>
        /// <param name="oid">订单ID</param>
        public void ChangeOrderState(object oid)
        {
            //根据ID直接改变订单状态
        }
        #endregion
        #region 14. 返回打卡时间是否在指定时间范围内
        /// <summary>
        /// 返回打卡时间是否在指定时间范围内
        /// </summary>
        /// <param name="now">实际打卡时间</param>
        /// <param name="predict">预计打卡时间</param>
        /// <returns></returns>
        /// 
        public bool IsTrueTime(DateTime now, DateTime predict)
        {
            //实际打卡时间必须在预定打卡时间开始后半小时之内
            return true;
        }
        #endregion
        #region 15.根据座位 ID 在座位表内查询对应的图书馆号
        /// <summary>
        /// 根据座位 ID 在座位表内查询对应的图书馆号
        /// </summary>
        /// <param name="seatId">座位 ID</param>
        /// <returns>返回图书馆 ID</returns>
        public string GetLIdBySId(string seatId)
        {
            return "";
        }
        #endregion
        #region 16.根据图书馆 ID 返回一个图书馆对象
        /// <summary>
        /// 根据图书馆 ID 返回一个图书馆对象
        /// </summary>
        /// <param name="libraryId">图书馆 ID</param>
        /// <returns>图书馆对象</returns>
        public Library GetLibraryById(string libraryId)
        {
            return new Library();
        }
        #endregion
        #endregion
        static void Main(string[] args)
        {
            Student student = new Student();
            List<Student> students = new List<Student>();
            Program program = new Program();
            #region 初始测试
            //0.
            /*      program.QueryStudentSql("1606020002", "1606020002");
                  Console.WriteLine(student.Sname);*/
            //1.
            /* student= program.QueryStudentByPassword("1606020002", "1606020002");
              Console.WriteLine(student.Sname);*/
            //3.

            //2.
            /* program.QueryStudentsByPassword("16");*/
            /*  students = program.AllStudent();*/
            //4.
            /*         students = program.QueryStudents();
                     foreach (var item in students)
                     {
                         Console.WriteLine(item.Sid);
                     }*/

            //5.
            /*  program.UpdataStudentFen("1606020002");*/
            /*6.*/
            /*program.deleteSb("1606020097");
            program.updateLibrary("L3", "厕所图书馆");*/
            #endregion
            #region 正式测试
            #region 1.查询密码
            #endregion
            #region 2.修改密码
            #endregion
            #region 3.根据邮箱，姓名返回学生
            #endregion
            #region 4.空闲座位查询
            #endregion
            #region 5.不懂
            #endregion
            #region 6.查询座位状态

            #endregion
            #region 7.修改个人的lock锁

            #endregion
            #region 8.查询lock锁状态

            #endregion
            #region 9.修改座位状态

            #endregion
            #region 10.订单增加一条数据

            #endregion
            #region 11.根据Seat的ID获取Seat对象

            #endregion
            #region 12.获取所有正在使用的订单
            #endregion
            #region 13.根据订单改变订单状态

            #endregion
            #region 14. 返回打卡时间是否在指定时间范围内

            #endregion
            #region 15.根据座位 ID 在座位表内查询对应的图书馆号

            #endregion
            #region 16.根据图书馆 ID 返回一个图书馆对象

            #endregion
            #endregion
        }
    }
}
