using UnityEngine;
using UnityEditor;
using System;
using System.IO;

using DataHelpers.Readers;
using DataHelpers.Contracts;

namespace DataHelpers {

    /// <summary>
    /// A class that provides helper functions for post processors. It looks at an asset and decides which 
    /// reader helpers to use to get the data into a raw bundle, then passes the bundle to the appropriate
    /// importers and validators.
    /// </summary>
    public static class PostprocessorHelper {

        /// <summary>
        /// Import the specified type of asset, running it through the specified importer and validator. 
        /// This version does not have an explicit declaration of field names.
        /// </summary>
        /// <typeparam name="TAsset">the type of asset to import</typeparam>
        /// <typeparam name="TImporter">the importer to use</typeparam>
        /// <typeparam name="TValidator">the validator to use</typeparam>
        /// <param name="asset">the location of the asset</param>
        public static void Import<TAsset, TImporter, TValidator>(string asset) where TAsset : ScriptableObject 
                                                                               where TImporter : IImporter<TAsset> 
                                                                               where TValidator : IValidator {
            ImportData data = new ImportData();

            if (IsImportableExcelFile(asset)) {

                var reader = new ExcelReader();
                var validator = new ValidationRunner();
                var userValidator = Activator.CreateInstance<TValidator>();

                reader.ReadAsset(asset, ref data);

                if (validator.IsValid(data, userValidator)) {

                    var scriptableObject = LoadOrCreateAsset<TAsset>(asset);
                    var userImporter = Activator.CreateInstance<TImporter>();

                    userImporter.Import((TAsset)scriptableObject, data);

                    EditorUtility.SetDirty(scriptableObject);
                }
            }
        }


        /// <summary>
        /// Will return true if the filename matches an importable Excel file. Excel files are importable if:
        /// - they have an .xlsx extension
        /// - they don't contain ~$ in the filename (indicates a temp file)
        /// </summary>
        /// <param name="filename">file name to check</param>
        /// <returns>true if file is importable excel file</returns>
        private static bool IsImportableExcelFile(string filename) {
            return filename.EndsWith(".xlsx") && !filename.Contains("~$");
        }


        /// <summary>
        /// Will either load an existing asset of the specified type, or create a new one.
        /// </summary>
        /// <typeparam name="T">Type of asset to create</typeparam>
        /// <param name="assetPath">where to load from or create</param>
        /// <returns>the scriptiable object that was loaded or created.</returns>
        private static ScriptableObject LoadOrCreateAsset<T>(string assetPath) where T : ScriptableObject {

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
    }
}