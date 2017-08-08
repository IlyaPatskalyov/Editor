using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Editor.Front
{
    public static class Loader
    {
        private static string GetBinPath()
        {
            var relativeSearchPath = AppDomain.CurrentDomain.RelativeSearchPath;
            if (string.IsNullOrEmpty(relativeSearchPath))
                return AppDomain.CurrentDomain.BaseDirectory;
            if (relativeSearchPath.Contains("ReSharperPlatform"))
                return AppDomain.CurrentDomain.BaseDirectory;
            return AppDomain.CurrentDomain.RelativeSearchPath;
        }

        public static Assembly[] LoadFromBinDirectory(string mask)
        {
            var locations = mask.Split('|').SelectMany(m => Directory.GetFiles(GetBinPath(), m, SearchOption.TopDirectoryOnly));

            var result = new List<Assembly>();
            foreach (var dllFile in locations)
            {
                AssemblyName assemblyName;
                try
                {
                    assemblyName = AssemblyName.GetAssemblyName(dllFile);
                    var assembly = Assembly.Load(assemblyName);
                    result.Add(assembly);
                }
                catch (BadImageFormatException)
                {
                    //Log.Information("Can't load file {dllFile}", dllFile);
                }
                //Log.Information("Assembly {0} loaded", assemblyName);
            }
            return result.ToArray();
        }
    }
}