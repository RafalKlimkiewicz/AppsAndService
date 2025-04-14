partial class Program
{
    private static async IAsyncEnumerable<int> GetNumbersAsync()
    {
        Random r = Random.Shared;

        await Task.Delay(r.Next(1500, 3000));

        yield return r.Next(1, 10);

        await Task.Delay(r.Next(1500, 3000));

        yield return r.Next(20, 30);

        await Task.Delay(r.Next(1500, 3000));

        yield return r.Next(30, 40);
    }
}
