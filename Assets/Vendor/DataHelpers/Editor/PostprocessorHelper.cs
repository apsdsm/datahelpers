﻿using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using DataHelpers.Interfaces;


/// <summary>
/// A class that provides helper functions for post processors. It looks at an asset and decides which 
/// reader helpers to use to get the data into a raw bundle, then passes the bundle to the appropriate
/// importers and validators.
/// </summary>
public static class PostprocessorHelper
{

    /// <summary>
    /// Will either load an existing asset of the specified type, or create a new one.
    /// </summary>
    /// <typeparam name="T">Type of asset to create</typeparam>
    /// <param name="assetPath">where to load from or create</param>
    /// <returns>the scriptiable object that was loaded or created.</returns>
    private static ScriptableObject LoadOrCreateAsset<T>(string assetPath) where T : ScriptableObject
    {

        // get the file name
        string assetFileName = Path.GetFileName(assetPath);

        // remove file name
        if (assetPath.EndsWith(assetFileName)) {
            assetPath.TrimEnd(assetFileName.ToCharArray());
        }

        // if asset path contains the assets folder, remove that
        if (assetPath.StartsWith("Assets/")) {
            assetPath = assetPath.TrimStart("Assets/".ToCharArray());
        }

        // will hold the source folder name
        string sourceFolder = "";

        // read through the asset path to find the first folder name above 'Assets/'
        while (assetPath != "") {
            sourceFolder = assetPath;
            assetPath = Path.GetDirectoryName(assetPath);
        }

        // now let's reconstruct the path we need to make the asset in
        string destination = assetPath.Replace(sourceFolder, "Resources");

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
    /// Import the specified type of asset, running it through the specified importer and validator. This version does not have an explicit delcaration of field names.
    /// </summary>
    /// <typeparam name="TAsset">the type of asset to import</typeparam>
    /// <typeparam name="TImporter">the importer to use</typeparam>
    /// <typeparam name="TValidator">the validator to use</typeparam>
    /// <param name="asset">the location of the asset</param>
    public static void Import<TAsset, TImporter, TValidator>(string asset) where TAsset : ScriptableObject where TImporter : IImporter<TAsset> where TValidator : IValidator
    {
        ImportData rb = new ImportData();

        // if this is an excel file
        if (asset.EndsWith(".xlsx"))
        {
            // if this isn't a temp file
            if (!asset.Contains("~$"))
            {

                var reader = new ExcelReader();

                reader.ReadAsset(asset, ref rb);

                var validator = new ValidationRunner();

                // create a validator
                var userValidator = Activator.CreateInstance<TValidator>();

                // add parsable rows to validator
                validator.AddParsableRows(rb);

                // if passes validation, send to importer
                if (validator.IsValid(rb, userValidator))
                {
                    var so = LoadOrCreateAsset<TAsset>(asset);

                    var importer = Activator.CreateInstance<TImporter>();

                    importer.Import((TAsset)so, rb);

                    EditorUtility.SetDirty(so);
                }
            }
        }
    }
}