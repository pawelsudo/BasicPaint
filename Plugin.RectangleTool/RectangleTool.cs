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

namespace Plugin.RectangleTool
{
    public class RectangleTool:IPlugin
    {
        private Rectangle rectangle;
        public Canvas canvas { get; set; }
        public Brush fill { get; set; }
        public Brush stroke { get; set; }
        public double strokeThickness { get; set; }
        public System.Drawing.Bitmap btnBackground { get; set; }
        public bool canFill { get; set; }
        private string name;

        public void Initialize()
        {
            this.name = "Rectangle";
            this.btnBackground = Resource1.rectangle;
        }

        public string GetName()
        {
            return name;
        }

        public IPlugin Initialize(Canvas canvas, Brush stroke, double strokeThickness, Brush fill = null)
        {
            this.canvas = canvas;
            this.fill = fill;
            this.stroke = stroke;
            this.strokeThickness = strokeThickness;
            this.canFill = true;

            return this;
        }

        public void CreateShape(Point mousePoint)
        {
            rectangle = new Rectangle();
            if (fill != null)
                rectangle.Fill = fill;
            rectangle.Stroke = stroke;
            rectangle.StrokeThickness = strokeThickness;
            rectangle.Margin = new Thickness(mousePoint.X, mousePoint.Y, 0, 0);

            canvas.Children.Add(rectangle);
        }

        public void Draw(Point mousePoint)
        {
            if (rectangle != null)
            {
                var width = mousePoint.X - rectangle.Margin.Left;
                var height = mousePoint.Y - rectangle.Margin.Top;
                rectangle.Width = width > 0 ? width : 1;
                rectangle.Height = height > 0 ? height : 1;
            }
        }

        public List<Shape> EndDrawing()
        {
            var shapes = new List<Shape>();
            shapes.Add(rectangle);
            rectangle = null;

            return shapes;
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
