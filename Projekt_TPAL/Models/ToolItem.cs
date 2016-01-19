using PluginInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_TPAL.Models
{
    public class ToolItem
    {
        public string Name { get; set; }
        public IPlugin Tool { get; set; }
    }
}
