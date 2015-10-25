using UnityEngine;
using System;

/// <summary>
/// Attribute is used to specify a specific method in the importer that should be used to copy this value into an
/// asset. The method itself should take a string as its only parameter and return the type of value that goes in
/// the asset field.
/// </summary>
[AttributeUsage (AttributeTargets.Field)]
public class DICopyMethod : System.Attribute {
	
	public readonly string MethodName;
	
	public DICopyMethod(string methodName) {
		this.MethodName = methodName;
	}
}
