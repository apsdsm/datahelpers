using UnityEngine;
using System;

/// <summary>
/// Attribute is used to specify the column names in an asset that is handled by a Data Importer.
/// </summary>
[AttributeUsage (AttributeTargets.Field)]
public class DIColName : System.Attribute {

	public readonly string Name;

	public DIColName(string name) {
		this.Name = name;
	}
}
