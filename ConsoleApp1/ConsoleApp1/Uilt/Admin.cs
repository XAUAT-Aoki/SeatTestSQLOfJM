using ConsoleApp1.ModelOfJM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp1.Uilt
{
    class Admin
    {
        #region 1.查询管理员密码 已完成
        /// <summary> 
        /// 
        /// 查询密码
        /// </summary>
        /// <param name="user">工号</param>
        /// <returns>密文密码</returns>
        public string GetPassword(string user)
        {
            using (var context = new LSSDataContext())
            {
                string str = null;
                Administor administor = new Administor();
                administor = context.Administor.FirstOrDefault(b => b.Aid == user);
                if (administor != null)
                {
                    str = administor.Apassword;
                }
                return str;
            }
        }
        #endregion
        #region 2.向管理员表中写入该管理员的新密码 已完成 
        /// <summary>
        /// 向管理员表中写入该管理员的新密码
        /// </summary>
        /// <param name="username">管理员名</param>
        /// <param name="newpassword">新密码</param>
        public bool ChangePassword(string userId, string newpassword)
        {
            //直接写密码（管理员表）
            bool flag = false;
            using (var dbContext = new LSSDataContext())
            {
                Administor adminstor = new Administor();
                //修改数据库信息最好有一些事务操作
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        adminstor = dbContext.Administor.Where(x => x.Aid == userId).FirstOrDefault();
                        if (adminstor != null)
                        {
                            adminstor.Apassword = newpassword;
                            dbContext.Administor.Update(adminstor);
                            dbContext.SaveChanges();
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
                return flag;
            }
        }
        #endregion
        #region 3.按座位号查询座位封装为对象 已完成
        /// <summary>
        /// 按座位号查询座位封装为对象
        /// </summary>
        /// <param name="seatid">座位号</param>
        /// <returns>返回座位对象</returns>

        public Seat ReferSeatById(string seatid)
        {
            //直接在表中进行座位查询
            Seat seat = new Seat();
            using (var dbContext = new LSSDataContext())
            {
                seat = dbContext.Seat.Where(x => x.Tid == seatid).FirstOrDefault();
            }
            return seat;
        }
        #endregion
        #region 4.查询所有的符合条件的座位 已完成 重新修改
        /// <summary>
        /// 查询所有的符合条件的座位
        /// </summary>
        /// <param name="libraryname"></param>
        /// <param name="floor"></param>
        /// <returns></returns>
        public List<Seat> ReferSeatViewModel(string libraryname, int floor)
        {
            using (var dbContext = new LSSDataContext())
            {
                Library li = new Library();
                List<Seat> seat = new List<Seat>();
                int lid = 0;
                li = dbContext.Library.Where(x => x.Lname == libraryname).FirstOrDefault();
                if (li != null)
                {
                    var seats = (from list in dbContext.Seat
                                 where list.Lid == li.Lid && list.Tfloor == floor
                                 select list).GetEnumerator();
                    while (seats.MoveNext()) seat.Add(seats.Current);
                }
                return seat;
            }
        }
        #endregion
        #region 5.设置座位状态  已完成 
        public bool SetSeatState(string id, string operation)
        {
            bool flag = false;
            using (var dbContext = new LSSDataContext())
            {
                Seat seat = new Seat();
                //修改数据库信息最好有一些事务操作
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        seat = dbContext.Seat.Where(x => x.Tid == id).FirstOrDefault();
                        if (seat != null)
                        {
                            seat.Tstate = operation;
                            dbContext.Seat.Update(seat);
                            dbContext.SaveChanges();
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
                return flag;
            }
        }
        #endregion
        #region 6.根据座位号查询目前未使用的订单对应的学号 已完成
        public List<string> GetSIdByOrder(string seatid)
        {
            //根据座位号查询目前未使用的订单对应的学号
            using (var dbContext = new LSSDataContext())
            {
                List<string> str = new List<string>();
                var stringList = (from list in dbContext.Order
                                  where list.Tid == seatid && list.Ostate == "01"
                                  select list.Sid).GetEnumerator();
                while (stringList.MoveNext()) str.Add(stringList.Current);
                return str;
            }
        }
        #endregion
        #region 7.修改座位状态 【operation 类型应该为string】已完成  传入为1或0,48为全为1或0
        /// <summary>
        /// 修改座位状态 【operation 类型应该为string】
        /// </summary>
        /// <param name="seatid">座位id</param>
        /// <param name="operation">目标值</param>
        public bool SeatInfor(string seatid, int operation)
        {
            //将座位所有时间的状态修改为operation
            Seat seat = new Seat();
            bool flag = false;
            using (var context = new LSSDataContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        seat = context.Seat.FirstOrDefault(x => x.Tid == seatid);
                        if (seat != null)
                        {
                            char[] a = new char[48];
                            for (int i = 0; i < 48; i++) a[i] = char.Parse(operation.ToString());
                            string str = new string(a);
                            seat.Tstate = str;
                            context.Seat.Update(seat);
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
        #region 8.根据订单id查询订单 已完成
        /// <summary>
        /// 根据订单id查询订单
        /// </summary>
        /// <param name="orderid">订单id</param>
        /// <returns>返回一个订单</returns>
        public Order GetOrderById(string orderid)
        {
            Order order = new Order();
            using (var context = new LSSDataContext())
            {
                order = context.Order.FirstOrDefault(x => x.Oid == orderid);
            }
            return order;
        }
        #endregion
        #region 9.修改该学生的积分 已完成
        /// <summary>
        /// 修改该学生的积分
        /// </summary>
        /// <param name="studentid">学生id</param>
        public bool AddGlory(string studentid)
        {
            //给该学生的信誉积分加1
            Student student = new Student();
            bool flag = false;
            using (var context = new LSSDataContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        student = context.Student.FirstOrDefault(x => x.Sid == studentid);
                        if (student != null)
                        {
                            student.Svalue += student.Svalue;
                            context.Student.Update(student);
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
        #region 10.获取所有的图书馆名称 已完成
        /// <summary>
        /// 获取所有的图书馆名称
        /// </summary>
        /// <returns>返回所有的图书馆名称集</returns>
        public List<string> GetAllLibraryName()
        {
            List<string> libraryname = new List<string>();
            Library library = new Library();
            using (var context = new LSSDataContext())
            {
                libraryname = (from l in context.Library
                               select l.Lname).ToList();
            }
            return libraryname;
        }
        #endregion
        #region 11.添加图书馆 已完成 
        /// <summary>
        /// 添加图书馆
        /// </summary>
        /// <param name="library">图书馆对象</param>
        public bool AddLibrary(Library library)
        {
            bool flag = false;
            using (var context = new LSSDataContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.Add(library);
                        context.SaveChanges();
                        transaction.Commit();
                        flag = true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                    return false;
                }
            }


            //将除id之外的字段添加进图书馆表中
        }
        #endregion
        #region 12.获取该楼层的起始和结束楼层 已完成
        /// <summary>
        /// 获取该楼层的起始和结束楼层
        /// </summary>
        /// <param name="libraryname">图书馆名称</param>
        /// <returns>返回list</returns>
        public List<int> GetFloor(string libraryname)
        {
            List<int> list = new List<int>();
            Library library = new Library();
            using (var context = new LSSDataContext())
            {
                library = context.Library.FirstOrDefault(x => x.Lname == libraryname);
            }
            if (library != null)
            {
                list.Add((int)library.Lsfloor);
                list.Add((int)library.Lefloor);
            }
            //将图书馆的起始楼层和结束楼层分别放在list的0、1空间
            return list;
        }
        #endregion
        #region 13.修改该图书馆的起始楼层和结束楼层 已完成
        /// <summary>
        /// 修改该图书馆的起始楼层和结束楼层
        /// </summary>
        /// <param name="libraryname">图书馆名称</param>
        /// <param name="startfloor">起始楼层</param>
        /// <param name="endfloor">结束楼层</param>
        public bool AddFloor(string libraryname, int startfloor, int endfloor)
        {
            //修改该图书馆的起始楼层和结束楼层
            Library library = new Library();
            bool flag = false;
            using (var context = new LSSDataContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        library = context.Library.FirstOrDefault(x => x.Lname == libraryname);
                        if (library != null)
                        {
                            library.Lsfloor = startfloor;
                            library.Lefloor = endfloor;
                            context.Library.Update(library);
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
        #region 14.空闲座位查询 根据图书馆名，图书馆楼层，开始时间，结束时间，以及日期(第几天)获取空闲座位 已完成
        public List<Seat> QueryAllEmptySeat(string libraryName, int floor, int startTime, int endTime, int data)
        {
            using (var dbContext = new LSSDataContext())
            {
                Library li = new Library();//准备接受
                List<Seat> seat = new List<Seat>();//准备接受
                //int starts = (int)DateTime.Now.Hour;
                //int start = (int)startTime.Hour;//提取开始的时间在座位状态中的位置
                //int end = (int)endTime.Hour;//提取结束的时间在座位状态中的位置
                li = dbContext.Library.Where(x => x.Lname == libraryName).FirstOrDefault();//根据图书馆名获取图书馆信息
                if (li != null)//该图书馆不为空
                {
                    var seats = (from list in dbContext.Seat
                                 where list.Lid == li.Lid && list.Tfloor == floor
                                 select list).GetEnumerator();//获取该图书馆某楼层的所有座位
                    char[] a = new char[48];//准备接受座位状态信息
                    if (data == 0)//第一天
                    {

                        while (seats.MoveNext())
                        {//遍历所有座位
                            a = seats.Current.Tstate.ToCharArray();
                            bool flag = true;//设置标志
                            for (int i = startTime; i < endTime; i++)
                            {
                                if (a[i] != '0')
                                    flag = false;//所选的时间段中有一个时间段被占用，则false。表示该座位不符合要求
                            }
                            if (flag)
                            {
                                seat.Add(seats.Current);
                            }
                        }
                    }
                    else
                    {
                        while (seats.MoveNext())
                        {//遍历所有座位
                            a = seats.Current.Tstate.ToCharArray();
                            bool flag = true;//设置标志
                            for (int i = 24 + startTime - 1; i < 24 + endTime - 1; i++)
                            {
                                if (a[i] != '0')
                                    flag = false;//所选的时间段中有一个时间段被占用，则false。表示该座位不符合要求
                            }
                            if (flag)
                            {
                                seat.Add(seats.Current);
                            }
                        }
                    }
                }
                return seat;
            }
        }
        #endregion
        #region 15.查询不可预定座位  根据图书馆名，图书馆楼层，开始时间，结束时间，以及第几天获取不可使用座位 已完成
        public List<Seat> QueryAllBookNotSeat(string libraryName, int floor, int startTime, int endTime, int data)
        {
            using (var dbContext = new LSSDataContext())
            {
                Library li = new Library();//准备接受
                List<Seat> seat = new List<Seat>();//准备接受
                //int start = (int)startTime.Hour;//提取开始的时间在座位状态中的位置
                //int end = (int)endTime.Hour;//提取结束的时间在座位状态中的位置
                li = dbContext.Library.Where(x => x.Lname == libraryName).FirstOrDefault();//根据图书馆名获取图书馆信息
                if (li != null)//该图书馆不为空
                {
                    var seats = (from list in dbContext.Seat
                                 where list.Lid == li.Lid && list.Tfloor == floor
                                 select list).GetEnumerator();//获取该图书馆某楼层的所有座位
                    char[] a = new char[48];//准备接受座位状态信息
                    if (data == 0)//第一天
                    {
                        while (seats.MoveNext())
                        {//遍历所有座位
                            a = seats.Current.Tstate.ToCharArray();
                            bool flag = false;//设置标志
                            for (int i = startTime; i < endTime; i++)
                            {
                                if (a[i] == '1')
                                    flag = false;//所选的时间段中有一个时间段被占用，则true。表示该座位不符合要求
                            }
                            if (flag)
                            {
                                seat.Add(seats.Current);
                            }
                        }
                    }
                    else
                    {
                        while (seats.MoveNext())
                        {//遍历所有座位
                            a = seats.Current.Tstate.ToCharArray();
                            bool flag = false;//设置标志
                            for (int i = 24 + startTime - 1; i < 24 + endTime - 1; i++)
                            {
                                if (a[i] == '1')
                                    flag = true;//所选的时间段中有一个时间段被占用，则true。表示该座位不符合要求
                            }
                            if (flag)
                            {
                                seat.Add(seats.Current);
                            }
                        }
                    }
                }
                return seat;
            }
        }
        #endregion
        #region 16.查询某图书馆，某楼层，当前时间的所有座位的使用状况 已完成
        /// <summary>
        /// 查询某图书馆，某楼层，当前时间的所有座位的使用状况
        /// </summary>
        /// <param name="libraryName">传入图书馆名</param>
        /// <param name="floor">传入楼层</param>
        ///  <param name="1">已预约或者使用中</param>
        ///  <param name="0">未预约</param>
        /// <returns></returns>
        public List<LibrarySeats> QueryAllLibrarySeats(string libraryName, int floor)
        {
            using (var dbContext = new LSSDataContext())
            {
                Library li = new Library();//准备接受
                List<LibrarySeats> librarySeats = new List<LibrarySeats>();//准备接受
                li = dbContext.Library.Where(x => x.Lname == libraryName).FirstOrDefault();//根据图书馆名获取图书馆信息
                if (li != null)//该图书馆不为空
                {
                    var seats = (from list in dbContext.Seat
                                 where list.Lid == li.Lid && list.Tfloor == floor
                                 select list).GetEnumerator();//获取该图书馆某楼层的所有座位
                    while (seats.MoveNext())
                    {//遍历所有座位
                        LibrarySeats librarySeat = new LibrarySeats();
                        string s = DiscriminationStatus(seats.Current.Tstate);//获得相应时间下的座位状态
                        librarySeat.Tstate = s;
                        librarySeat.libraryName = libraryName;
                        librarySeat.Tfloor = floor;
                        librarySeat.Tid = seats.Current.Tid;
                        librarySeats.Add(librarySeat);
                    }
                }
                return librarySeats;
            }
        }

        /// <summary>
        ///以当前时间为参考判断座位使用状态
        /// </summary>
        ///  <param name="1">已预约或者使用中</param>
        ///  <param name="0">未预约</param>
        ///  <param name="0">未预约</param>
        /// <returns>返回座位使用状态</returns>
        public string DiscriminationStatus(string flag)
        {
            int hour = DateTime.Now.Hour;
            char[] a = new char[48];//准备接受座位状态信息
            a = flag.ToCharArray();
            string discriminationStatus = new string(a[hour].ToString());
            return discriminationStatus;
        }
        #endregion
        #region 17.统计订单数据
        public StatisticalOrder StatisticalCount()
        {
            StatisticalOrder statisticalOrder = new StatisticalOrder();

            using (var dbContext = new LSSDataContext())
            {
                //获取正在使用的订单数。(开始时间<当前时间<结束时间)
                string nowTempTime1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                DateTime nowTime1 = Convert.ToDateTime(nowTempTime1);
                statisticalOrder.usingOrderCount = (from o in dbContext.Order
                                                    where o.Ostime < nowTime1 && o.Oetime >= nowTime1
                                                    select o).ToList().Count;
                //获取今明两天的订单总数
                string nowTempTime2 = DateTime.Now.Date.ToString("yyyy-MM-dd");
                DateTime nowTime2 = Convert.ToDateTime(nowTempTime2);
                statisticalOrder.todayAndMorningOrderCount = (from o in dbContext.Order
                                                              where nowTime2 <= o.Ostime
                                                              select o).ToList().Count;
                //获取违约订单总数
                statisticalOrder.ViolationOrderCount = dbContext.Order.Where(x => x.Ostate == "00").ToList().Count;
                //预约订单数(当前时间<开始时间)
                statisticalOrder.bookOrderCount = (from o in dbContext.Order
                                                   where nowTime1 < o.Ostime
                                                   select o).ToList().Count;
            }
            return statisticalOrder;
        }
        #endregion
    }

}
