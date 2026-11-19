using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfTool.Controllers
{
    internal class Perforce
    {
		public string P4exe { get; set; }

        public Perforce() 
        { 
            P4exe = "C:\\Program Files\\Perforce\\P4.exe";
        }

        public string GetInfo()
        {
            return ExecuteP4("info");
        }

        public string Blame(string filename)
        {
            return ExecuteP4($" annotate -u {filename}");
        }

        private string ExecuteP4(string arguments)
        {
            var proc = new Process();
            proc.StartInfo.FileName = P4exe;
            proc.StartInfo.Arguments = arguments;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            string outPut = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            var exitCode = proc.ExitCode;
            proc.Close();
            return outPut.Replace(Environment.NewLine, "\n");
        }

    }
}
