using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

// for importing excel files
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

using DataHelpers.Contracts;

namespace DataHelpers.Readers {

    /// <summary>
    /// Provides methods that assist in getting data from Excel documents that have been formated in a specific way.
    /// </summary>
    public class ExcelReader : IReader {

        /// <summary>
        /// Read an excel file and extract the meta, variables, and parsable rows.
        /// </summary>
        /// <param name="assetPath"></param>
        public void ReadAsset(string assetPath, ref ImportData data) {

            Debug.Log("START IMPORT PROCESS FOR XLSX: " + Path.GetFileName(assetPath));

            // get an absolute path to the asset
            string absolutePath = System.IO.Directory.GetCurrentDirectory() + "/" + assetPath;

            // open a file stream to the asset
            using (FileStream fs = new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {

                // get workbook
                XSSFWorkbook wb = new XSSFWorkbook(fs);

                // get field names
                GetFieldNames(data, wb);

                // get key/value meta data
                GetKeyValData(ref data.meta, wb, "#");

                // get key/value 
                GetKeyValData(ref data.vars, wb, "$");

                // get the parsable rows
                GetRows(data, wb);
            }
        }

        /// <summary>
        /// Will return the value of a cell as a string, no matter what the actual cell type is.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns>string representation of the cell</returns>
        private static string CellValueAsString(ICell cell) {
            string s = "";

            if (cell == null)
                return s;

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
        /// A Row is parable and this method will return true only if:
        /// - it does not contain a field title
        /// - it does not contain metadata
        /// - it does not contain a variable
        /// - it is not empty
        /// </summary>
        /// <param name="row">row to check</param>
        /// <returns>true if row is parsable</returns>
        private bool RowIsParsable(IRow row) {

            if (row == null || row.Cells.Count == 0) {
                return false;
            }

            bool isEmpty = true;

            for (var i = row.FirstCellNum; i < row.LastCellNum; ++i) {
                ICell cell = row.GetCell(i, MissingCellPolicy.RETURN_BLANK_AS_NULL);

                if (cell != null) {
                    string val = CellValueAsString(cell);

                    if (val[0] == '[' || val[0] == '#' || val[0] == '$') {
                        return false;

                    } else if (val != "") {
                        isEmpty = false;
                    }
                }
            }

            if (isEmpty) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the parsable rows from a workbook
        /// </summary>
        /// <param name="rows">a reference to an object that will contain parsable rows</param>
        /// <param name="workbook">the workbook to extract rows from</param>
        private void GetRows(ImportData data, XSSFWorkbook workbook) {

            ISheet sheet = workbook.GetSheetAt(0);

            bool reading = true;
            int rowIndex = sheet.FirstRowNum;

            while (reading) {
                IRow row = sheet.GetRow(rowIndex);

                if (RowIsParsable(row)) {

                    var validatorRow = new Row();
                    validatorRow.lineNumber = rowIndex;

                    for (int i = 0; i < data.fieldNames.Count; ++i) {
                        var fieldName = data.fieldNames[i];
                        var field = new Field() { value = CellValueAsString(row.GetCell(i)) };

                        validatorRow.fields.Add(fieldName, field);
                    }

                    data.rows.Add(validatorRow);
                }

                rowIndex++;

                if (rowIndex > sheet.LastRowNum) {
                    reading = false;
                }
            }
        }


        /// <summary>
        /// Get key/value data from the workbook, and store it in the provided dictionary. The key for the
        /// data pair must be prefixed with a single character, such as '$' or '#'. The prefix will be 
        /// removed from the keyname before it is inserted into the dictionary.
        /// </summary>
        /// <param name="dictionary">where meta will be stored</param>
        /// <param name="workbook">the workbook to extract data from</param>
        /// <param name="assetpath">the path of the asset being trawled</param>
        private static void GetKeyValData(ref Dictionary<string, string> dictionary, XSSFWorkbook workbook, string dataPrefix) {

            ISheet sheet = workbook.GetSheetAt(0);

            bool reading = true;
            int rindex = sheet.FirstRowNum;

            while (reading) {

                IRow row = sheet.GetRow(rindex);

                if (row != null) {
                    ICell keyCell = row.GetCell(0);
                    ICell valCell = row.GetCell(1);

                    if (keyCell != null && valCell != null) {

                        // get key and value as strings
                        string key = CellValueAsString(keyCell);
                        string val = CellValueAsString(valCell);

                        // if the key is not blank, and starts with the specified data prefix, 
                        // remove the prefix and add the keyvalue pair to the dictionary
                        if (key.Length > 1 && key.StartsWith(dataPrefix)) {
                            key = key.Replace(dataPrefix, "");
                            dictionary.Add(key, val);
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
        public static void GetFieldNames(ImportData data, XSSFWorkbook workbook) {

            ISheet sheet = workbook.GetSheetAt(0);
            bool reading = true;
            int rindex = sheet.FirstRowNum;

            while (reading) {
                IRow row = sheet.GetRow(rindex);

                if (row != null) {
                    ICell cell = row.GetCell(0);

                    if (cell != null) {
                        string s = CellValueAsString(cell);

                        if (s != "" && s[0] == '[') {
                            for (int i = 0; i < row.LastCellNum; i++) {
                                s = CellValueAsString(row.GetCell(i)).TrimEnd(']').TrimStart('['); ;
                                data.fieldNames.Add(s);
                            }

                            // don't read more than one row of field names
                            reading = false;
                        }
                    }
                }

                rindex++;

                if (rindex > sheet.LastRowNum) {
                    reading = false;
                }
            }
        }
    }
}
