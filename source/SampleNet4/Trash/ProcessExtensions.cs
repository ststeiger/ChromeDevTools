
namespace SampleNet4.Trash
{


    public static class ProcessExtensions
    {

        public static System.Collections.Generic.Dictionary<int, int> GetAllProcessParentPids()
        {
            var childPidToParentPid = new System.Collections.Generic.Dictionary<int, int>();

            var processCounters = new System.Collections.Generic.SortedDictionary<string, System.Diagnostics.PerformanceCounter[]>();
            var category = new System.Diagnostics.PerformanceCounterCategory("Process");

            // As the base system always has more than one process running, 
            // don't special case a single instance return.
            var instanceNames = category.GetInstanceNames();
            foreach (string t in instanceNames)
            {
                try
                {
                    processCounters[t] = category.GetCounters(t);
                }
                catch (System.InvalidOperationException)
                {
                    // Transient processes may no longer exist between 
                    // GetInstanceNames and when the counters are queried.
                }
            }

            foreach (var kvp in processCounters)
            {
                int childPid = -1;
                int parentPid = -1;

                foreach (var counter in kvp.Value)
                {
                    if ("ID Process".CompareTo(counter.CounterName) == 0)
                    {
                        childPid = (int)(counter.NextValue());
                    }
                    else if ("Creating Process ID".CompareTo(counter.CounterName) == 0)
                    {
                        parentPid = (int)(counter.NextValue());
                    }
                }

                if (childPid != -1 && parentPid != -1)
                {
                    childPidToParentPid[childPid] = parentPid;
                }
            }

            return childPidToParentPid;
        } // End Function GetAllProcessParentPids 



        public static void ProcessTree()
        {
            System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            int currentProcessId = currentProcess.Id;
            System.Collections.Generic.Dictionary<int, int> childPidToParentPid = GetAllProcessParentPids();


            System.Console.WriteLine(currentProcess.ProcessName);

            while (childPidToParentPid.ContainsKey(currentProcessId))
            {
                currentProcessId = childPidToParentPid[currentProcessId];
                try
                {
                    currentProcess = System.Diagnostics.Process.GetProcessById(currentProcessId);
                }
                catch (System.Exception ex)
                {
                    break;
                }

                System.Console.Write(" - ");
                System.Console.WriteLine(currentProcess.ProcessName);
            }

        }


        private static string FindIndexedProcessName(int pid)
        {
            string processName = System.Diagnostics.Process.GetProcessById(pid).ProcessName;
            System.Diagnostics.Process[] processesByName = System.Diagnostics.Process.GetProcessesByName(processName);
            string processIndexdName = null;

            for (int index = 0; index < processesByName.Length; index++)
            {
                processIndexdName = index == 0 ? processName : processName + "#" + index;
                System.Diagnostics.PerformanceCounter processId =
                    new System.Diagnostics.PerformanceCounter("Process", "ID Process", processIndexdName);
                if ((int)processId.NextValue() == pid)
                {
                    return processIndexdName;
                }
            }

            return processIndexdName;
        }

        private static System.Diagnostics.Process FindPidFromIndexedProcessName(string indexedProcessName)
        {
            System.Diagnostics.PerformanceCounter parentId =
                new System.Diagnostics.PerformanceCounter("Process", "Creating Process ID", indexedProcessName);

            return System.Diagnostics.Process.GetProcessById((int)parentId.NextValue());
        }

        public static System.Diagnostics.Process Parent(this System.Diagnostics.Process process)
        {
            return FindPidFromIndexedProcessName(FindIndexedProcessName(process.Id));
        }


        public static void Test()
        {
            ProcessTree();
            System.Console.WriteLine(System.Diagnostics.Process.GetCurrentProcess().Parent().Id);
            System.Console.WriteLine(System.Diagnostics.Process.GetProcessById(System.Diagnostics.Process.GetCurrentProcess().Parent().Id).ProcessName);
        }

    }


}
