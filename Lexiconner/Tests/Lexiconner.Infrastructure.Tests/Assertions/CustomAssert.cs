using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lexiconner.Infrastructure.Tests.Assertions
{
    /// <summary>
    /// Custom assertions, because Assert has static method and we can't write extensions for it
    /// </summary>
    public static class CustomAssert
    {
        /// <summary>
        /// Checks that all tasks throw exception of type TException
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public static Task AllThrowsAsync<TException>(IEnumerable<Task> tasks)
        {
            try
            {
                Parallel.ForEach(tasks, task =>
                {
                    try
                    {
                        task.GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        if (ex is TException)
                        {
                            // this is what we checking, so stop
                            return;
                        }
                        else
                        {
                            throw;
                        }
                    }

                    throw new InvalidOperationException($"Exception of type {typeof(TException).Name} wasn't thrown!");
                });
            }
            catch (AggregateException ex)
            {
                // rethrow to avoid getting AggregateException
                throw ex.InnerException ?? ex;
            }
            return Task.CompletedTask;
        }
    }
}
