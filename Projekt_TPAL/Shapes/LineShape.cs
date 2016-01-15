using Projekt_TPAL.Shapes;
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
    public class LineShape:IMyShape
    {
        private Line line;

        public Canvas canvas { get; set; }

        public Brush fill { get; set; }

        public Brush stroke { get; set; }

        public double strokeThickness { get; set; }

        public LineShape(Canvas canvas, Brush stroke, double strokeThickness)
        {
            this.canvas = canvas;
            this.stroke = stroke;
            this.strokeThickness = strokeThickness;
        }

        public void CreateShape(Point mousePoint)
        {
            line = new Line();
            line.Stroke = stroke;
            line.StrokeThickness = strokeThickness;
            line.X1 = line.X2 = mousePoint.X;
            line.Y1 = line.Y2 = mousePoint.Y;
            canvas.Children.Add(line);
        }

        public void Draw(Point mousePoint)
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
    }
}
