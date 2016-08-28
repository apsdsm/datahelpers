using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DataHelpers.Interfaces
{
    /// <summary>
    /// Contract defines functionality for any class that acts as importer.
    /// </summary>
    public interface IImporter<T> where T : ScriptableObject
    {
        void Import(T asset, ReadBundle readBundle);
    }
}
