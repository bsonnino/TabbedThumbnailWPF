using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace TabbedThumbnailWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int numTab = 0;
        public MainWindow()
        {
            InitializeComponent();
            tabControl.SelectionChanged += new SelectionChangedEventHandler(tabControl_SelectionChanged);
        }

        void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.Items.Count > 0 && e.AddedItems.Count > 0)
            {
                var grid = (Grid)((TabItem)e.AddedItems[0]).Content;
                if (TaskbarManager.Instance.TabbedThumbnail.GetThumbnailPreview(grid) != null)
                    TaskbarManager.Instance.TabbedThumbnail.SetActiveTab(grid);
            }
        }

        public static Vector GetOffset(Visual parent, Visual visual)
        {
            GeneralTransform ge = visual.TransformToVisual(parent);
            Point offset = ge.Transform(new Point(0, 0));
            return new Vector(offset.X, offset.Y);
        }

        Brush[] backBrushes = new Brush[] { Brushes.Red, Brushes.Blue, Brushes.Yellow, Brushes.Magenta, Brushes.AliceBlue, Brushes.Beige };

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TabItem item = new TabItem();
            item.Header = string.Format("Página {0}", ++numTab);
            Grid grid = new Grid();
            grid.Loaded += new RoutedEventHandler(grid_Loaded);
            grid.Background = backBrushes[numTab % 6];
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            item.Content = grid;
            tabControl.Items.Add(item);
            tabControl.SelectedItem = item;
        }

        void grid_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = (Grid)sender;
            if (TaskbarManager.Instance.TabbedThumbnail.GetThumbnailPreview(grid) == null)
            {
                var preview = new TabbedThumbnail(this, grid, GetOffset(tabControl, grid));
                TaskbarManager.Instance.TabbedThumbnail.AddThumbnailPreview(preview);
                preview.TabbedThumbnailActivated += new EventHandler<TabbedThumbnailEventArgs>(preview_TabbedThumbnailActivated);
                preview.TabbedThumbnailClosed += new EventHandler<TabbedThumbnailEventArgs>(preview_TabbedThumbnailClosed);
            }
        }

        void preview_TabbedThumbnailClosed(object sender, TabbedThumbnailEventArgs e)
        {
            tabControl.Items.Remove(((FrameworkElement)e.WindowsControl).Parent);
        }

        void preview_TabbedThumbnailActivated(object sender, TabbedThumbnailEventArgs e)
        {
            tabControl.SelectedItem = ((FrameworkElement)e.WindowsControl).Parent;
        }

    }
}
