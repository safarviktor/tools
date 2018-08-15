using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Prism.Commands;
using Prism.Mvvm;

namespace VCompare
{
    public class MainWindowVM : BindableBase
    {
        private string _file1;
        private string _file2;
        private string _vsPath;

        public ICommand LaunchCommand { get; private set; }

        public MainWindowVM()
        {
            LaunchCommand = new DelegateCommand(this.LaunchCompareTool);
            VSPath = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE\devenv.exe";
        }
        
        private void LaunchCompareTool()
        {
            if (FileExists(File1) && FileExists(File2))
            {
                System.Diagnostics.Process.Start(VSPath, " /diff " + string.Join(" ", File1, File2));
            }
        }

        private bool FileExists(string file)
        {
            return !string.IsNullOrWhiteSpace(file) && System.IO.File.Exists(file);
        }

        public Brush File1BorderBrush => FileExists(File1) ? Brushes.Green : Brushes.Red;
        public Brush File2BorderBrush => FileExists(File2) ? Brushes.Green : Brushes.Red;
        public Brush VSPathBorderBrush => FileExists(VSPath) ? Brushes.Green : Brushes.Red;

        public string VSPath
        {
            get => _vsPath;
            set
            {
                _vsPath = value;
                RaisePropertyChanged(nameof(VSPath));
                RaisePropertyChanged(nameof(VSPathBorderBrush));
            }
        }

        public string File1
        {
            get => _file1;
            set
            {
                _file1 = value;
                RaisePropertyChanged(nameof(File1));
                RaisePropertyChanged(nameof(File1BorderBrush));
            }
        }

        public string File2
        {
            get => _file2;
            set
            {
                _file2 = value;
                RaisePropertyChanged(nameof(File2));
                RaisePropertyChanged(nameof(File2BorderBrush));
            }
        }

        public void HandleFileDrop(List<string> files, int index)
        {
            if (files.Any())
            {
                if (files.Count > 1)
                {
                    var fi = new FileInfo(files[0]);
                    File1 = fi.FullName;
                    fi = new FileInfo(files[1]);
                    File2 = fi.FullName;
                }
                else
                {
                    var fi = new FileInfo(files[0]);
                    if (index == 1)
                    {
                        File1 = fi.FullName;
                    }
                    else
                    {
                        File2 = fi.FullName;
                    }
                }
            }
        }
    }
}