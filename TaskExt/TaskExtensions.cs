using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TaskExt
{
    public static class TaskExtensions
    {
        /// <summary>
        /// A workaround for getting all of AggregateException.InnerExceptions with try/await/catch
        /// </summary>
        public static Task WithAggregatedExceptions(this Task @this)
        {
            // using AggregateException.Flatten as a bonus
            return @this.ContinueWith(
                continuationFunction: anteTask =>
                    anteTask.IsFaulted &&
                    anteTask.Exception is AggregateException ex &&
                    (ex.InnerExceptions.Count > 1 || ex.InnerException is AggregateException) ?
                    Task.FromException(ex.Flatten()) : anteTask,
                cancellationToken: CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                scheduler: TaskScheduler.Default).Unwrap();
        }

        public static async Task<T[]> WhenAll<T>(params Task<T>[] tasks)
        {
            var allTasks = Task.WhenAll(tasks);

            try
            {
                return await allTasks;
            }
            catch (Exception)
            {
                // ignore
            }

            throw allTasks.Exception ?? throw new Exception("Something went really wrong");
        }

        public static async Task WhenAll(params Task[] tasks)
        {
            var allTasks = Task.WhenAll(tasks);

            try
            {
                await allTasks;
                return;
            }
            catch (Exception)
            {
                // ignore
            }

            throw allTasks.Exception ?? throw new Exception("Something went really wrong");
        }

        public static TaskAwaiter<T[]> GetAwaiter<T>(this (Task<T>, Task<T>) tasksTuple)
        {
            return TaskExtensions.WhenAll(tasksTuple.Item1, tasksTuple.Item2).GetAwaiter();
        }

        public static TaskAwaiter<T[]> GetAwaiter<T>(this (Task<T>, Task<T>, Task<T>) tasksTuple)
        {
            return TaskExtensions.WhenAll(tasksTuple.Item1, tasksTuple.Item2, tasksTuple.Item3).GetAwaiter();
        }

        public static TaskAwaiter<T[]> GetAwaiter<T>(this (Task<T>, Task<T>, Task<T>, Task<T>) tasksTuple)
        {
            return TaskExtensions.WhenAll(tasksTuple.Item1, tasksTuple.Item2, tasksTuple.Item3, tasksTuple.Item4).GetAwaiter();
        }
    }
}
