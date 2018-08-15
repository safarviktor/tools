using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

namespace VCompare
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = new MainWindowVM();
            InitializeComponent();
        }

        private void F1_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var data = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (data != null && data.Any())
                {
                    var vm = ((MainWindowVM)this.DataContext);
                    vm.HandleFileDrop(data.ToList(), 1);
                    return;
                }
            }
        }

        private void F2_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var data = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (data != null && data.Any())
                {
                    var vm = ((MainWindowVM)this.DataContext);
                    vm.HandleFileDrop(data.ToList(), 2);
                    return;
                }
            }
        }
    }
}
