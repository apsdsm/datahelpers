using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// A class that provides built in Validations that can be used on data before it's imported.
/// </summary>
class Validation {

    ///// <summary>
    ///// Checks to see if the specified resource exists. The resource must be derived from a scriptable object
    ///// </summary>
    ///// <typeparam name="T">tyep of the resource to look for</typeparam>
    ///// <param name="name">the name of the asset</param>
    ///// <param name="node">the node that is responsibel for the asset</param>
    ///// <returns>true if resource is valid, otherwise false</returns>
    //protected bool IsValidResource<T>(string name, ValidatorRow node) where T : ScriptableObject {

    //    ResourceValidation validation = null;
    //    ScriptableObject scriptable = null;
    //    bool status = true;
    //    Type type = typeof(T);

    //    try {

    //        // try find existing validation
    //        validation = resourceValidations.Find(i => i.resourceType == type && i.name == name);

    //        // if this validation hasn't been run before, run the validation
    //        if (validation == null) {

    //            // search for assets with the specific type
    //            string[] guids = AssetDatabase.FindAssets(name + " t:" + type.ToString());

    //            // don't allow if more than one asset with same type and name
    //            if (guids.Length > 1) {
    //                SetErrorMessage(node, "conflict - more than one instance of resource exists.");
    //                return false;
    //            }

    //            // don't allow if asset not found
    //            if (guids.Length <= 0) {
    //                SetErrorMessage(node, "resource of type " + type.ToString() + " with name " + name + " was not found in asset database.");
    //                return false;
    //            }

    //            // load the scriptable asset
    //            scriptable = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), type) as ScriptableObject;
    //        }
    //    }

    //    // if anything goes wrong, prime the values to reflect a false outcome
    //    catch {
    //        status = false;
    //        scriptable = null;
    //    }

    //    // if there is no validation yet assigned, create a new validation and add it to the queue
    //    if (validation == null) {
    //        validation = new ResourceValidation();
    //        validation.resourceType = type;
    //        validation.resource = scriptable;
    //        validation.valid = status;

    //        resourceValidations.Add(validation);
    //    }


    //    // add the scriptable object to the resources pile for this node
    //    if (scriptable != null && !node.resources.ContainsKey(name)) {
    //        node.resources.Add(name, scriptable);
    //    }


    //    // return final status
    //    return validation.valid;
    //}
}