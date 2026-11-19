using Microsoft.Win32;
using System;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WpfTool.Properties;

namespace WpfTool
{
    public partial class SetupWindow : Window
    {
        public SetupWindow()
        {
            InitializeComponent();
            EditorPath.Text = Settings.Default.EditorPath;
            P4Path.Text = Settings.Default.P4Path;
        }

        private void BrowseEditor_Click(object sender, RoutedEventArgs e)
        {
            BrowseForExecutable(EditorPath, "Choose a text editor", "Applications (*.exe)|*.exe");
        }

        private void BrowseP4_Click(object sender, RoutedEventArgs e)
        {
            BrowseForExecutable(P4Path, "Choose P4.exe", "Perforce CLI (p4.exe)|p4.exe|Applications (*.exe)|*.exe");
        }

        private void BrowseForExecutable(TextBox pathField, string title, string filter)
        {
            var dialog = new OpenFileDialog { Title = title, Filter = filter, CheckFileExists = true };
            string currentPath = Environment.ExpandEnvironmentVariables(pathField.Text.Trim().Trim('"'));
            if (File.Exists(currentPath))
                dialog.InitialDirectory = Path.GetDirectoryName(currentPath);

            if (dialog.ShowDialog(this) == true)
                pathField.Text = dialog.FileName;
        }

        private void UseNotepad_Click(object sender, RoutedEventArgs e)
        {
            EditorPath.Text = Path.Combine(Environment.SystemDirectory, "notepad.exe");
        }

        private void UseVsCode_Click(object sender, RoutedEventArgs e)
        {
            string[] candidates =
            {
                @"%LOCALAPPDATA%\Programs\Microsoft VS Code\Code.exe",
                @"%ProgramFiles%\Microsoft VS Code\Code.exe",
                @"%ProgramW6432%\Microsoft VS Code\Code.exe",
                @"%ProgramFiles(x86)%\Microsoft VS Code\Code.exe"
            };

            foreach (string candidate in candidates)
            {
                string executable = Environment.ExpandEnvironmentVariables(candidate);
                if (File.Exists(executable))
                {
                    EditorPath.Text = executable;
                    return;
                }
            }

            BrowseForExecutable(EditorPath, "Find Visual Studio Code (Code.exe)", "Visual Studio Code (Code.exe)|Code.exe");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var settings = Settings.Default;
            string previousEditor = settings.EditorPath;
            string previousP4 = settings.P4Path;
            string editor;
            string p4;

            if (!TryGetExecutable(EditorPath, previousEditor, "Choose a text editor executable.", out editor))
                return;
            if (!TryGetExecutable(P4Path, previousP4, "Choose the Perforce command-line executable (P4.exe).", out p4))
                return;

            settings.EditorPath = editor;
            settings.P4Path = p4;
            try
            {
                settings.Save();
            }
            catch (Exception ex) when (ex is ConfigurationErrorsException || ex is IOException || ex is UnauthorizedAccessException)
            {
                settings.EditorPath = previousEditor;
                settings.P4Path = previousP4;
                MessageBox.Show(this, "Could not save settings.\n\n" + ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
        }

        private bool TryGetExecutable(TextBox field, string previousPath, string message, out string path)
        {
            path = field.Text.Trim().Trim('"');

            // An unchanged path may refer to software that is temporarily unavailable.
            if (path == previousPath)
                return true;

            string executable = Environment.ExpandEnvironmentVariables(path);
            if (File.Exists(executable) && string.Equals(Path.GetExtension(executable), ".exe", StringComparison.OrdinalIgnoreCase))
            {
                path = Path.GetFullPath(executable);
                return true;
            }

            MessageBox.Show(this, message, Title, MessageBoxButton.OK, MessageBoxImage.Warning);
            field.Focus();
            return false;
        }
    }
}
