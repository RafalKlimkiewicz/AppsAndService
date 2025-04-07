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

                Write(".");
            }
        }
    }

    private static void MethodB()
    {
        for (int i = 0; i < 5; i++)
        {
            Thread.Sleep(Random.Shared.Next(2000));

            SharedObjects.Messsage += "B";

            Write(".");
        }
    }
}
