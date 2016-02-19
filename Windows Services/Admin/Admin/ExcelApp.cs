/*
*Filename:		ExcelApp.cs
*Project:		RDB Assignment 4 - WMP Assignment 6
*By:			Zheng Hua/Shaohua Mao
*Date:			2015.11.27
*Description:	The file includes class ExcelApp, this class is used to show the admin
*               with the list of the question, the question ID, the every time to answer the question correctly
*               and the histogram to show the every time to answer the question correctly
*/


using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;


namespace Admin
{
    class ExcelApp
    {
        /// NAME	:	FormAdmin
        /// PURPOSE :   This class is used to allow admin to :
        ///             view the question number, question text, average time to answer correctly, 
        ///             percentage of answering the question correctly, 
        ///             and a histogram to show the average length of time needed to answer each question correctly
        private List<Questions> allQuestions;       // list of all the questions
        private List<double> percentage;            // list to record the percentage of answering the question correctly
        private DAL dal;                            // contains methods to talk to database


        ///Function:		createExcel
        ///Description:     this method is called to create an excel file and initiate the cells with the 
        ///                 data of the questions and statistic
        ///Parameters:      MySqlConnection cn: mysql connection
        ///                 MySqlCommand cm: mysql command
        ///                 MySqlDataReader r: mysql reader
        ///Return Values:   result string: used to return the status of the excel
        public string createExcel(DAL dataAccess)
        {
            dal = dataAccess;
            
            // new an excel application
            Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            string result="";
            // check if it created
            if (excelApp == null)
            {
                result="Sorry, EXCEL could not be started.";
                return result;
            }

            // create an workbook and a worksheet
            Workbook wb = excelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            Worksheet ws = (Worksheet)wb.Worksheets[1];

            // check if it created
            if(ws==null)
            {
                result = "Sorry, worksheet could not be created";
                return result;
            }

            // get the list of all questions
            allQuestions = dal.GetAllQuestions();
            percentage = dal.GetPercentage();               // get the list of the percentage for each question

            // array that will be used to assign the value to the cells
            string[,] titleArray = new string[1, 4];

            // array for the title
            titleArray[0, 0] = "Question ID";
            titleArray[0, 1] = "Question text";
            titleArray[0, 2] = "Average time (s)";
            titleArray[0, 3] = "Percentage of answering correctly (%)";

            // set the range of the table
            Range rangeTitle = ws.get_Range("A1", "D1");
            rangeTitle.Value = titleArray;          // set the title of the table

            // array that will be used to assign the value to the cells
            string[,] Questions = new string[(allQuestions.Count + 1),2];

            // loop to set the array that will be used to assign value of the cells
            for (int i = 0; i < allQuestions.Count; i++)
            {
                // assign the value
                Questions[i, 0] = allQuestions[i].questionID.ToString();
                Questions[i, 1] = allQuestions[i].questionText;
            }

            // set the range
            Range RangeQuestion = ws.get_Range("A2", "B" + (allQuestions.Count + 1).ToString());
            RangeQuestion.Value = Questions;            // assign the question id and text to the cells

            // array that will be used to assign the value to the cells
            double[ , ] percentageArr = new double[allQuestions.Count + 1, 2];

            // loop to set the array that will be used to assign value of the cells
            for (int i = 0; i < allQuestions.Count; i++)
            {
                // check it is 0
                if (allQuestions[i].averageTime == "")
                {
                    percentageArr[i, 0] = 0;        // set to 0
                }
                else
                {
                    // assign the value
                    percentageArr[i, 0] = Double.Parse(allQuestions[i].averageTime);
                }
                percentageArr[i, 1] = percentage[i] * 100;      // change to percentage
            }

            // set the range
            Range RangePer = ws.get_Range("C2", "D" + (allQuestions.Count + 1).ToString());
            RangePer.Value = percentageArr;             // assign the percentage to the cells

            
            // going to create a chart
            Excel.Shape chart = ws.Shapes.AddChart(Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            chart.Chart.ChartType = Excel.XlChartType.xlColumnClustered;

            // set range
            Excel.Range chartRange;
            chartRange = ws.get_Range("C2", "C11");

            chart.Chart.SetSourceData(chartRange, Type.Missing);
            chart.Chart.HasTitle = true;
            chart.Chart.ChartTitle.Text = "Average Time to Answer Correctly";   // set the title

            //Set the y-axis of the chart
            var yAxis = (Excel.Axis)chart.Chart.Axes(Excel.XlAxisType.xlValue, Excel.XlAxisGroup.xlPrimary);
            yAxis.HasTitle = true;
            yAxis.AxisTitle.Text = "Average Time (second)";

            //Vertical Allignment of y-axis title
            yAxis.AxisTitle.Orientation = Excel.XlOrientation.xlVertical;


            //Set the X-axis of the chart
            var xAxis = (Excel.Axis)chart.Chart.Axes(Excel.XlAxisType.xlCategory, Excel.XlAxisGroup.xlPrimary);
            xAxis.HasTitle = true;
            xAxis.AxisTitle.Text = "Question";

            yAxis.AxisTitle.Orientation = Excel.XlOrientation.xlHorizontal;

            // set the size of the chart
            chart.Width = 350;
            chart.Height = 350;

            excelApp.Visible = true;        // Make it visible

            return result;
        }

    }
}
