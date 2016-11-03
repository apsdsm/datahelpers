using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// A single parsable row of a file stored using tabular data. this should be combined directly
/// with validator row, to skip an unnecessary step.
/// </summary>
public class ParsableRow {

    // an array containing the contents for each cell
    public string[] cells;

    // the line at which the data was extracted
    public int linenumber;
}