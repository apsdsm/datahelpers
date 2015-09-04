using UnityEngine;
using System.Collections.Generic;
using System.IO;

// for importing excel files
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;



/// <summary>
/// Provides methods that assist in getting data from Excel documents that have been formated in a specific way.
/// </summary>
public class ExcelReader {

    /// <summary>
    /// Read an excel file and extract the meta, variables, and parsable rows.
    /// </summary>
    /// <param name="assetPath"></param>
    public static void ReadXLSX(string assetPath, ref ReadBundle bundle) {

        Debug.Log("START TIMELINE IMPORT PROCESS FOR XLSX");

        // get an absolute path to the asset
        string absolutePath = System.IO.Directory.GetCurrentDirectory() + "/" + assetPath;

        // open a file stream to the asset
        using (FileStream fs = new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {

            // get workbook
            XSSFWorkbook wb = new XSSFWorkbook(fs);

            // get key/value meta data
            GetKeyValData(ref bundle.meta, wb, "#");

            // get key/value 
            GetKeyValData(ref bundle.vars, wb, "$");

            // get the parsable rows
            GetParsableRows(ref bundle.rows, wb);

        }
    }


    /// <summary>
    /// Get the parsable rows from a workbook
    /// </summary>
    /// <param name="rows">a reference to an object that will contain parsable rows</param>
    /// <param name="workbook">the workbook to extract rows from</param>
    public static void GetParsableRows(ref List<ParsableRow> rows, XSSFWorkbook workbook) {

        ISheet sheet = workbook.GetSheetAt(0);

        bool reading = true;
        int rindex = sheet.FirstRowNum;

        while (reading) {
            IRow row = sheet.GetRow(rindex);

            if (row != null) {

                // check to see if the row is parsable

                ICell firstCell = row.GetCell(0);

                if (firstCell != null) {

                    string firstCellVal = firstCell.StringCellValue;

                    // treat the cell as parsable if it is:
                    // - not empty
                    // - not a title
                    // - not metadata
                    // - not a variable
                    if (firstCellVal.Length > 0 && firstCellVal[0] != '[' && firstCellVal[0] != '#' && firstCellVal[0] != '$') {

                        string[] cells = new string[row.LastCellNum];

                        ParsableRow pr = new ParsableRow();

                        pr.linenumber = rindex;
                        pr.cells = new string[row.LastCellNum];

                        for (int i = 0; i < row.LastCellNum; i++) {
                            pr.cells[i] = row.GetCell(i).StringCellValue;
                        }

                        rows.Add(pr);
                    }
                }
            }

            rindex++;

            // check to make sure we're not out of bounds
            if (rindex > sheet.LastRowNum) {
                reading = false;
            }
        }
    }


    /// <summary>
    /// Get meta data from the workbook, and store it in the provided dictionary.
    /// </summary>
    /// <param name="data">where meta will be stored</param>
    /// <param name="workbook">the workbook to extract data from</param>
    /// <param name="assetpath">the path of the asset being trawled</param>
    public static void GetKeyValData(ref Dictionary<string, string> data, XSSFWorkbook workbook, string dataPrefix) {

        ISheet sheet = workbook.GetSheetAt(0);

        bool reading = true;
        int rindex = sheet.FirstRowNum;

        while (reading) {

            // get the current row
            IRow row = sheet.GetRow(rindex);

            // check the row to see if it's a valid variable row, and if so copy it to the meta dictionary
            if (row != null) {
                ICell keyCell = row.GetCell(0);
                ICell valCell = row.GetCell(1);

                if (keyCell != null && valCell != null) {

                    string key = keyCell.StringCellValue;
                    string val = valCell.StringCellValue;

                    if (key.Length > 1 && key.StartsWith(dataPrefix)) {

                        // remove the data prefix from the variable name
                        key = key.Replace(dataPrefix, "");

                        // add to data set
                        data.Add(key, val);
                    }
                }
            }

            // increase index
            rindex++;

            // check to make sure we're not out of bounds
            if (rindex > sheet.LastRowNum) {
                reading = false;
            }
        }
    }
}