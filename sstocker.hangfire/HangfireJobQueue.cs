using Hangfire;
using System;

namespace sstocker.hangfire
{
    public static class HangfireJobQueue
    {
        public static void EnqueueJob()
        {
            BackgroundJob.Enqueue(() => Console.WriteLine("Hangfire started!"));
        }
    }
}
