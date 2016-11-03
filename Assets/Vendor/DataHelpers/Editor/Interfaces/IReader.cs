using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IReader {

    /// <summary>
    /// Read an asset file
    /// </summary>
    /// <param name="assetPath"></param>
    void ReadAsset(string assetPath, ref ImportData bundle);


}