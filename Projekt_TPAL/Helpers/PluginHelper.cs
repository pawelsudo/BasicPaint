using PluginInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_TPAL.Helpers
{
    public class PluginHelper
    {
        private List<Assembly> GetAssemblies(string directory)
        {
            var assemblies = new List<Assembly>();
            if (Directory.Exists(directory))
            {
                foreach (var file in Directory.GetFiles(directory, "*.dll"))
                {
                    assemblies.Add(Assembly.LoadFrom(file));
                }
            }
            return assemblies;
        }

        public List<IPlugin> InitializePlugins()
        {
            var assemblies = GetAssemblies("Plugins");

            List<IPlugin> plugins = new List<IPlugin>();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if (type.IsClass && type.IsPublic && type.GetInterface(typeof(IPlugin).FullName) != null)
                    {
                        var item = Activator.CreateInstance(type) as IPlugin;
                        plugins.Add(item);
                    }
                }
            }

            return plugins;
        }
    }
}
