if (args.Length != 1)
{
    Console.WriteLine("Please pass the name of the class to run as the only argument.");
    Environment.Exit(1);
    return;
}

string typeName = Path.ChangeExtension(args.Single(), string.Empty)
    .TrimEnd('.');

DemoBase? demo = null;
try
{
    demo = (DemoBase?)Activator.CreateInstance(
        typeof(Program).Assembly.FullName!, typeName)?.Unwrap();
}
catch (Exception)
{
    FastFail();
}

if (demo is DemoBase nonNull)
    await nonNull.Main();
else
    FastFail();

static void FastFail()
{
    Console.WriteLine("Type not found or not matched, please check your input");
    Environment.Exit(1);
}