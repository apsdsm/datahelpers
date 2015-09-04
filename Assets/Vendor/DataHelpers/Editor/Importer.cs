using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;


/// <summary>
/// Specifies the structure of any Importer class.
/// </summary>
public interface IImporter {
    void Import(ScriptableObject asset, ValidatorNode[] nodes);
}

/// <summary>
/// Base importer class. All importers should be derived from this class. Currently this only deals with Excel files, though it could be expanded to
/// deal with other file types.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Importer<T> : IImporter {

    MethodInfo copyMethod = null;

    /// <summary>
    /// Constructs a new validator.
    /// </summary>
    public Importer() {

        // get method info for a validation method (if one has been defined)
        copyMethod = GetType().GetMethod("CopyDataToAsset", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(T), typeof(ValidatorNode[]) }, null);

        if (copyMethod == null) {
            Debug.Log("no copy method defined");
        }
    }


    /// <summary>
    /// Is called when an asset import data is deemed valid. Will pass the data to the copy method defined in the inherited class.
    /// </summary>
    /// <param name="asset"></param>
    public void Import(ScriptableObject asset, ValidatorNode[] nodes) {

        Debug.Log("start import process now");

        if (copyMethod != null) {
            copyMethod.Invoke(this, new object[] { asset, nodes });    
        }

    }
}
