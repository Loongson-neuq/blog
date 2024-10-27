class SingleThreaded : DemoBase
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

        for (int t = 0; t < 16; t++)
        {
            Add_100000();
        }

        Console.WriteLine($"Processed value of variable `counter`: {counter}");

        await Task.CompletedTask;
    }
}