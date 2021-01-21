
namespace ExceptionConsole
{
    using System;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    static class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                args = new string[1] { "--exception" };
            }

            switch (args[0])
            {
                case "--exception":
                    {
                        throw new InvalidOperationException("Hello World!");
                    }
                default:
                    {
                        Console.WriteLine(args[0]);
                        break;
                    }
            }
        }
    }
}
