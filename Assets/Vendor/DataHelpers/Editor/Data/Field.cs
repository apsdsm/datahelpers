using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace DataHelpers {
    public class Field {
        public string value;

        /// <summary>
        /// Returns the field value converted to an int32.
        /// </summary>
        public int AsInt {
            get {
                return Convert.ToInt32(value);
            }
        }


        /// <summary>
        /// Returns the field value conveted to a boolean.
        /// </summary>
        public bool AsBool {
            get {
                return Convert.ToBoolean(value);
            }
        }


        /// <summary>
        /// Returns the field value converted to a float.
        /// </summary>
        public float AsFloat {
            get {
                return Convert.ToSingle(value);
            }
        }


        /// <summary>
        /// Returns the field value converted to a double.
        /// </summary>
        public double AsDouble {
            get {
                return Convert.ToDouble(value);
            }
        }


        /// <summary>
        /// Returns true if value is null or empty string, otherwise true.
        /// </summary>
        /// <returns>true if field is empty</returns>
        public bool IsEmpty() {
            return (value == "" || value == null);
        }


        /// <summary>
        /// Returns true if the value contains name of valid resource.
        /// </summary>
        /// <returns></returns>
        public bool IsValidResource() {

            string[] guids = AssetDatabase.FindAssets(value);

            if (guids.Length <= 0) {
                return false;
            }

            return true;
        }
    }
}