class Locked : DemoBase
{
    public override async Task Main()
    {
        ulong counter = 0;

        // Added a sync object
        object sync = new object();

        Console.WriteLine($"Initial value of variable `counter`: {counter}");

        void Add_100000()
        {
            for (int i = 0; i < 100000; i++)
            {
                // Explicitly lock the code block
                lock (sync)
                {
                    counter++;
                }
            }

            // Micro view of the loop body

            // Unprotected Version:
            //     Fetch the counter and store it in a certain register
            //     Add 1 to the register
            //     Write the new value back to the memory

            // Imagine the situation, where
            // the first frame Thread 0 and Thread 1 both stored the counter in their register.
            // the second frame, Thread 0 and Thread 1 both added 1 to their register.
            // the third frame, Thread 0 and Thread 1 both write the register to the memory where `counter` exists
            // What's the value of `counter` now?

            // Lock comes to rescue
            // Lock makes a batch of opeations atomatic

            // Locked Version
            //     Check if the lock is obtained by someone
            //         if obtained, wait for release
            //         not obtained, try obtain it.
            //     Lock is obtained by this thread
            //     Fetch the counter and store it in a certain register
            //     Add 1 to the register
            //     Write the new value back to the memory
            //     Release the lock so that other threads can add it.
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