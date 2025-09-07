class Unprotected : DemoBase
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

        List<Task> threads = [];

        for (int t = 0; t < 16; t++)
        {
            threads.Add(Task.Run(Add_100000));
        }

        await Task.WhenAll(threads);

        Console.WriteLine($"Processed value of variable `counter`: {counter}");
    }
}