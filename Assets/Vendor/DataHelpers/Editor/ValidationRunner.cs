
using System.Collections.Generic;
using DataHelpers.Contracts;

namespace DataHelpers {

    /// <summary>
    /// Base class validator. All validators should be derived from this class.
    /// </summary>
    public class ValidationRunner {

        // a list of all the nodes used in this validator
        List<Row> validationChain = new List<Row>();

        // A list of all the errors that are encountered
        List<string> errors = new List<string>();

        // keeps track of the overall validation validity
        bool valid = true;


        /// <summary>
        /// An array of the error values that were encountered.
        /// </summary>
        public string[] Errors {
            get {
                return errors.ToArray();
            }
        }


        /// <summary>
        /// An array of all the nodes in the validation chain.
        /// </summary>
        public Row[] Nodes {
            get {
                return validationChain.ToArray();
            }
        }


        /// <summary>
        /// Check to see if the current validation chain is valid.
        /// </summary>
        /// <returns>true if valid, otherwise false</returns>
        public bool IsValid(ImportData readBundle, IValidator validator) {
            foreach (Row n in validationChain) {
                validator.Validate(n);

                if (!n.valid) {
                    errors.Add(n.errorMessage);
                    valid = false;
                }
            }

            // if valid, copy the validation chain to the read bundle
            if (valid) {
                readBundle.rows = validationChain;
            }

            return valid;
        }
    }
}