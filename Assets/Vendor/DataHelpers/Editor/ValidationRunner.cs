using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using DataHelpers.Interfaces;


/// <summary>
/// Base class validator. All validators should be derived from this class.
/// </summary>
public class ValidationRunner {

    // a list of all the nodes used in this validator
    List<ValidatorRow> validationChain = new List<ValidatorRow>();

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
    public ValidatorRow[] Nodes {
        get {
            return validationChain.ToArray();
        }
    }

    /// <summary>
    /// Add parsable rows from the read bundle, which should have the field names included.
    /// </summary>
    /// <param name="rb">Rb.</param>
    public void AddParsableRows(ImportData rb) {

        foreach (ParsableRow p in rb.rows) {
            ValidatorRow v = new ValidatorRow();
            v.lineNumber = p.linenumber;

            for (int i = 0; i < rb.fieldNames.Count; i++) {

                var fieldName = rb.fieldNames[i];
                var field = new ValidatorField() { value = i < p.cells.Length ? p.cells[i] : "" };

                v.fields.Add(fieldName, field);
            }

            validationChain.Add(v);
        }
    }

    /// <summary>
    /// Check to see if the current validation chain is valid.
    /// </summary>
    /// <returns>true if valid, otherwise false</returns>
    public bool IsValid(ImportData readBundle, IValidator validator) {
        foreach (ValidatorRow n in validationChain) {
            validator.Validate(n, this);

            if (!n.valid) {
                errors.Add(n.errorMessage);
                valid = false;
            }
        }

        // if valid, copy the validation chain to the read bundle
        if (valid) {
            readBundle.validationRows = validationChain;
        }

        return valid;
    }
}