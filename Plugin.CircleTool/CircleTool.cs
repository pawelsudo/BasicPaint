﻿using PluginInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Plugin.CircleTool
{
    public class CircleTool : IPlugin
    {
        private Ellipse circle;
        public Canvas canvas { get; set; }    
        public Brush fill { get; set; }      
        public Brush stroke { get; set; }
        public double strokeThickness { get; set; }
        public System.Drawing.Bitmap btnBackground { get; set; }
        public bool canFill { get; set; }
        private string name;

        public void Initialize()
        {
            this.name = "Circle";
            this.btnBackground = Resource1.circle;
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
            canFill = true;

            return this;
        }

        public void CreateShape(Point mousePoint)
        {
            circle = new Ellipse();
            if (fill != null)
                circle.Fill = fill;
            circle.Stroke = stroke;
            circle.StrokeThickness = strokeThickness;
            circle.Margin = new Thickness(mousePoint.X, mousePoint.Y, 0, 0);

            canvas.Children.Add(circle);
        }

        public void Draw(Point mousePoint)
        {
            if (circle != null)
            {
                var width = mousePoint.X - circle.Margin.Left;
                var height = mousePoint.Y - circle.Margin.Top;

                circle.Width = width > 0 ? width : 1;
                circle.Height = height > 0 ? height : 1;
            }
        }

        public List<Shape> EndDrawing()
        {
            var shapes = new List<Shape>();
            shapes.Add(circle);
            circle = null;

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
