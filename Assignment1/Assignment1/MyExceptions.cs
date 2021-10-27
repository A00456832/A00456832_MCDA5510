using System;
using System.IO;

namespace Assignment1
{
    public class MyExceptions : ApplicationException
    {
        public void BlankFilePathException()
        {               
            Console.WriteLine("Please provide the source file path.");                 
        }
            
    }
}