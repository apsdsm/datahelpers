using System.Collections.Generic;

namespace DataHelpers {

    /// <summary>
    /// A bundle of the things required to import all the data in a given file to the game. Functions
    /// as a return object for a Reader's ReadAsset method.
    /// </summary>
    public class ImportData {

        // Dictionary of #metadata contained in a file 
        public Dictionary<string, string> meta = new Dictionary<string, string>();

        // Dictionary of $variables contained in a file
        public Dictionary<string, string> vars = new Dictionary<string, string>();

        // List of the fieldnames contained in a file
        public List<string> fieldNames = new List<string>();

        // a list of all of rows that have been validated by a validator
        public List<Row> rows = new List<Row>();
    }

}