using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WinPathTool
{
    internal static class ProcessManager
    {
        static public Process Source { get; } = Process.GetCurrentProcess();

        static public bool RelaunchAsElevated(string arguments = "")
        {
            try
            {
                bool status;

                using (var process = new Process())
                {
                    string filename = Source.MainModule.FileName;
                    string dirname = Path.GetDirectoryName(filename);

                    process.StartInfo.FileName = "cmd";
                    process.StartInfo.WorkingDirectory = dirname;
                    process.StartInfo.Arguments = $"/d /c \"cd {dirname}\" && echo [Relauncing process as administrator] && echo. && {filename} {arguments} && echo. && echo Args: {arguments} && pause";
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.Verb = "runas";

                    status = process.Start();
                    process.WaitForExit();
                }

                return status;
            }
            catch
            {
                throw;
            }
        }

        [DllImport("shell32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static public extern bool IsUserAnAdmin();
    }
}
