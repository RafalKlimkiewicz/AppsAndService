partial class Program
{
    private static void MethodA()
    {
        lock (SharedObjects.Conch)
        {
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(2000);

                SharedObjects.Messsage += "A";

                Interlocked.Increment(ref SharedObjects.Counter);

                Write(".");
            }
        }
    }

    private static void MethodAAntiDeadLock()
    {
        bool lockAcquired = false;
        try
        {
            lockAcquired = Monitor.TryEnter(SharedObjects.Conch, TimeSpan.FromSeconds(15));
            if (lockAcquired)
            {
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(2000);

                    SharedObjects.Messsage += "A";

                    Interlocked.Increment(ref SharedObjects.Counter);

                    Write(".");
                }
            }
            else
            {
                WriteLine("Method A timed out when entering a monitor on conch.");
            }
        }
        finally
        {
            if (lockAcquired)
                Monitor.Exit(SharedObjects.Conch);
        }
    }

    private static void MethodB()
    {
        bool lockAcquired = false;

        try
        {
            lockAcquired = Monitor.TryEnter(SharedObjects.Conch, TimeSpan.FromSeconds(15));
            if (lockAcquired)
            {
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(Random.Shared.Next(2000));

                    SharedObjects.Messsage += "B";

                    Interlocked.Increment(ref SharedObjects.Counter);

                    Write(".");
                }
            }
            else
            {
                WriteLine("Method B timed out when antering a monitor on conch.");
            }
        }
        finally
        {
            if (lockAcquired)
                Monitor.Exit(SharedObjects.Conch);
        }
    }
}
