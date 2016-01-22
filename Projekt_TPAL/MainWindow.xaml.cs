﻿using Microsoft.Win32;
using PluginInterface;
using Projekt_TPAL.Helpers;
using Projekt_TPAL.Models;
using Projekt_TPAL.Shapes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Projekt_TPAL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isFileOpen = false;
        private bool isFileSave = false;
        private SaveHelper saveHelper = new SaveHelper();
        private PluginHelper pluginHelper = new PluginHelper();
        List<List<Shape>> history = new List<List<Shape>>();
        List<List<Shape>> removed = new List<List<Shape>>();
        List<ToolItem> tools = new List<ToolItem>();
        Canvas canvas;
        private Brush stroke;
        private Brush fill;


        //IMyShape myShape;
        IPlugin tool;

        public MainWindow()
        {
            CultureResources.ChangeCulture(new CultureInfo("en"));
            InitializeComponent();
            UpdateStatusBar();
            UpdateBtnColor();
            var plugins = pluginHelper.InitializePlugins();

            foreach (var plugin in plugins)
            {
                plugin.Initialize();
                tools.Add(new ToolItem() { Tool = plugin, Name = plugin.GetName(), BackgroundImg = CreateBitmapSourceFromBitmap(plugin.btnBackground) });
            }

            toolsListView.ItemsSource = tools;

            stroke = Brushes.Black;
            fill = Brushes.White;

        }

        public BitmapSource CreateBitmapSourceFromBitmap(System.Drawing.Bitmap bitmap)
        {

            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        #region MenuActions

        private void UndoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (history.Count() > 0)
            {
                foreach (var item in history.Last())
                {
                    canvas.Children.Remove(item);
                }
                removed.Add(history.Last());
                history.Remove(history.Last());
            }

        }

        private void RedoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            history.Add(removed.Last());
            removed.Remove(history.Last());
            foreach (var item in history.Last())
            {
                canvas.Children.Add(item);
            }
        }

        private void ChangeLanguageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            bool setPolishCulture = (sender == PolishMenuItem);

            CultureResources.ChangeCulture(new CultureInfo(setPolishCulture ? "pl" : "en"));
            PolishMenuItem.IsChecked = setPolishCulture;
            EnglishMenuItem.IsChecked = !setPolishCulture;
            UpdateStatusBar();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ClearDrawingArea();

            canvas = new Canvas();

            AddMouseEvents(canvas);

            canvas.Background = Brushes.White;
            canvas.Margin = new Thickness(2, 2, 2, 2);
            canvas.Width = 1280;
            canvas.Height = 768;

            pictureGrid.Children.Add(canvas);

            isFileOpen = true;
            isFileSave = false;
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ClearDrawingArea();
        }

        #endregion

        #region MouseEvents

        private void RemoveMouseEvents(Canvas canvas)
        {
            canvas.MouseMove -= Canvas_MouseMove;
            canvas.MouseUp -= Canvas_MouseUp;
            canvas.MouseDown -= Canvas_MouseDown;
        }

        private void AddMouseEvents(Canvas canvas)
        {
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseUp += Canvas_MouseUp;
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (tool != null)
            {
                var shapes = tool.EndDrawing();
                history.Add(shapes);
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tool != null)
                tool.CreateShape(Mouse.GetPosition(canvas));
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (tool != null)
                tool.Draw(Mouse.GetPosition(canvas));
        }

        #endregion

        #region ButtonEvents       

        private void toolBtn_Click(object sender, RoutedEventArgs e)
        {
            var toolItem = (ToolItem)((Button)sender).DataContext;
            
            if (toolItem != null)
            {
                var plugin = toolItem.Tool;
                tool = plugin.Initialize(canvas, stroke, 2, fill);

                UpdateBtnColor(toolItem);
            }
        }

        #endregion

        #region ColorPickersEvents

        private void strokeColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            stroke = new SolidColorBrush(strokeColorPicker.SelectedColor.Value);
            tool.SetStroke(stroke);
        }

        private void fillColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            fill = new SolidColorBrush(fillColorPicker.SelectedColor.Value);
            tool.SetFill(fill);
        }

        #endregion

        #region Helpers

        private void UpdateStatusBar()
        {
            DateTime dt = DateTime.Now;
            StatusBarLabel.Text = dt.ToString("d", Properties.Resources.Culture) + "    " +
                dt.ToString("t", Properties.Resources.Culture);
        }

        private void ClearDrawingArea()
        {
            if (isFileOpen && !isFileSave)
                SaveFileConfirmation();
            pictureGrid.Children.Clear();
            UpdateBtnColor();
            isFileOpen = false;
        }

        private bool SaveFileConfirmation()
        {
            if (MessageBox.Show(Properties.Resources.SaveConfirmation, Properties.Resources.Save, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                isFileSave = false;
                return false;
            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = ".png";
                saveFileDialog.Filter = "Image (.png)|*.png";
                if (saveFileDialog.ShowDialog() == true)
                {
                    saveHelper.SaveFile(canvas, saveFileDialog.FileName);

                    isFileSave = true;
                    return true;
                }
            }
            return false;
        }

        private void UpdateBtnColor(ToolItem toolItem = null)
        {
            foreach (var item in tools)
            {
                if (toolItem != null && toolItem.Name == item.Name)
                    item.ActiveToolColor = Brushes.Red;
                else
                    item.ActiveToolColor = Brushes.White;

                toolsListView.ItemsSource = null;
                toolsListView.ItemsSource = tools;

            }
        }




        #endregion       
    }
}
