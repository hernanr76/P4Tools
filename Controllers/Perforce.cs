using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WpfTool.Properties;

namespace WpfTool.Controllers
{
    internal class Perforce
    {
        public string GetInfo()
        {
            return ExecuteP4("info");
        }

        public string Blame(string filename)
        {
            return ExecuteP4("annotate -u \"" + filename + "\"");
        }

        private string ExecuteP4(string arguments)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = Environment.ExpandEnvironmentVariables(Settings.Default.P4Path),
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var process = Process.Start(startInfo))
            {
                Task<string> readError = process.StandardError.ReadToEndAsync();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                string error = readError.GetAwaiter().GetResult();

                if (process.ExitCode != 0)
                {
                    string details = string.IsNullOrWhiteSpace(error) ? output : error;
                    throw new InvalidOperationException(
                        "Perforce exited with code " + process.ExitCode + ".\n\n" + details.Trim());
                }

                return output;
            }
        }
    }
}
