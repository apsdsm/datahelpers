﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;

/// <summary>
/// Generic interface describing any validation class.
/// </summary>
public interface IValidator {
    void AddParsableRows(List<ParsableRow> rows, string[] fieldnames);
    bool IsValid();
    string[] Errors { get; }
    ValidatorNode[] Nodes { get; }
}

/// <summary>
/// A generic validation node. These are used to store validated versions of row data.
/// </summary>
public class ValidatorNode {

    public int lineNumber = 0;
    public bool valid = true;
    public string message = "";

    // store the field data for this node
    public Dictionary<string, string> fields = new Dictionary<string, string>();

    // store pointers to resources used by this node
    public Dictionary<string, ScriptableObject> resources = new Dictionary<string, ScriptableObject>();

    // override the bracket accessor to make working with nodes a little more pleasant
    public string this[string fieldName] {
        get {
            return fields[fieldName];
        }

        set {
            fields[fieldName] = value;
        }
    }
}

/// <summary>
/// A validation object for scriptable objects.
/// </summary>
public class ResourceValidation {

    // what kind of resource
    public Type resourceType;

    // the resource
    public ScriptableObject resource;

    // what it's called or how it's referenced
    public string name;

    // the path to the object
    public string path;

    // if this is a valid resource or not
    public bool valid;
}


/// <summary>
/// Base class validator. All validators should be derived from this class.
/// </summary>
public abstract class Validator : IValidator {

    // a list of all the nodes used in this validator
    List<ValidatorNode> validationChain = new List<ValidatorNode>();

    // A list of all the errors that are encountered
    List<string> errors = new List<string>();

    // keeps track of the overall validation validity
    bool valid = true;

    // a list of all the resourceValidations that were performed for this validator
    List<ResourceValidation> resourceValidations = new List<ResourceValidation>();

    // method will be called when validating nodes
    readonly MethodInfo validatorMethod = null;

    // method will be called when fetching the fieldnames to import
    readonly MethodInfo fieldNamesMethod = null;

    /// <summary>
    /// Constructs a new validator.
    /// </summary>
    public Validator() {

        // get method info for a validation method (if one has been defined)
        validatorMethod = GetType().GetMethod("Validate", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(ValidatorNode) }, null);
    }


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
    public ValidatorNode[] Nodes {
        get {
            return validationChain.ToArray();
        }
    }


    /// <summary>
    /// Add a list of parsable rows to the validation chain.
    /// </summary>
    /// <param name="rows">the list of rows to add</param>
    public void AddParsableRows(List<ParsableRow> rows, string[] fieldnames) {

        // of each parsable row, copy the field data into a validation node, and then add that to the validation chain
        foreach (ParsableRow row in rows) {

            ValidatorNode node = new ValidatorNode();

            // copy values into node fields, either cell values, or just a blank strings if there are no more cells to read
            for (int i = 0; i < fieldnames.Length; i++) {
                node.fields.Add(fieldnames[i], i < row.cells.Length ? row.cells[i] : "");
            }

            // copy the line number
            node.lineNumber = row.linenumber;

            // add to validation chain
            validationChain.Add(node);
        }
    }

    /// <summary>
    /// Check to see if the current validation chain is valid.
    /// </summary>
    /// <returns>true if valid, otherwise false</returns>
    public bool IsValid() {

        // if there was no method found, flag a universal error and stop processing
        if (validatorMethod == null) {
            errors.Add("no validation methods are defined.");
            valid = false;
        }

        // if a method was found, run each node through the validation method. If any errors are reported
        // they'll be copied to the error list before processing continues.
        else {
            foreach (ValidatorNode n in validationChain) {
                validatorMethod.Invoke(this, new object[] { n });

                if (!n.valid) {
                    errors.Add(n.message);
                    valid = false;
                }
            }
        }

        return valid;
    }


    /// <summary>
    /// Checks to see if the specified resource exists. The resource must be derived from a scriptable object
    /// </summary>
    /// <typeparam name="T">tyep of the resource to look for</typeparam>
    /// <param name="name">the name of the asset</param>
    /// <param name="node">the node that is responsibel for the asset</param>
    /// <returns>true if resource is valid, otherwise false</returns>
    protected bool IsValidResource<T>(string name, ValidatorNode node) where T : ScriptableObject {

        ResourceValidation validation = null;
        ScriptableObject scriptable = null;
        bool status = true;
        Type type = typeof(T);

        try {

            // try find existing validation
            validation = resourceValidations.Find(i => i.resourceType == type && i.name == name);

            // if this validation hasn't been run before, run the validation
            if (validation == null) {

                // search for assets with the specific type
                string[] guids = AssetDatabase.FindAssets(name + " t:" + type.ToString());

                // don't allow if more than one asset with same type and name
                if (guids.Length > 1) {
                    SetErrorMessage(node, "conflict - more than one instance of resource exists.");
                    return false;
                }

                // don't allow if asset not found
                if (guids.Length <= 0) {
                    SetErrorMessage(node, "resource of type " + type.ToString() + " with name " + name + " was not found in asset database.");
                    return false;
                }

                // load the scriptable asset
                scriptable = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), type) as ScriptableObject;
            }
        }

        // if anything goes wrong, prime the values to reflect a false outcome
        catch {
            status = false;
            scriptable = null;
        }

        // if there is no validation yet assigned, create a new validation and add it to the queue
        if (validation == null) {
            validation = new ResourceValidation();
            validation.resourceType = type;
            validation.resource = scriptable;
            validation.valid = status;

            resourceValidations.Add(validation);
        }

        
        // add the scriptable object to the resources pile for this node
        if (scriptable != null && !node.resources.ContainsKey(name)) {
            node.resources.Add(name, scriptable);
        }
        
       
        // return final status
        return validation.valid;
    }

    /// <summary>
    /// An easy way to flag a node as invalid, and set an error message at the same time. Will include the line number in the error message.
    /// </summary>
    /// <param name="node">the node to mark as invalid</param>
    /// <param name="message">the message to be appended to the node</param>
    protected void SetErrorMessage(ValidatorNode node, string message) {
        node.valid = false;
        node.message = "error line: " + node.lineNumber + ": " + message;
    }
}