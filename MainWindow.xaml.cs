using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using WpfTool.Controllers;
using WpfTool.Properties;

namespace WpfTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Perforce P4 { get; }

        public MainWindow()
        {
            InitializeComponent();
            P4 = new Perforce();
            ContentRendered += OpenFileFromCommandLine;
        }

        private void UiExit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void UiOpen_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog(this) == true)
                Annotate(dialog.FileName);
        }

        private void UiGetInfo_Click(object sender, RoutedEventArgs e)
        {
            RunAndOpen(P4.GetInfo, "info");
        }

        private void UiSetup_Click(object sender, RoutedEventArgs e)
        {
            new SetupWindow { Owner = this }.ShowDialog();
        }

        private void OpenFileFromCommandLine(object sender, EventArgs e)
        {
            ContentRendered -= OpenFileFromCommandLine;
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && File.Exists(args[1]))
                Annotate(args[1]);
        }

        private void Annotate(string fileName)
        {
            RunAndOpen(() => P4.Blame(Path.GetFullPath(fileName)), "annotate");
        }

        private void RunAndOpen(Func<string> runCommand, string outputName)
        {
            string output;
            try
            {
                output = runCommand();
            }
            catch (Win32Exception ex)
            {
                UiOutput.Text = "Could not start Perforce. Check the P4.exe path in File > Setup.\n\n" + ex.Message;
                return;
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException || ex is ArgumentException || ex is UnauthorizedAccessException)
            {
                UiOutput.Text = ex.Message;
                return;
            }

            UiOutput.Text = output;

            string outputPath;
            try
            {
                string outputDirectory = Path.Combine(Path.GetTempPath(), "P4Annotate");
                Directory.CreateDirectory(outputDirectory);
                outputPath = Path.Combine(outputDirectory, outputName + "-" + Guid.NewGuid().ToString("N") + ".txt");
                File.WriteAllText(outputPath, output);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show(this, "Could not write the output file.\n\n" + ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var editor = new ProcessStartInfo
                {
                    FileName = Environment.ExpandEnvironmentVariables(Settings.Default.EditorPath),
                    Arguments = "\"" + outputPath + "\"",
                    UseShellExecute = false
                };
                using (Process.Start(editor))
                {
                    // The editor can continue running independently.
                }
            }
            catch (Exception ex) when (ex is Win32Exception || ex is InvalidOperationException || ex is ArgumentException)
            {
                MessageBox.Show(this,
                    "Could not open the text editor. Check the editor path in File > Setup.\n\nOutput file:\n" +
                    outputPath + "\n\n" + ex.Message,
                    Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
