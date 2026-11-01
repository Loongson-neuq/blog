using System.Diagnostics;

class LockedTimeMeasured : DemoBase
{
    public override async Task Main()
    {
        ulong counter = 0;

        // TODO: Added a sync object
        object sync = new object();

        Console.WriteLine($"Initial value of variable `counter`: {counter}");

        void Add_100000()
        {
            for (int i = 0; i < 100000; i++)
            {
                // TODO: Explicitly lock the code block
                lock (sync)
                {
                    counter++;
                }
            }
        }

        List<Task> threads = [];

        Stopwatch sw = Stopwatch.StartNew();
        {
            for (int t = 0; t < 16; t++)
            {
                threads.Add(Task.Run(Add_100000));
            }

            await Task.WhenAll(threads);
        }
        sw.Stop();

        Console.WriteLine($"Processed value of variable `counter`: {counter}");
        Console.WriteLine($"Elapsed time: {sw.Elapsed.TotalMilliseconds:F3} ms");
    }
}