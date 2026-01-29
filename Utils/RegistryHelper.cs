using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace SupremeScreenSharePro.Utils
{
    public static class RegistryHelper
    {
        public static string[] GetSubKeyNames(RegistryKey root, string path)
        {
            try
            {
                using (var key = root.OpenSubKey(path))
                {
                    if (key != null)
                    {
                        return key.GetSubKeyNames();
                    }
                }
            }
            catch
            {
            }
            return new string[0];
        }

        public static object GetValue(RegistryKey root, string path, string valueName)
        {
            try
            {
                using (var key = root.OpenSubKey(path))
                {
                    if (key != null)
                    {
                        return key.GetValue(valueName);
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        public static Dictionary<string, object> GetValues(RegistryKey root, string path)
        {
            var values = new Dictionary<string, object>();
            try
            {
                using (var key = root.OpenSubKey(path))
                {
                    if (key != null)
                    {
                        foreach (var valueName in key.GetValueNames())
                        {
                            values[valueName] = key.GetValue(valueName);
                        }
                    }
                }
            }
            catch
            {
            }
            return values;
        }
    }
}
