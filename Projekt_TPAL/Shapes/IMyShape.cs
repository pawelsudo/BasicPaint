using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Projekt_TPAL.Shapes
{
    public interface IMyShape
    {
        Canvas canvas { get; set; }
        Brush fill { get; set; }
        Brush stroke { get; set; }
        double strokeThickness { get; set; }

        void CreateShape(Point mousePoint);
        void Draw(Point mousePoint);
        List<Shape> EndDrawing();
    }
}
