using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ForkBro.Controller
{
    public interface IWorker
    {
        bool IsWork { get; set; }
        protected Thread thread { get; set; }

        abstract void Work(object delay);
        public bool StartWork(int delay,string customName=null)
        {
            bool result = false;
            try
            {
                if (thread == null || !thread.IsAlive)
                {
                    thread = new Thread(Work);
                    thread.Name = customName != null? customName : GetType().Name;                    
                    thread.Start(delay);
                    IsWork = true;
                    result = true;
                }
            }
            finally { }
            return result;

        }
        public bool StopWork(int ms_wait)
        {
            bool result = false;
            try
            {
                IsWork = false;
                if(!thread.Join(ms_wait))
                    thread.Abort();
                result = true;
            }
            finally { }
            return result;
        }
    }
}
