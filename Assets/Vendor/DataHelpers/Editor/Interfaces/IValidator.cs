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
    public interface IValidator
    {
        void Validate(ValidatorNode node, Validator validator);
    }
}
