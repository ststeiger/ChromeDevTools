
using System.Diagnostics;


namespace SampleNet4
{

    public class ProcessData
    {
        public int Id;
        public string Name;
        public string PerformanceProcessName;
        public PerformanceCounter Counter;
    }

    class abc
    {

        public static void foo()
        {
            System.Diagnostics.Process[] processLise = System.Diagnostics.Process.GetProcesses();

            long peakSet = 0;

            foreach (System.Diagnostics.Process p in processLise)
            {
                if (!"chrome".Equals(p.ProcessName, System.StringComparison.InvariantCultureIgnoreCase)
                    && !"chromium".Equals(p.ProcessName, System.StringComparison.InvariantCultureIgnoreCase)
                    )
                    continue;

                long peak = p.PeakWorkingSet64 / 1024;
                long set = p.WorkingSet64 / 1024;
                long priv = p.PrivateMemorySize64 / 1024;


                peakSet += peak;
            }

            peakSet = peakSet / 1024;

            System.Console.WriteLine(peakSet);


            GetProcessList();
            
        }


        private static System.Collections.Generic.List<ProcessData> GetProcessList()
        {
            System.Collections.Generic.List<ProcessData> ls = new System.Collections.Generic.List<ProcessData>();

            PerformanceCounterCategory cat = new PerformanceCounterCategory("Process");

            string[] processes = cat.GetInstanceNames();
            foreach (string process in processes)
            {

                using (PerformanceCounter cnt = new PerformanceCounter("Process", "ID Process", process, true))
                {
                    try
                    {
                        int rautePos = process.IndexOf("#");
                        string sanitizedProcess = process;

                        if (rautePos != -1)
                            sanitizedProcess = sanitizedProcess.Substring(0, rautePos);

                        int val = (int)cnt.RawValue;
                        ls.Add(
                            new ProcessData()
                            {
                                Id = (int)cnt.RawValue,
                                Name = sanitizedProcess,
                                PerformanceProcessName= process,
                                Counter = cnt
                            }
                        );
                    }
                    catch (System.Exception exProcessTerminated)
                    {
                        System.Console.WriteLine(exProcessTerminated.Message);
                    }

                    

                }
            }

            ls.Sort(
                delegate (ProcessData a, ProcessData b)
                {
                    int c = string.Compare(a.Name, b.Name, true);
                    if (c != 0)
                        return c;

                    if (a.Id == b.Id)
                        return 0;

                    if (a.Id < b.Id)
                        return -1;

                    return 1;
                }
            );

            return ls;
        }


            private static string GetProcessInstanceName(int pid)
        {
            

            PerformanceCounterCategory cat = new PerformanceCounterCategory("Process");

            string[] instances = cat.GetInstanceNames();
            foreach (string instance in instances)
            {

                using (PerformanceCounter cnt = new PerformanceCounter("Process", "ID Process", instance, true))
                {
                    int val = (int)cnt.RawValue;
                    if (val == pid)
                    {
                        return instance;
                    }
                }
            }

            throw new System.Exception("Could not find performance counter " +
                "instance name for current process. This is truly strange ...");
        }


        public static void foobar()
        { 
            // https://docs.microsoft.com/en-us/previous-versions/windows/it-pro/windows-server-2003/cc780836(v=ws.10)
            string[] processCategories = new string[] {
                 "% Processor Time"
                ,"% Privileged Time"
                ,"% User Time"
                ,"Creating Process ID"
                ,"Elapsed Time"
                ,"Handle Count"
                ,"ID Process"
                ,"IO Data Bytes/sec"
                ,"IO Data Operations/sec"
                ,"IO Other Bytes/sec"
                ,"IO Other Operations/sec"
                ,"IO Read Bytes/sec"
                ,"IO Read Operations/sec"
                ,"IO Write Bytes/sec"
                ,"IO Write Operations/sec"
                ,"Page Faults/sec"
                ,"Page File Bytes"
                ,"Page File Bytes Peak"
                ,"Pool Paged Bytes"
                ,"Pool Nonpaged Bytes"
                ,"Priority Base"
                ,"Private Bytes"
                ,"Thread Count"
                ,"Virtual Bytes"
                ,"Virtual Bytes Peak"
                ,"Working Set"
                ,"Working Set Peak"
                ,"Working Set - Private"
            };

            GetProcessCounters();
            GetAllCounters("Process");
            PerformanceCounterCategory[] categories = PerformanceCounterCategory.GetCategories();

            

            System.Console.WriteLine(categories);


            //PerformanceCounterCPU.CategoryName = "Process";
            //PerformanceCounterCPU.CounterName = "% Processor Time";
            //PerformanceCounterCPU.InstanceName = proc.ProcessHandle.ProcessName;

            //PerformanceCounterMemory.CategoryName = "Process";
            //PerformanceCounterMemory.CounterName = "Working Set - Private";
            //PerformanceCounterMemory.InstanceName = proc.ProcessHandle.ProcessName;
        }

        private static void  GetProcessCounters()
        {
            PerformanceCounterCategory cat = new PerformanceCounterCategory("Process");

            System.Console.WriteLine("Category {0}", cat.CategoryName);
            try
            {

                string[] processNames = cat.GetInstanceNames();
                if (processNames != null && processNames.Length > 0)
                {
                    foreach (string thisProcessName in processNames)
                    {
                        //if (cat.CounterExists(thisProcessName))
                        //{
                        bool isFirst = true;

                        foreach (PerformanceCounter counter in cat.GetCounters(thisProcessName))
                        {
                            // System.Console.WriteLine("\tCounter Name {0} [{1}]", counter.CounterName, thisProcessName);

                            string counterName = counter.CounterName;
                            counterName = counterName.Replace(@"\", @"\\");




                            if (isFirst)
                                System.Console.WriteLine("\t \"{0}\"", counter.CounterName);
                            else
                                System.Console.WriteLine("\t,\"{0}\"", counter.CounterName);

                            isFirst = false;
                        }
                        //}
                    }
                }
                else
                {
                    foreach (PerformanceCounter counter in cat.GetCounters())
                    {
                        System.Console.WriteLine("\tCounter Name {0}", counter.CounterName);
                    }
                }
            }
            catch (System.Exception ex)
            {
                // NO COUNTERS
                System.Console.WriteLine(ex.Message);
            }
        }



            private static void GetAllCounters(string categoryFilter)
        {
            PerformanceCounterCategory[] categories = PerformanceCounterCategory.GetCategories();
            foreach (PerformanceCounterCategory cat in categories)
            {
                if (categoryFilter != null && categoryFilter.Length > 0)
                {
                    if (!cat.CategoryName.Contains(categoryFilter)) continue;
                }

                System.Console.WriteLine("Category {0}", cat.CategoryName);
                try
                {
                    string[] instances = cat.GetInstanceNames();
                    if (instances != null && instances.Length > 0)
                    {
                        foreach (string instance in instances)
                        {
                            //if (cat.CounterExists(instance))
                            //{
                            foreach (PerformanceCounter counter in cat.GetCounters(instance))
                            {
                                System.Console.WriteLine("\tCounter Name {0} [{1}]", counter.CounterName, instance);
                            }
                            //}
                        }
                    }
                    else
                    {
                        foreach (PerformanceCounter counter in cat.GetCounters())
                        {
                            System.Console.WriteLine("\tCounter Name {0}", counter.CounterName);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    // NO COUNTERS
                    System.Console.WriteLine(ex.Message);
                }
            }

            System.Console.ReadLine();
        }



    }
}
