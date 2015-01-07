using System;
using System.Threading;


namespace TestMemoryMapFile
{
   
    public class WaitOne
    {
        static AutoResetEvent autoEvent = new AutoResetEvent(false);
        public static void waitExec()
        {
            Console.WriteLine("Main starting.");
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorkMethod), autoEvent);
            //Wait for work method to signal.
            autoEvent.WaitOne();
            Console.WriteLine("Work method signaled.\nMain ending.");
        }

        public static void WorkMethod(object stateInfo)
        {
            Console.WriteLine("Work starting.");

            // Simulate time spent working.
            Thread.Sleep(new Random().Next(100, 2000));

            // Signal that work is finished.
            Console.WriteLine("Work ending.");
            ((AutoResetEvent)stateInfo).Set();
        }
    }
}
