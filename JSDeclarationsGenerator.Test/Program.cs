using JSDeclarationsGenerator.Code.Models;
using System;

namespace JSDeclarationsGenerator.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter file Path:");
            string filepath = Console.ReadLine();

            JSGenerator generator = new JSGenerator();

            generator.LoadFile(filepath);

            Console.WriteLine("Declarations:");
            Console.WriteLine(generator.GenerateDeclarations());

        }
    }
}
