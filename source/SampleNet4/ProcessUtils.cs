
namespace SampleNet4
{

    internal class WindowsProcess
    {
        // Define an extension method for type System.Process that returns the command 
        // line via WMI.
        public static string GetCommandLine(System.Diagnostics.Process process)
        {
            string cmdLine = null;
            using (System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher(
              $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {process.Id}"))
            {
                // By definition, the query returns at most 1 match, because the process 
                // is looked up by ID (which is unique by definition).
                System.Management.ManagementObjectCollection.ManagementObjectEnumerator matchEnum = searcher.Get().GetEnumerator();
                if (matchEnum.MoveNext()) // Move to the 1st item.
                {
                    cmdLine = matchEnum.Current["CommandLine"]?.ToString();
                }
            }
            /*
            if (cmdLine == null)
            {
                // Not having found a command line implies 1 of 2 exceptions, which the
                // WMI query masked:
                // An "Access denied" exception due to lack of privileges.
                // A "Cannot process request because the process (<pid>) has exited."
                // exception due to the process having terminated.
                // We provoke the same exception again simply by accessing process.MainModule.
                var dummy = process.MainModule; // Provoke exception.
            }
            */
            return cmdLine;
        } // End Function GetCommandLine 


        /// <summary>
        /// Kill a process, and all of its children, grandchildren, etc.
        /// </summary>
        /// <param name="pid">Process ID.</param>
        public static void KillProcessAndChildren(int pid)
        {
            // Cannot close 'system idle process'.
            if (pid == 0)
            {
                return;
            }

            System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher
                    ("Select * From Win32_Process Where ParentProcessID=" + pid);

            System.Management.ManagementObjectCollection moc = searcher.Get();
            foreach (System.Management.ManagementObject mo in moc)
            {
                KillProcessAndChildren(System.Convert.ToInt32(mo["ProcessID"]));
            }

            try
            {
                System.Diagnostics.Process proc = System.Diagnostics.Process.GetProcessById(pid);
                proc.Kill();
            }
            catch (System.ArgumentException)
            {
                // Process already exited.
            }

        } // End Sub KillProcessAndChildren 


        // https://stackoverflow.com/questions/5901679/kill-process-tree-programmatically-in-c-sharp
        private static void EndProcessTree(string imageName)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "taskkill",
                Arguments = $"/im {imageName} /f /t",
                CreateNoWindow = true,
                UseShellExecute = false
            }).WaitForExit();
        } // End Sub EndProcessTree 


    } // End Class WindowsProcess


    internal class UnixProcess
    {


        public static string GetCommandLine(System.Diagnostics.Process process)
        {
            string file = $"/proc/{process.Id}/cmdline";

            if (!System.IO.File.Exists(file))
                return "";

            string commandLine = System.IO.File.ReadAllText(file, System.Text.Encoding.UTF8);

            if (string.IsNullOrEmpty(commandLine))
                return commandLine;

            commandLine = commandLine.Replace((char)0, ' ');
            commandLine = commandLine.Trim('\f', '\v', '\t', ' ', '\r', '\n');
            return commandLine;
        } // End Function GetCommandLine 


        public static void KillProcessAndChildren(int pid)
        {
            // https://stackoverflow.com/questions/392022/whats-the-best-way-to-send-a-signal-to-all-members-of-a-process-group
            // kill  -TERM -PID
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("kill", $"-TERM -{pid}"));
        } // End Sub KillProcessAndChildren 


    } // End Class UnixProcess


    internal class ProcessUtils
    {


        public static string GetCommandLine(System.Diagnostics.Process process)
        {
            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
                return UnixProcess.GetCommandLine(process);
            
            return WindowsProcess.GetCommandLine(process);
        } // End Function GetCommandLine


        public static void KillProcessAndChildren(int pid)
        {
            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
                UnixProcess.KillProcessAndChildren(pid);
            else
                WindowsProcess.KillProcessAndChildren(pid);
        } // End Sub KillProcessAndChildren 


        public static void KillProcessAndChildren(System.Diagnostics.Process proc)
        {
            KillProcessAndChildren(proc.Id);
        } // End Sub KillProcessAndChildren 


    } // End Class ProcessUtils


} // End Namespace SampleNet4 
