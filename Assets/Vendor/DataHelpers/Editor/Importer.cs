using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;



/// <summary>
/// Base importer class. All importers should be derived from this class. Currently this only deals with Excel files, though it could be expanded to
/// deal with other file types.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Importer<T> {

    MethodInfo copyMethod = null;

    /// <summary>
    /// Constructs a new validator.
    /// </summary>
    public Importer() 
	{
        // get method info for a validation method (if one has been defined)
		copyMethod = GetType().GetMethod("CopyDataToAsset", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(T), typeof(ReadBundle) }, null);
    }


    /// <summary>
    /// Is called when an asset import data is deemed valid. Will pass the data to the copy method defined in the inherited class.
    /// </summary>
    /// <param name="asset"></param>
    public void Import(ScriptableObject asset, ReadBundle readBundle) {
        if (copyMethod != null) 
		{
            copyMethod.Invoke(this, new object[] { asset, readBundle });    
        }
    }


	/// <summary>
	/// Helper function to take a read bundle, and copy the validated nodes into a list of assets, based on the
	/// assumption that each item is itself a new asset.
	/// </summary>
	/// <param name="readBundle">Read bundle.</param>
	/// <param name="list">List.</param>
	/// <typeparam name="TAsset">The 1st type parameter.</typeparam>
	public void CopyReadBundleIntoList<TAsset>( ReadBundle readBundle, ref List<TAsset> list )
	{
		System.Reflection.MemberInfo info = typeof( TAsset );

		// this dictionary will map valid field names to asset fields
		Dictionary<string, FieldInfo> map = new Dictionary<string, FieldInfo>();

		// get fields in type
		FieldInfo[] fields = typeof( TAsset ).GetFields();

		// for each field in type
		for ( int i = 0; i < fields.Length; i++ )
		{
			FieldInfo fieldInfo = fields[i];

			// get attributes that conform to the DIColName type
			object[] attributes = fieldInfo.GetCustomAttributes(typeof(DIColName), true);

			// if there were any hits
			if ( attributes.Length > 0 ) 
			{

				// cast the attribute to make it easier to work with
				DIColName colname = (DIColName)attributes[0];

				// if the column name is contained by the fieldnames add it to the map
				if (readBundle.fieldNames.Contains( colname.Name ))
				{
					map.Add(colname.Name, fieldInfo);
				}
			}
		}

		// refresh the list
		list = new List<TAsset>();

		// for each node that was validated
		foreach (ValidatorNode node in readBundle.validatedNodes)
		{

			// make a new instance of the asset
			TAsset asset = Activator.CreateInstance<TAsset>();

			// for each field name stored in the read bundle
			foreach (string fieldName in readBundle.fieldNames) 
			{

				// try get the mapped field info object
				FieldInfo fieldInfo = map[fieldName];

				// if there is a match
				if (fieldInfo != null)
				{

					// check to see what kind of data we're getting
					switch (fieldInfo.FieldType.ToString()) {
					
					// if it's a string - just copy the value across
					case "System.String":
						fieldInfo.SetValue(asset, node[fieldName]);
						break;

					// if it's a single, then copy as a float
					case "System.Single":
						fieldInfo.SetValue(asset, node.AsFloat(fieldName));
						break;

					// if it's something we can't handle automatically, check for a copy method and if one is available use that
					default:
						object[] attributes = fieldInfo.GetCustomAttributes(typeof(DICopyMethod), true);

						if ( attributes.Length > 0 ) 
						{
							string fieldcopyMethodName = ((DICopyMethod)attributes[0]).MethodName;
							MethodInfo fieldCopyMethod = GetType().GetMethod( fieldcopyMethodName, BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(string), typeof(TAsset) }, null );

							if (fieldCopyMethod != null) 
							{
								fieldCopyMethod.Invoke( this, new object[] { node[fieldName], asset } );
							}
						}

						break;

					}
				}
			}

			// add asset to the list
			list.Add(asset);
		}
	}
}
