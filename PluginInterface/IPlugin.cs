using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace PluginInterface
{
    public interface IPlugin:IDisposable
    {
        //MenuItem GetMenuItem();

        void SetStroke(Brush stroke);

        void SetFill(Brush fill);

        void SetThickness(int thickness);  

        string GetName();
        void Initialize();
        IPlugin Initialize(Canvas canvas, Brush stroke, double strokeThickness, Brush fill = null);      
        void CreateShape(Point mousePoint);
        void Draw(Point mousePoint);
        List<Shape> EndDrawing();


        //string name { get; set; }
        Canvas canvas { get; set; }
        Brush fill { get; set; }
        Brush stroke { get; set; }
        double strokeThickness { get; set; }
        System.Drawing.Bitmap btnBackground { get; set; }

    }
}
