using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Projekt_TPAL
{
    public class CultureResources
    {
        private static ObjectDataProvider resourceProvider =
            (ObjectDataProvider)App.Current.FindResource("Resources");

        public Properties.Resources GetResourceInstance()
        {
            return new Properties.Resources();
        }

        public static void ChangeCulture(CultureInfo culture)
        {
            Properties.Resources.Culture = culture;
            resourceProvider.Refresh();
        }
    }
}
