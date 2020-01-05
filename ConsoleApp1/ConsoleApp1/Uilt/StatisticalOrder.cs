using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Uilt
{
    class StatisticalOrder
    { /// <summary>
      /// 正在使用的订单
      /// </summary>
        public int usingOrderCount { get; set; }
        /// <summary>
        /// 两天的订单总数
        /// </summary>
        public int todayAndMorningOrderCount { get; set; }
        /// <summary>
        /// 违约订单数
        /// </summary>
        public int ViolationOrderCount { get; set; }
        /// <summary>
        /// 预约订单数
        /// </summary>
        public int bookOrderCount { get; set; }
    }
}
