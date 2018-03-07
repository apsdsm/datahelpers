
using System.Collections.Generic;
using DataHelpers.Contracts;

namespace DataHelpers {

    /// <summary>
    /// Base class validator. All validators should be derived from this class.
    /// </summary>
    public class ValidationRunner {

        /// <summary>
        /// Check to see if the current validation chain is valid.
        /// </summary>
        /// <returns>true if valid, otherwise false</returns>
        public bool IsValid(ImportData readBundle, IValidator validator) {

            var valid = true;

            foreach (Row row in readBundle.rows) {
                validator.Validate(row);

                if (!row.valid) {
                    valid = false;
                    Debug.LogError("Import failed " + row.errorMessage);
                }
            }

            return valid;
        }
    }
}
