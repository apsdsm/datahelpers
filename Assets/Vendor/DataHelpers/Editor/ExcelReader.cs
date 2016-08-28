using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

// for importing excel files
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

/// <summary>
/// Provides methods that assist in getting data from Excel documents that have been formated in a specific way.
/// </summary>
public class ExcelReader {

    /// <summary>
    /// Will return the value of a cell as a string, no matter what the actual cell type is.
    /// </summary>
    /// <param name="cell"></param>
    /// <returns>string representation of the cell</returns>
    private static string CellValueAsString(ICell cell) {
        string s = "";

        switch (cell.CellType) {
            case CellType.String:
                s = cell.StringCellValue;
                break;

            case CellType.Numeric:
                s = Convert.ToString(cell.NumericCellValue);
                break;

            case CellType.Boolean:
                s = Convert.ToString(cell.BooleanCellValue);
                break;
        }

        return s;
    }

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

			// get field names
			GetFieldNames(ref bundle.fieldNames, wb);

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

            // if there is a row, and it's not empty
            if (row != null && row.Cells.Count > 0) {

                // check to see if the row is parsable by looking at the first value
                ICell firstCell = row.GetCell(0);

                if (firstCell != null) {

                    string firstCellVal = CellValueAsString(firstCell);
                                        
                    // treat the row as parsable if it is:
                    // - not a title
                    // - not metadata
                    // - not a variable
                    if (firstCellVal[0] != '[' && firstCellVal[0] != '#' && firstCellVal[0] != '$') {

                        // cells are stored in parsable row objects
                        ParsableRow pr = new ParsableRow();

                        // copy the line number to the parsable row object
                        pr.linenumber = rindex;

						// make an array which is as long as the last cell number - note that NPOI will show cells **up to** the last cell that
						// contains data, so if a row in the sheet only has a single value in the fourth column, that row will return null for all
						// indices up to 2, then a value for index 3.
                        pr.cells = new string[row.LastCellNum];

                        // for each cell in the row, convert it to a string and copy it to the parsable row object
                        for (int i = 0; i < row.LastCellNum; i++) {
                            pr.cells[i] = CellValueAsString(row.GetCell(i));                   
                        }
                        
                        // add the parsable row object we just set up to the list of rows that was passed to the method
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

                    // get key and value as strings
                    string key = CellValueAsString(keyCell);
                    string val = CellValueAsString(valCell);

                    // if the key is not blank, and starts with the specified data precix, remove the prefix and add the keyvalue pair to the dictionary
                    if (key.Length > 1 && key.StartsWith(dataPrefix)) {
                        key = key.Replace(dataPrefix, "");
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


	/// <summary>
	/// Gets the field names from a workbook and stores them in a list.
	/// </summary>
	/// <param name="fieldNames">list where field names will be stored</param>
	/// <param name="workbook">workbook from which field names will be extracted</param>
	public static void GetFieldNames(ref List<string> fieldNames, XSSFWorkbook workbook) 
	{
		ISheet sheet = workbook.GetSheetAt( 0 );
		bool reading = true;
		int rindex = sheet.FirstRowNum;

		while ( reading )
		{
			IRow row = sheet.GetRow( rindex );

			if ( row != null )
			{
				ICell cell = row.GetCell( 0 );

				if ( cell != null ) 
				{
					string s = CellValueAsString(cell);

					if (s != "" && s[0] == '[') 
					{
						for (int i = 0; i < row.LastCellNum; i++) 
						{
							s = CellValueAsString(row.GetCell(i)).TrimEnd(']').TrimStart('[');;
							fieldNames.Add(s);
						}

						// don't read more than one row of field names
						reading = false;
					}
				}
			}

			// increase index
			rindex++;

			// stop reading if we're out of bounds
			reading = rindex <= sheet.LastRowNum;
		}
	}
}