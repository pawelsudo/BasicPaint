using PluginInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Projekt_TPAL.Models
{
    public class ToolItem
    {
        public string Name { get; set; }
        public IPlugin Tool { get; set; }
        public ImageSource BackgroundImg { get; set; }
    }
}
