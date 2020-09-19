using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace FakerDTO
{
    class PluginLoader
    {
        public Dictionary<Type, IGenerator> RefreshPlugins()
        {
            var PluginDict = new Dictionary<Type, IGenerator>();

            string PluginPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
            DirectoryInfo PluginDirectory = new DirectoryInfo(PluginPath);
            if (!PluginDirectory.Exists)
                PluginDirectory.Create();

            string[] PluginFiles = Directory.GetFiles(PluginPath, "*.dll");
            foreach (string file in PluginFiles)
            {
                Assembly asm = Assembly.LoadFrom(file);
                Type[] types = asm.GetTypes();
                foreach (Type type in types)
                {
                    IGenerator plugin = asm.CreateInstance(type.FullName) as IGenerator;
                    if (Attribute.IsDefined(plugin.GetType(), typeof(NameAttribute)))
                    {
                        string attr = (Attribute.GetCustomAttribute(plugin.GetType(), typeof(NameAttribute)) as NameAttribute).GeneratorType;
                        Type GeneratorType = Type.GetType(attr);
                        if (GeneratorType != null && !PluginDict.ContainsKey(GeneratorType))
                            PluginDict.Add(GeneratorType, plugin);
                    }
                }
            }

            return PluginDict;
        }
    }
}
