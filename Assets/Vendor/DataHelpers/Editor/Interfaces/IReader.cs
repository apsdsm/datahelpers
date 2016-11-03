namespace DataHelpers.Contracts {

    /// <summary>
    /// Contract defines object that can act as a data reader, to get data from a source
    /// file to an ImportData object.
    /// </summary>
    public interface IReader {

        /// <summary>
        /// Read the data contained in an asset, and use it to create a data object.
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="data"></param>
        void ReadAsset(string assetPath, ref ImportData data);
    }
}