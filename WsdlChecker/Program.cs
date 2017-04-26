using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WsdlExtractor;

namespace WsdlChecker
{
    /// <summary>
    /// Simple testing app for WsdlExtractor
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var lines = System.IO.File.ReadAllLines(@"testUrls.txt");
            var goodLines = new List<string>();

            foreach(var testUrl in lines)
            {
                var result = Core.GetDataTypes(testUrl);

                foreach (var url in result.XsdUrls)
                {
                    Console.WriteLine(url);
                }

                Console.WriteLine("-------------------------------------------------------------------------");
                Console.WriteLine("\n");

                foreach (var type in result.Types.Keys)
                {
                    Console.WriteLine($" >>> ({type}) : ");
                    Console.WriteLine();

                    foreach (var desc in result.Types[type])
                    {
                        var name = String.Format("{0,-35}", desc.Name);
                        var typetype = String.Format("{0,-35}", desc.Type);
                        var typelib = String.Format("{0,-30}", desc.TypeLib);
                        Console.WriteLine($"{name}  |  {typetype}  |  {typelib}");
                    }

                    Console.WriteLine();

                }

                Console.WriteLine();
                Console.WriteLine("#########################################################################");
                Console.WriteLine();
                Console.ReadKey();
            }



            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
