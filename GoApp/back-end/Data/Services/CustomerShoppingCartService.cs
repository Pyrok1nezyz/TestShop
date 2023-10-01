using BootstrapBlazor.Components;
using GoApp;
using GoApp.back_end.Data;
using GoApp.Db;
using Microsoft.EntityFrameworkCore;
using Timer = System.Timers.Timer;

namespace TestShop.Entitys
{
	public class CustomerShoppingCartService
    {
        private static CustomerShoppingCartService instance;
        SqlDbContext dbContext;
        private int HoursIntervalBetweenEvents = 4;
        private int DaysOfLiveCookies = 2;
        Timer timer;
        public CustomerShoppingCartService(SqlDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.timer = GetTimer();
        }

        private Timer GetTimer()
        {
            var timer = new System.Timers.Timer( 1*60*60 * 1000);
            timer.Elapsed += TimerEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
            return timer;
        }

        private void TimerEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            dbContext.CustomersShoppingCart.Where(e => e.LastCheck.AddDays(2) < DateTime.Today).ExecuteDelete();
        }

        public double GetTimerCoolDown()
        {
	        return timer.Interval;
        }
    }
}
