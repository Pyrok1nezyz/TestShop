﻿using Microsoft.EntityFrameworkCore;
using Timer = System.Timers.Timer;

namespace TestShop.Entitys
{
	public class CustomerShoppingCartService
    {
	    readonly SqlDbContext _dbContext;
        Timer _timer;

		private int HoursIntervalBetweenEvents = 4;

        public CustomerShoppingCartService(SqlDbContext dbContext)
        {
            this._dbContext = dbContext;
            this._timer = GetTimer();
        }

        private Timer GetTimer()
        {
            var timer = new System.Timers.Timer(HoursIntervalBetweenEvents * 60 * 60 * 1000);
            timer.Elapsed += TimerEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
            return timer;
        }

        private void TimerEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            _dbContext.CustomersShoppingCart.Where(e => e.LastCheck.AddDays(2) < DateTime.Today).ExecuteDelete();
        }
    }
}
