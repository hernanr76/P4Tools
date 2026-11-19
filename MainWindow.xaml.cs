using Microsoft.Win32;
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
using WpfTool.Controllers;

namespace WpfTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Perforce P4 { get; }
        private string[] Args { get; }
        public MainWindow()
        {
            InitializeComponent();
            P4 = new Perforce();
            Args = Environment.GetCommandLineArgs();

            OpenFielFromCommandLine();

        }

        private void UiExit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void UiOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                UiOutput.Text = P4.Blame(openFileDialog.FileName);
        }

        private void UiGetInfo_Click(object sender, RoutedEventArgs e)
        {
            UiOutput.Text = P4.GetInfo();
        }

        private void OpenFielFromCommandLine()
        { 
            if (Args.Length > 1 && File.Exists(Args[1])) 
            {
                UiOutput.Text = P4.Blame(Args[1]);
            }
        }
    }
}
