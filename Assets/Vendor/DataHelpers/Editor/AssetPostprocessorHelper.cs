using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;


/// <summary>
/// A single parsable row of a file stored using tabular data
/// </summary>
public class ParsableRow {
    public string[] cells;
    public int linenumber;
}

/// <summary>
/// A bundle of the things required to import all the data in a given file to the game.
/// </summary>
public class ReadBundle {
    public Dictionary<string, string> meta = new Dictionary<string, string>();
    public Dictionary<string, string> vars = new Dictionary<string, string>();
    public List<ParsableRow> rows = new List<ParsableRow>();
}


/// <summary>
/// A class that provides helper functions for post processors. It looks at an asset an decides which reader helpers to use to get the data into a raw bundle, then it 
/// passes the bundle to the appropriate importers and validators.
/// </summary>
public static class PostprocessorHelper {
    
  

    public static ScriptableObject LoadOrCreateAsset<T>(string path) where T : ScriptableObject {

        // get the file name
        string assetFileName = Path.GetFileName(path);

        // copy asset path
        string reading = path;

        // remove file name
        if (reading.EndsWith(assetFileName)) {
            reading.TrimEnd(assetFileName.ToCharArray());
        }

        // if asset path contains the assets folder, remove that
        if (reading.StartsWith("Assets/")) {
            reading = reading.TrimStart("Assets/".ToCharArray());
        }

        // will hold the source folder name
        string sourceFolder = "";

        // read through the asset path to find the first folder name above 'Assets/'
        while (reading != "") {
            sourceFolder = reading;
            reading = Path.GetDirectoryName(reading);
        }

        // now let's reconstruct the path we need to make the asset in
        string destination = path.Replace(sourceFolder, "Resources");


        // get the last extension
        string extension = Path.GetExtension(destination);

        // replace the extension
        destination = destination.Replace(extension, ".asset");
        
        // now get the specific destination folder

        string destinationFolder = Path.GetDirectoryName(destination);

        // try create the target directory
        Directory.CreateDirectory(destinationFolder);

        ScriptableObject a = AssetDatabase.LoadAssetAtPath(destination, typeof(T)) as ScriptableObject;

        if (a == null) {
            a = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(a, destination);
        }

        return a;

    }

    /// <summary>
    /// Import the specified type of asset, running it through the specified importer and validator.
    /// </summary>
    /// <typeparam name="TAsset">the type of asset to import</typeparam>
    /// <typeparam name="TImporter">the importer to use</typeparam>
    /// <typeparam name="TValidator">the validator to use</typeparam>
    /// <param name="asset">the location of the asset</param>
    public static void Import<TAsset, TImporter, TValidator>(string asset, string[] fields) where TAsset : ScriptableObject where TImporter : IImporter where TValidator : IValidator {

        ReadBundle rb = new ReadBundle();

        // if this is an excel file
        if (asset.EndsWith(".xlsx")) {

            // check if it's a temp file
            if (asset.Contains("~$")) {
                Debug.Log("looks like a temp file arrived: " + asset);
            }

            // otherwise import this asset
            else {

                // read using Excel
                ExcelReader.ReadXLSX(asset, ref rb);

                // validate using Timeline validator
                TValidator validator = Activator.CreateInstance<TValidator>();

                validator.AddParsableRows(rb.rows, fields);

                if (validator.IsValid()) {

                    ScriptableObject so = LoadOrCreateAsset<TAsset>(asset);

                    TImporter importer = Activator.CreateInstance<TImporter>();

                    importer.Import(so, validator.Nodes);

                    EditorUtility.SetDirty(so);
                }
            }
        }
    }
}
