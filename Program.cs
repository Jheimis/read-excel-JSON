

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ReadExcel
{
    class Program
    {
        static void Main(string[] args)
        {
            string excelString = ReadExcelJSON();
            Console.WriteLine("Hello World!");
        }

        static string ReadExcelJSON()
        {
            try
            {
                DataTable dtTable = new DataTable();
                //Lets open the existing excel file and read through its content . Open the excel using openxml sdk
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open("test.xlsx", false))
                {
                    //create the object for workbook part  
                    WorkbookPart workbookPart = doc.WorkbookPart;
                    Sheets thesheetcollection = workbookPart.Workbook.GetFirstChild<Sheets>();

                    //using for each loop to get the sheet from the sheetcollection  
                    foreach (Sheet thesheet in thesheetcollection.OfType<Sheet>())
                    {
                        //statement to get the worksheet object by using the sheet id  
                        Worksheet theWorksheet = ((WorksheetPart)workbookPart.GetPartById(thesheet.Id)).Worksheet;

                        SheetData thesheetdata = theWorksheet.GetFirstChild<SheetData>();



                        for (int rCnt = 0; rCnt < thesheetdata.ChildElements.Count(); rCnt++)
                        {
                            List<string> rowList = new List<string>();
                            for (int rCnt1 = 0; rCnt1
                                < thesheetdata.ElementAt(rCnt).ChildElements.Count(); rCnt1++)
                            {

                                Cell thecurrentcell = (Cell)thesheetdata.ElementAt(rCnt).ChildElements.ElementAt(rCnt1);
                                //statement to take the integer value  
                                string currentcellvalue = string.Empty;
                                if (thecurrentcell.DataType != null)
                                {
                                    if (thecurrentcell.DataType == CellValues.SharedString)
                                    {
                                        int id;
                                        if (Int32.TryParse(thecurrentcell.InnerText, out id))
                                        {
                                            SharedStringItem item = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(id);
                                            if (item.Text != null)
                                            {
                                                //first row will provide the column name.
                                                if (rCnt == 0)
                                                {
                                                    dtTable.Columns.Add(item.Text.Text);
                                                }
                                                else
                                                {
                                                    rowList.Add(item.Text.Text);
                                                }
                                            }
                                            else if (item.InnerText != null)
                                            {
                                                currentcellvalue = item.InnerText;
                                            }
                                            else if (item.InnerXml != null)
                                            {
                                                currentcellvalue = item.InnerXml;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (rCnt != 0)//reserved for column values
                                    {
                                        rowList.Add(thecurrentcell.InnerText);
                                    }
                                }
                            }
                            if (rCnt != 0)//reserved for column values
                                dtTable.Rows.Add(rowList.ToArray());

                        }

                    }

                    return JsonConvert.SerializeObject(dtTable);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

    }
}
