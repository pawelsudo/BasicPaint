using Microsoft.Win32;
using PluginInterface;
using Projekt_TPAL.Helpers;
using Projekt_TPAL.Models;
using Projekt_TPAL.Shapes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

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
        private int strokeThickness;
   
        IPlugin tool;

        private readonly BackgroundWorker worker = new BackgroundWorker();

        private void InitializePlugins()
        {
            tools = new List<ToolItem>();
            if (string.IsNullOrEmpty(pluginsDirectory))
                return;
            var plugins = pluginHelper.InitializePlugins(pluginsDirectory);

            if(plugins.Count() == 0)
                tools = null;

            foreach (var plugin in plugins)
            {
                plugin.Initialize();
                tools.Add(new ToolItem() { Tool = plugin, Name = plugin.GetName(), BackgroundImg = CreateBitmapSourceFromBitmap(plugin.btnBackground) });
            }

            toolsListView.ItemsSource = tools;
        }

        public MainWindow()
        {
            CultureResources.ChangeCulture(new CultureInfo("en"));
            InitializeComponent();
            //UpdateStatusBar();
            UpdateBtnColor();

            InitializePlugins();

            fillColorPicker.SelectedColor = Color.FromRgb(255, 255, 255);
            strokeColorPicker.SelectedColor = Color.FromRgb(0, 0, 0);
            strokeThicknessCB.SelectedIndex = 0;
            propertiesGrid.Visibility = Visibility.Hidden;
            toolsListView.IsEnabled = false;
            this.Closing += MainWindow_Closing;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.WorkerReportsProgress = true;
            closeMenuItem.IsEnabled = false;
            saveMenuItem.IsEnabled = false;

        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            saveProgressBar.Value = e.ProgressPercentage * 20;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            saveProgressBar.Value = 0;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 1; i <= 6; ++i)
            {
                if (i == 3 || i == 4)
                    Thread.Sleep(200);
                else
                    Thread.Sleep(1000);
                worker.ReportProgress(i);
            }

            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                    new Action(() => saveHelper.SaveFile(canvas, saveFileDialog.FileName)));
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
                SetUndoRedoEnabled();
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
            SetUndoRedoEnabled();
        }

        private void ChangeLanguageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            bool setPolishCulture = (sender == PolishMenuItem);

            CultureResources.ChangeCulture(new CultureInfo(setPolishCulture ? "pl" : "en"));
            PolishMenuItem.IsChecked = setPolishCulture;
            EnglishMenuItem.IsChecked = !setPolishCulture;
            //UpdateStatusBar();
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
            toolsListView.IsEnabled = true;
            closeMenuItem.IsEnabled = true;
            saveMenuItem.IsEnabled = true;
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (isFileOpen && !isFileSave)
                SaveFileConfirmation();
            isFileSave = false;
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ClearDrawingArea();
        }

        string pluginsDirectory = "Plugins";
        private void PluginsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                pluginsDirectory = folderBrowserDialog1.SelectedPath;
                InitializePlugins();
                propertiesGrid.Visibility = Visibility.Hidden;
                toolsListView.IsEnabled = false;
                tool = null;
            }

            if (tools != null && tools.Count() > 0)
                toolsListView.Visibility = Visibility.Visible;
            else
            {
                toolsListView.Visibility = Visibility.Hidden;
                MessageBox.Show(Properties.Resources.PluginsError, Properties.Resources.Plugins);
            }

        }

        private void HelpMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("", Properties.Resources.Help);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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
                SetUndoRedoEnabled();                
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
                tool = plugin.Initialize(canvas, stroke, strokeThickness, fill);

                propertiesGrid.Visibility = Visibility.Visible;
                HideFillClrPicker(tool);
                UpdateBtnColor(toolItem);
            }
        }

        #endregion

        #region ToolsEvents

        private void strokeColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            stroke = new SolidColorBrush(strokeColorPicker.SelectedColor.Value);
            if(tool!=null)
                tool.SetStroke(stroke);
        }

        private void fillColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            fill = new SolidColorBrush(fillColorPicker.SelectedColor.Value);
            if(tool!=null)
                tool.SetFill(fill);
        }

        private void strokeThicknessCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            strokeThickness = Convert.ToInt32(((ComboBoxItem)strokeThicknessCB.SelectedItem).Content);
            if(tool!=null)
                tool.SetThickness(strokeThickness);
        }

        #endregion

        #region Helpers

        private void SetUndoRedoEnabled()
        {
            if (history.Count() > 0)
                undoMenuItem.IsEnabled = true;
            else
                undoMenuItem.IsEnabled = false;

            if (removed.Count() > 0)
                redoMenuItem.IsEnabled = true;
            else
                redoMenuItem.IsEnabled = false;
        }

        //private void UpdateStatusBar()
        //{
        //    DateTime dt = DateTime.Now;
        //    StatusBarLabel.Text = dt.ToString("d", Properties.Resources.Culture) + "    " +
        //        dt.ToString("t", Properties.Resources.Culture);
        //}

        private void ClearDrawingArea()
        {
            if (isFileOpen && !isFileSave)
                SaveFileConfirmation();
            pictureGrid.Children.Clear();
            UpdateBtnColor();
            isFileOpen = false;
            toolsListView.IsEnabled = false;
            propertiesGrid.Visibility = Visibility.Collapsed;
            history.Clear();
            removed.Clear();
            undoMenuItem.IsEnabled = false;
            redoMenuItem.IsEnabled = false;
            closeMenuItem.IsEnabled = false;
            saveMenuItem.IsEnabled = false;
        }

        SaveFileDialog saveFileDialog;
        private bool SaveFileConfirmation()
        {
            if (MessageBox.Show(Properties.Resources.SaveConfirmation, Properties.Resources.Save, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                isFileSave = false;
                return false;
            }
            else
            {
                saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = ".png";
                saveFileDialog.Filter = "Image (.png)|*.png";
                if (saveFileDialog.ShowDialog() == true)
                {
                    saveProgressBar.Value = 0;
                    worker.RunWorkerAsync();
                    //saveHelper.SaveFile(canvas, saveFileDialog.FileName);

                    isFileSave = true;
                    return true;
                }
            }
            return false;
        }      

        private void UpdateBtnColor(ToolItem toolItem = null)
        {
            if(tools!=null)
            foreach (var item in tools)
            {
                if (toolItem != null && toolItem.Name == item.Name)
                    item.ActiveToolColor = Brushes.MediumVioletRed;
                else
                    item.ActiveToolColor = Brushes.White;

                toolsListView.ItemsSource = null;
                toolsListView.ItemsSource = tools;

            }
        }

        private void HideFillClrPicker(IPlugin tool)
        {
            if (!tool.canFill)
            {
                fillColorPicker.Visibility = Visibility.Collapsed;
                fillTB.Visibility = Visibility.Collapsed;
            }
            else
            {
                fillColorPicker.Visibility = Visibility.Visible;
                fillTB.Visibility = Visibility.Visible;
            }
        }

        #endregion

        
    }
}
