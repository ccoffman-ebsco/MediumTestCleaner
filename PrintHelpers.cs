using System;
using System.Collections.Generic;
using System.Text;

namespace MediumTestCleaner
{
    public static class PrintHelpers
    {        
        public static void PrintList<T>(List<T> testFoldersOnly)
        {
            Console.WriteLine("Printing list of " + testFoldersOnly.Count + " " + typeof(T));
            foreach(var item in testFoldersOnly)
            {
                Console.WriteLine(item.ToString());
            }
        }
    }
}
