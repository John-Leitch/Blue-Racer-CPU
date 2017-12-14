using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public static class TaskExtension
    {
        public static TResult Sync<TResult>(this Task<TResult> task)
        {
            try
            {
                task.Wait();
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }

            return task.Result;
        }
    }
}
