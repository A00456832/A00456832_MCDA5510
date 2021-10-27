Name: Imtiyaz Shaikh
A#: A00456832
I have 11 years of experience as a 'Data engineer'.
I am not familier with frontend technologies but will explore those in the MSCDA program.

Thanks for conducting lectures.

Assignment #1
-------------------------------------------------------------------

The goal of the assignment is to simply combine 3 of these programs into a single
program that will recursively read a series of data files in CSV format and enter 
them into a single file.

The program must log the amount of time it takes to read the files in each directory 
and the time it takes to write the files to a file using the logger.


Assignment #1 comments by Imtiyaz:
This program reads the csv file from the source directory in the iterative manner.

Working flow:
1. The 'Assignment1' folder will be created under the user provide source directory.
2. All the runtime errors will be logged in the Errorlog.txt file.
3. Custom Exception is implemented is user input is not provided.

Valid record: 
If all the column value are present.
If comma (,) is present in the column value, I have used regex to validate this.
         
Invalid record: 
If any of the column values are blank or null then log the record number and missing column name into the ErrorOutput.txt
If any of the column is missing then log the record number and missing column name into the ErrorOutput.txt

Assumptions: 
1. The child directory structure should be in the same format '2018 >> 1 >> 8'.

References:
https://forums.asp.net/t/1247607.aspx?Reading+CSV+with+comma+placed+within+double+quotes+