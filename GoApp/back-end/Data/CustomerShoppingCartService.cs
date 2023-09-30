using GoApp;
using GoApp.back_end.Data;
using GoApp.Db;
using Microsoft.EntityFrameworkCore;
using Timer = System.Timers.Timer;

namespace GoApp.back_end.Entitys
{
    public class CustomerShoppingCartService
    {
        private static CustomerShoppingCartService instance;
        SqlDbContext dbContext;
        private int HoursIntervalBetweenEvents = 4;
        static Timer timer;
        public CustomerShoppingCartService(SqlDbContext dbContext)
        {
            this.dbContext = dbContext;
            Timer = GetTimer();
        }

        public Timer GetTimer()
        {
            var timer = new System.Timers.Timer( 1*60*60 * 1000);
            timer.Elapsed += TimerEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
            return timer;
        }

        public void TimerEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            dbContext.CustomersShoppingCart.Where(e => e.LastCheck.AddHours(HoursIntervalBetweenEvents) < DateTime.Now).ExecuteDelete();
        }
    }
}
