using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Assignment1
{
    class SimpleCSVParser
    {
        public  string[] GetFilesFromSourceDirectory(string SourceDirectory)
        {
            /* This method accepts Source Directory as an input parameter and fetches the list of csv files in iterative manner. */
            string[] dirs = Directory.GetFiles(SourceDirectory, "*.csv", SearchOption.AllDirectories);
            return dirs;           
        }

        public static DataTable ConvertCSVtoDataTable(string strFilePath, string SourceDirectory, string Failure_ErrorLogFileName)
        {
            /* This method reads csv file from the Source Directory.
             Insert records into Datatable for intermediate processing purpose and not to exclusively lock the source file.
             
            Valid records: 
            If all the column value are present.
            If comma (,) is present in the column value, I have used regex to validate this.
         
            Invalid record: 
            If any of the column values are blank or null then log the record number and missing column name into the ErrorOutput.txt
            If any of the column is missing then log the record number and missing column name into the ErrorOutput.txt
           
            */
            DataTable dt = new DataTable();
            try
            {
                Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))"); /* Regex is used to validate of given special character present in the data field. It should be allowed.*/
                using (StreamReader sr = new StreamReader(strFilePath))
                {
                    try
                    {
                        string[] headers = sr.ReadLine().Split(',');
                        foreach (string header in headers)
                        {
                            dt.Columns.Add(header);
                        }
                        while (!sr.EndOfStream)
                        {
                            string[] RowTemp = new string[10]; /* If few column are not mentioned in the source file then I need to identify the columns for which data value is not given.
                                                                I have 10 fixed columns in the source file.*/      
                            string[] rows = CSVParser.Split(sr.ReadLine());
                            rows.CopyTo(RowTemp, 0);
                            DataRow dr = dt.NewRow();
                            for (int i = 0; i < headers.Length; i++) /*  && rows.Length == headers.Length */
                            {
                                dr[i] = RowTemp[i];
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                    catch (Exception Ex)
                    {
                        Console.WriteLine("Runtime error occurred 1: " + Ex.Message);                               
                        File.WriteAllText(SourceDirectory + "\\" + Failure_ErrorLogFileName, "Message : " + Ex.Message + " >> StackTrace : " + Ex.StackTrace+ Environment.NewLine);                                           
                    }
                }                                
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Runtime error occurred 2: " + Ex.Message);
            }            
            return dt;
        }

        public static void WriteFile(string Filename, StringBuilder sb) /* This method is used to append stringbuilder contents to destination file.*/
        {        
            try
            {                              
                String Directory_Output = Filename.Substring(0, Filename.LastIndexOf(@"\"));

                if (!Directory.Exists(Directory_Output))
                { 
                    Directory.CreateDirectory(Directory_Output);
                }

                String ActualFilename = Filename.Substring(Filename.LastIndexOf(@"\"), Filename.LastIndexOf(".")+4 - Filename.LastIndexOf(@"\"));

                if (File.Exists(Directory_Output + "\\" + ActualFilename))
                {
                    File.Delete(Directory_Output + "\\" + ActualFilename);
                }
               
                File.AppendAllText(Directory_Output + "\\" + ActualFilename, sb.ToString());                          
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Runtime error occurred 3: " + Ex.Message);               
            }
        }

      
        public void ApplyBusinessLogic(string[] DirectoryStructure, string SourceDirectory)
        {
            string Success_OutputFileName = SourceDirectory +@"\Assignment1\Output\Output.txt";
            string Failure_ErrorLogFileName = SourceDirectory + @"\Assignment1\logs\ErrorOutput.log";

            try
            {      
                var watch = new System.Diagnostics.Stopwatch(); /* This variable is used to track the execution time of the program. */

                watch.Start();
                Int64 TotalValidRecordCount = 0;  /* This variable is used track the valid record count. */
                Int64 TotalInvalidRecordCount = 0; /* This variable is used track the invalid record count. */

                StringBuilder failureBuilder = new StringBuilder(); /* The failure error log is maintained in the ErrorOutput.txt. */
                StringBuilder successBuilder = new StringBuilder(); /* This stors the valid data row details whic hwill be inserted in to Output.csv. */

                successBuilder.AppendLine("First Name,Last Name,Street Number,Street,City,Province,Postal Code,Country,Phone Number,email Address"); /*Add column list in the first row for the output file.*/

                // Get list of all files from directory
                foreach (string dir in DirectoryStructure)
                {
                              
                    int FullFileName_Length = dir.Split('\\').Length;
                    string[] DateField = dir.Split('\\')[(FullFileName_Length - 4)..(FullFileName_Length - 1)];
                    /*Assumption: The csv file must be present in the similar cascaded folder structure (Sample Data\2018\12\24)
                     I am reading '2018', '12', '24' from this folder structure by manipulating the length and index of the file path as shown in the above code. */

                    // Get csv file and print its name on the console.
                    Console.WriteLine(dir);

                    //convert csv file into datatable for further operations
                    DataTable dt = ConvertCSVtoDataTable(dir, SourceDirectory, Failure_ErrorLogFileName);

                    List<string> stringEnumerable;
                    bool isNonEmpty;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        stringEnumerable = new List<string>();
                        isNonEmpty = true;

                        for (int j = 0; j < dt.Columns.Count; j++)
                        {                            
                            if (string.IsNullOrEmpty(dt.Rows[i][j].ToString()) || dt.Rows[i][j].ToString() == "\"\"") /* If any of the column value is null or blank then consider it as invalid record.*/
                            {
                                stringEnumerable.Add(dt.Columns[j].ToString());
                                isNonEmpty = false;
                            }
                        }

                        if (isNonEmpty) /* Flag based approach, if it is a valid data  then write record in the output file. */
                        {

                            successBuilder.Append(string.Join(',', dt.Rows[i].ItemArray));
                            successBuilder.AppendLine(',' + string.Join('-', DateField));
                            TotalValidRecordCount = TotalValidRecordCount + 1; /* Increment the counter by 1 to keep track of valid records. */
                        }
                        else /* In case of invalid record, log the detailed error in log file. */
                        {
                            /* Prepare a  list of invalid columns into string builder object. */
                            failureBuilder.AppendLine(string.Format("File name: {0}, Row number {1} has blank or null for {1} column(s).", dir,i+1,string.Join(",", stringEnumerable) ));
                            TotalInvalidRecordCount = TotalInvalidRecordCount + 1; /* Increment the counter by 1 to keep track of invalid records. */
                        }

                    }
                }
                /* Below code writes the details into Output file. */
                successBuilder.AppendLine(string.Format("Execution Time:{0} in milliseconds, Total valid record count: {1}, Total Invalid Record Count: {2}", watch.ElapsedMilliseconds, TotalValidRecordCount, TotalInvalidRecordCount));

                /* Below code writes the details into Output and error log file. */
                WriteFile(Success_OutputFileName, successBuilder); 
                WriteFile(Failure_ErrorLogFileName, failureBuilder);
            
                watch.Stop();
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Runtime error occurred: " + Ex.Message);
                File.AppendAllText(SourceDirectory + "\\" + Failure_ErrorLogFileName, "Message : " + Ex.Message + " >> StackTrace : " + Ex.StackTrace);              
            }
        }


      
      
}
}