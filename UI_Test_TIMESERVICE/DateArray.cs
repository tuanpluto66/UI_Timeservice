using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI_Test_TIMESERVICE;

namespace UI_Test_TIMESERVICE
{
    static class DateArray
    {
        public static List<DateTime> GetDatesBetween(DateTime startDate, DateTime endDate)
        {
            List<DateTime> allDates = new List<DateTime>();
            if (startDate >= endDate)
            {
                return null;
            }
            else
            {
                try
                {
                    for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                    {

                        allDates.Add(date);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.Message);
                    LogHelper.Error(ex.StackTrace);
                    return null;
                }
                return allDates;
            }
        }
        public static IEnumerable<T1> Get_list_date_difference<T1, T2, T>(this IEnumerable<T1> list1, IEnumerable<T2> list2, Func<T1, T> ex1, Func<T2, T> ex2)
        {
            try
            {
                return (from item1 in list1
                        join item2 in list2 on ex1(item1) equals ex2(item2) into left
                        from itemLeft in left.DefaultIfEmpty()
                        select new { item1, itemLeft }).Where(o => Equals(o.itemLeft, default(T2))).Select(o => o.item1);
            }
            catch
            {
                return null;
            }
        }
        
    }
}
   

