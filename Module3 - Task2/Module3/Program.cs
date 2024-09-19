using System;
using System.IO;

namespace Module3
{
    internal class Program
    {
        static void Main()
        {
            var visitor = new FileSystemVisitor(@"C:\Users\Yohana_Espinoza\Desktop\cv", path => Path.GetExtension(path) == ".txt");

            visitor.Start += () => Console.WriteLine("Search Started.");
            visitor.Finish += () => Console.WriteLine("Search Finished.");
            visitor.FileFound += (item, args) =>
            {
                Console.WriteLine($"File found: {item}");
                if (item.Contains("stop")) args.Stop = true;
            };

            foreach (var file in visitor.GetFiles())
            {
                Console.WriteLine(file);
            }

            Console.ReadKey();
        }
    }
}
