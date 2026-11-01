using System.Diagnostics;

class SingleThreadedTimeMeasured : DemoBase
{
    public override async Task Main()
    {
        ulong counter = 0;

        Console.WriteLine($"Initial value of variable `counter`: {counter}");

        void Add_100000()
        {
            for (int i = 0; i < 100000; i++)
                counter++;
        }

        Stopwatch sw = Stopwatch.StartNew();
        {
            for (int t = 0; t < 16; t++)
            {
                Add_100000();
            }
        }
        sw.Stop();

        Console.WriteLine($"Processed value of variable `counter`: {counter}");
        Console.WriteLine($"Elapsed time: {sw.Elapsed.TotalMilliseconds:F3} ms");

        await Task.CompletedTask;
    }
}