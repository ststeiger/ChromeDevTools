
using MasterDevs.ChromeDevTools.Protocol.Chrome.Page;
using MasterDevs.ChromeDevTools.Protocol.Chrome.DOM;
using Task = System.Threading.Tasks.Task;


namespace MasterDevs.ChromeDevTools.Sample
{


    internal class Program
    {
        const int ViewPortWidth = 1440;
        const int ViewPortHeight = 900;


        public static double cm2inch(double centimeters)
        {
            return centimeters * 0.393701;
        }


        public static void KillHeadless()
        {
            System.Diagnostics.Process[] allProcesses = System.Diagnostics.Process.GetProcesses();

            for (int i = 0; i < allProcesses.Length; ++i)
            {
                var proc = allProcesses[i];
                string commandLine = ProcessUtils.GetCommandLine(proc); // GetCommandLineOfProcess(proc);

                if (string.IsNullOrEmpty(commandLine))
                    continue;

                commandLine = commandLine.ToLowerInvariant();

                if (commandLine.IndexOf(@"\chrome.exe") == -1)
                    continue;

                if (commandLine.IndexOf(@"--headless") != -1)
                {

                    System.Console.WriteLine($"Killing process {proc.Id} with command line \"{commandLine}\"");
                    ProcessUtils.KillProcessAndChildren(proc.Id);
                }

            } // Next i 

            System.Console.WriteLine($"Finished killing headless chromes");
        } // End Sub KillHeadless 


        private static void Main(string[] args)
        {
            KillHeadless();

            Task.Run(async () =>
            {
                // synchronization
                System.Threading.ManualResetEventSlim screenshotDone = new System.Threading.ManualResetEventSlim();

                // STEP 1 - Run Chrome
                IChromeProcessFactory chromeProcessFactory = new ChromeProcessFactory(new StubbornDirectoryCleaner());
                using (IChromeProcess chromeProcess = chromeProcessFactory.Create(9222, true))
                {

                    // STEP 2 - Create a debugging session
                    //ChromeSessionInfo sessionInfo = (await chromeProcess.GetSessionInfo()).LastOrDefault();
                    ChromeSessionInfo[] sessionInfos = await chromeProcess.GetSessionInfo();
                    ChromeSessionInfo sessionInfo = (sessionInfos != null && sessionInfos.Length > 0) ?
                          sessionInfos[sessionInfos.Length - 1]
                        : new ChromeSessionInfo();

                    IChromeSessionFactory chromeSessionFactory = new ChromeSessionFactory();
                    IChromeSession chromeSession = chromeSessionFactory.Create(sessionInfo.WebSocketDebuggerUrl);

                    // STEP 3 - Send a command
                    //
                    // Here we are sending a commands to tell chrome to set the viewport size 
                    // and navigate to the specified URL
                    await chromeSession.SendAsync(new SetDeviceMetricsOverrideCommand
                    {
                        Width = ViewPortWidth,
                        Height = ViewPortHeight,
                        Scale = 1
                    });

                    var navigateResponse = await chromeSession.SendAsync(new NavigateCommand
                    {
                        Url = "http://www.google.com"
                    });
                    System.Console.WriteLine("NavigateResponse: " + navigateResponse.Id);

                    // STEP 4 - Register for events (in this case, "Page" domain events)
                    // send an command to tell chrome to send us all Page events
                    // but we only subscribe to certain events in this session
                    ICommandResponse pageEnableResult = await chromeSession.SendAsync<Protocol.Chrome.Page.EnableCommand>();
                    System.Console.WriteLine("PageEnable: " + pageEnableResult.Id);

                    chromeSession.Subscribe<LoadEventFiredEvent>(loadEventFired =>
                    {
                        // we cannot block in event handler, hence the task
                        Task.Run(async () =>
                        {
                            System.Console.WriteLine("LoadEventFiredEvent: " + loadEventFired.Timestamp);

                            long documentNodeId = (await chromeSession.SendAsync(new GetDocumentCommand())).Result.Root.NodeId;
                            long bodyNodeId =
                                (await chromeSession.SendAsync(new QuerySelectorCommand
                                {
                                    NodeId = documentNodeId,
                                    Selector = "body"
                                })).Result.NodeId;

                            long height = (await chromeSession.SendAsync(new GetBoxModelCommand {NodeId = bodyNodeId})).Result.Model.Height;

                            await chromeSession.SendAsync(new SetDeviceMetricsOverrideCommand
                            {
                                Width = ViewPortWidth,
                                Height = height,
                                Scale = 1
                            });

                            System.Console.WriteLine("Taking screenshot");
                            var screenshot = await chromeSession.SendAsync(new CaptureScreenshotCommand {Format = "png"});

                            byte[] data = System.Convert.FromBase64String(screenshot.Result.Data);
                            System.IO.File.WriteAllBytes("output.png", data);
                            System.Console.WriteLine("Screenshot stored");





                            PrintToPDFCommand printCommand = new PrintToPDFCommand()
                            {
                                MarginTop = 0,
                                MarginLeft = 0,
                                MarginRight = 0,
                                MarginBottom = 0,
                                PrintBackground = true,
                                Landscape = false,
                                PaperWidth = cm2inch(21),
                                PaperHeight = cm2inch(29.7)
                            };


                            System.Console.WriteLine("Printing PDF");
                            CommandResponse<PrintToPDFCommandResponse> pdf = await chromeSession.SendAsync(printCommand);
                            System.Console.WriteLine("PDF printed.");



                            // tell the main thread we are done
                            screenshotDone.Set();
                        });
                    });

                    // wait for screenshoting thread to (start and) finish
                    screenshotDone.Wait();

                    System.Console.WriteLine("Exiting ..");
                }
            }).Wait();
        }
    }
}