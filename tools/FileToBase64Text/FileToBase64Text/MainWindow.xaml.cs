
using System;
using System.Windows;
using Microsoft.Win32;

namespace FileToBase64Text
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnFilePath_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                SelectedFilePath.Text = openFileDialog.FileName;
                var bytes = System.IO.File.ReadAllBytes(openFileDialog.FileName);
                Base64Result.Text = Convert.ToBase64String(bytes);
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (Base64Input.Text != string.Empty)
            {
                var bytes = Convert.FromBase64String(Base64Input.Text);
                SaveFileDialog save = new SaveFileDialog();
                if (save.ShowDialog() == true)
                {
                    System.IO.File.WriteAllBytes(save.FileName, bytes);
                    MessageBox.Show("Success!");
                }
            }
        }
    }
}
