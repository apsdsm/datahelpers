using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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