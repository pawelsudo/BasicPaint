using PluginInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Plugin.LineTool
{
    public class LineTool : IPlugin
    {
        public System.Drawing.Bitmap btnBackground { get; set; }

        public Canvas canvas { get; set; }

        public System.Windows.Media.Brush fill { get; set; }

        public Brush stroke { get; set; }

        public double strokeThickness { get; set; }

        private string name;

        private Line line;

        public void CreateShape(System.Windows.Point mousePoint)
        {
            line = new Line();
            line.Stroke = stroke;
            line.StrokeThickness = strokeThickness;
            line.X1 = line.X2 = mousePoint.X;
            line.Y1 = line.Y2 = mousePoint.Y;
            canvas.Children.Add(line);
        }      

        public void Draw(System.Windows.Point mousePoint)
        {
            if (line != null)
            {
                (canvas.Children[canvas.Children.IndexOf(line)] as Line).X2 = mousePoint.X;
                (canvas.Children[canvas.Children.IndexOf(line)] as Line).Y2 = mousePoint.Y;
            }
        }

        public List<Shape> EndDrawing()
        {
            var shapes = new List<Shape>();
            shapes.Add(line);
            line = null;

            return shapes;
        }

        public void Initialize()
        {
            this.name = "Line";
            this.btnBackground = Resource1.line;
        }

        public IPlugin Initialize(Canvas canvas, Brush stroke, double strokeThickness, Brush fill = null)
        {
            this.canvas = canvas;
            this.stroke = stroke;
            this.strokeThickness = strokeThickness;

            return this;
        }

        public string GetName()
        {
            return name;
        }

        public void Dispose()
        {

        }

        public void SetStroke(Brush stroke)
        {
            this.stroke = stroke;
        }

        public void SetFill(Brush fill)
        {
            this.fill = fill;
        }

        public void SetThickness(int thickness)
        {
            this.strokeThickness = thickness;
        }

    }
}
