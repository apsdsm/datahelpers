using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataHelpers {
    public class Field {
        public string value;

        /// <summary>
        /// Returns the field value converted to an int32.
        /// </summary>
        /// <returns>value as int</returns>
        public int AsInt {
            get {
                return Convert.ToInt32(value);
            }
        }


        /// <summary>
        /// Returns the field value conveted to a boolean.
        /// </summary>
        /// <returns>value as bool</returns>
        public bool AsBool {
            get {
                return Convert.ToBoolean(value);
            }
        }
    }
}