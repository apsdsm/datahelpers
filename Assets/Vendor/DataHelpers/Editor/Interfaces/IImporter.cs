using UnityEngine;

namespace DataHelpers.Contracts
{
    /// <summary>
    /// Contract defines functionality for any class that acts as importer.
    /// </summary>
    public interface IImporter<T> where T : ScriptableObject
    {
        /// <summary>
        /// Must import the data contained in the data object into the provided asset.
        /// </summary>
        /// <param name="asset">asset into which data will be imported</param>
        /// <param name="data">source of data to import</param>
        void Import(T asset, ImportData data);
    }
}
