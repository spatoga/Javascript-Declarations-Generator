using JSDeclarationsGenerator.Code.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace JSDeclarationsGenerator.Test
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Enter file Path:");
            string filepath = Console.ReadLine();

            if (!File.Exists(filepath)) {
                Console.WriteLine("");
                Console.WriteLine("File Does not exist!");
                Console.WriteLine("Enter file Path:");
                filepath = Console.ReadLine();
            }

            JSGenerator generator = new JSGenerator();
            
            Console.WriteLine("Reading skiptags.txt, listcontrols.txt");
            generator.ASPX_Tags_To_Skip = LoadOptions("skiptags.txt", generator.ASPX_Tags_To_Skip);
            generator.ASPX_Tags_With_Options = LoadOptions("listcontrols.txt", generator.ASPX_Tags_With_Options);

            generator.LoadFile(filepath);

            Console.WriteLine("Declarations:");
            Console.WriteLine(generator.GenerateDeclarations());

            Console.WriteLine("Enter Any Key to Exit");
            Console.ReadKey();

        }

        private static string[] LoadOptions(string filename, string[] defaults)
        {
            if (File.Exists(filename))
            {
                string text = File.ReadAllText(filename);

                List<string> results = new List<string>();

                foreach (string a in text.Split($",{ Environment.NewLine }")) 
                {
                    if (a.Trim() != "") { results.Add(a.Trim(',')); }
                }

                return results.ToArray();
            }

            Console.WriteLine(filename + " not found. Using Defaults");

            return defaults;
        }
    }
}
