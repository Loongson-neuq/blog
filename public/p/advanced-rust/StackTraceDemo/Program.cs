using System;

class Program
{
    static void Buz()
    {
        Console.WriteLine("Hello, world");
        // 1. Comment the following line to see the difference
        throw new Exception("Exception from Buz");
    }

    static void Bar()
    {
        Buz();
        // 2. Comment the following line to see the difference
        throw new Exception("Exception from Bar");
    }

    static void Foo()
    {
        Bar();
        // 3. Comment the following line to see the difference
        throw new Exception("Exception from Foo");
    }

    static void Main(string[] args)
    {
        Foo();
        // 4. Comment the following line to see the difference
        throw new Exception("Exception from Main");
    }
}
