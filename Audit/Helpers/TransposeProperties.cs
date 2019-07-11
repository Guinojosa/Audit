using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Audit.Helpers
{
    public class TransposeProperties<T>
    {
        public static void Transpose(ref T objectOrigin, ref T objectDestination)
        {
            IList<PropertyInfo> objProperties = objectOrigin.GetType().GetProperties();
            foreach (PropertyInfo prop in objProperties)
            {
                objectDestination.GetType().GetProperty(prop.Name).SetValue(objectDestination, prop.GetValue(objectOrigin));
            }
        }
    }
}