using System;
using System.Collections.Generic;
using System.Text;

namespace ShinyBleApp
{
    public static class Helper
    {
        public static string GetEnumName<T>(T v)
        {
            string name = "null";
            try
            {
                name = Enum.GetName(typeof(T), v);
            }
            catch (Exception) { }

            return name;
        }
    }
}
