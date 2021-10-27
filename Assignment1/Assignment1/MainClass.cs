using System;


namespace Assignment1
{
    public class MainClass
    {

        public static void Main(String[] args)
        {
            try
            {
                MainClass ec = new MainClass();
                SimpleCSVParser CSV = new SimpleCSVParser();
                Console.WriteLine(@"Example: D:\SMU\Sample Data");
                Console.WriteLine(@"'Assignment1' folder will be created under this source directory.");
                Console.Write("Enter the parent source directory: ");
                 string SourceDirectory = Console.ReadLine(); /* Accepts parent directory as a user input. */
              
                if (string.IsNullOrEmpty(SourceDirectory))
                { 
                throw new MyExceptions();
                }
                string[] DirectoryStructure = CSV.GetFilesFromSourceDirectory(SourceDirectory); /* Prepare the list of csv files from given source directory. */
                CSV.ApplyBusinessLogic(DirectoryStructure, SourceDirectory); /* Apply a business logic and inserts valid record in Output.csv and and invalid records in ErrorOutput.txt file. */
            }
            catch (MyExceptions e)
            {
                e.BlankFilePathException();
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Runtime error occurred: " + Ex.Message);
            }
        }      
    }
}
