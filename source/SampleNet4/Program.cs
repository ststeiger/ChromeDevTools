
using MasterDevs.ChromeDevTools;
using MasterDevs.ChromeDevTools.Protocol.Chrome.Page;
using MasterDevs.ChromeDevTools.Protocol.Chrome.DOM;
using MasterDevs.ChromeDevTools.Protocol.Chrome.Runtime;
using MasterDevs.ChromeDevTools.Protocol.Chrome.Network;
using MasterDevs.ChromeDevTools.Protocol.Chrome.IndexedDB;
using MasterDevs.ChromeDevTools.Protocol.Chrome.Storage;

namespace SampleNet4 
{


    internal class Program
    {

        private delegate double UnitConversion_t(double value);
        const int ViewPortWidth = 1440;
        const int ViewPortHeight = 900;


        internal class UnsafeNativeMethods
        {
            internal const string LIBC = "libc";

            [System.Runtime.InteropServices.DllImport(LIBC, SetLastError = true)]
            public static extern uint geteuid();
        }


        public static bool IsAdministrator
        {
            get
            {
                // return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows)
                if (System.Environment.OSVersion.Platform != System.PlatformID.Unix)
                {
                    return new System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent())
                       .IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
                }

                // Mono.Posix.NETStandard
                // return Mono.Unix.Native.Syscall.geteuid() == 0;
                return UnsafeNativeMethods.geteuid() == 0;
            }
        }


        public static async System.Threading.Tasks.Task ClearData(IChromeSession chromeSession, string origin, string storageType)
        {
            CommandResponse<ClearDataForOriginCommandResponse> clearStorage = await chromeSession.SendAsync(new ClearDataForOriginCommand()
            {
                Origin = origin,
                StorageTypes = storageType
            });

            System.Console.WriteLine(clearStorage.Result);
        }


        [System.STAThread]
        public static void NotMain(string[] args)
        {
#if false
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
#endif

            System.Console.WriteLine(IsAdministrator);
            Portal_Convert.CdpConverter.ChromiumBasedConverter.KillHeadlessChromes();

            System.Threading.Tasks.Task2.Run(runVote).Wait();


            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
            System.Environment.Exit(0);
        } // End Sub NotMain 


        public static async System.Threading.Tasks.Task runVote()
        {
            // synchronization
            System.Threading.ManualResetEventSlim screenshotDone = new System.Threading.ManualResetEventSlim();

            // STEP 1 - Run Chrome
            IChromeProcessFactory chromeProcessFactory = new ChromeProcessFactory(new StubbornDirectoryCleaner());
            using (IChromeProcess chromeProcess = chromeProcessFactory.Create(9222, false))
            {
                // STEP 2 - Create a debugging session
                ChromeSessionInfo[] sessionInfos = await chromeProcess.GetSessionInfo();

                ChromeSessionInfo sessionInfo = (sessionInfos != null && sessionInfos.Length > 0) ?
                      sessionInfos[sessionInfos.Length - 1]
                    : new ChromeSessionInfo();

                IChromeSessionFactory chromeSessionFactory = new ChromeSessionFactory();
                IChromeSession chromeSession = chromeSessionFactory.Create(sessionInfo.WebSocketDebuggerUrl);




                CommandResponse<ClearBrowserCacheCommandResponse> clearCache = await chromeSession.SendAsync(new ClearBrowserCacheCommand());
                System.Console.WriteLine(clearCache.Result);


                CommandResponse<ClearBrowserCookiesCommandResponse> clearCookies = await chromeSession.SendAsync(new ClearBrowserCookiesCommand());
                System.Console.WriteLine(clearCookies.Result);

                // CommandResponse<ClearObjectStoreCommandResponse> clearObjectStorage = await chromeSession.SendAsync(new ClearObjectStoreCommand());
                // System.Console.WriteLine(clearObjectStorage.Result);


                // CommandResponse<ClearDataForOriginCommandResponse> clearStorage = await chromeSession.SendAsync(new ClearDataForOriginCommand() { Origin= "www.20min.ch", StorageTypes = "all" });
                // Whatever the correct command for clear everything is...
                await ClearData(chromeSession, "www.20min.ch", "all");
                await ClearData(chromeSession, "20min.ch", "all");
                await ClearData(chromeSession, "*", "all");
                await ClearData(chromeSession, "all", "all");



                // STEP 3 - Send a command
                //
                // Here we are sending a commands to tell chrome to set the viewport size 
                // and navigate to the specified URL
                await chromeSession.SendAsync(new SetDeviceMetricsOverrideCommand
                {
                    Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width,
                    Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height,
                    Scale = 1
                });


                CommandResponse<NavigateCommandResponse> navigateResponse = await chromeSession.SendAsync(new NavigateCommand
                {
                    // Url = "http://www.google.com"
                    // Url = "about:blank"
                    Url = "https://www.20min.ch/schweiz/news/story/GA-wird-teurer--kein-Studentenrabatt-mehr-27426069"
                });
                System.Console.WriteLine("NavigateResponse: " + navigateResponse.Id);



                // STEP 4 - Register for events (in this case, "Page" domain events)
                // send an command to tell chrome to send us all Page events
                // but we only subscribe to certain events in this session

                ICommandResponse pageEnableResult = await chromeSession.SendAsync<MasterDevs.ChromeDevTools.Protocol.Chrome.Page.EnableCommand>();
                System.Console.WriteLine("PageEnable: " + pageEnableResult.Id);

                chromeSession.Subscribe<LoadEventFiredEvent>(loadEventFired =>
                {
                    // we cannot block in event handler, hence the task
                    System.Threading.Tasks.Task2.Run(async () =>
                    {
                    System.Console.WriteLine("LoadEventFiredEvent: " + loadEventFired.Timestamp);
                    /*
                    long documentNodeId = (await chromeSession.SendAsync(new GetDocumentCommand())).Result.Root.NodeId;
                    long bodyNodeId =
                        (await chromeSession.SendAsync(new QuerySelectorCommand
                        {
                            NodeId = documentNodeId,
                            Selector = "body"
                        })).Result.NodeId;

                    long height = (await chromeSession.SendAsync(new GetBoxModelCommand { NodeId = bodyNodeId })).Result.Model.Height;
                    */


                    CommandResponse<EvaluateCommandResponse> removePopOver = await chromeSession.SendAsync(new EvaluateCommand
                    {
                        Expression = @"
    window.setTimeout(function(){ 
        var one = document.getElementById('onesignal-popover-cancel-button');
            if(one != null)
        one.click();
    }, 2000);


window.setTimeout(function(){ 
        window.close();
    }, 4000);
    ",
                        // ContextId = 123
                    });
                    
                    
                    if (true)
                    {
                        // document.querySelector("#thread3367_msg3367 > div.rate_button > div.clickable.top").click()
                        // document.querySelector("#thread3367_msg3367 > div.rate_button > div.clickable.bottom").click()
                        string threadId = "3399";
                        string msgId = "3399";
                        string voteDirection = "bottom"; // top / bottom

                        string votingElement = "#thread" + threadId + "_msg" + msgId + @" > div.rate_button > div.clickable." + voteDirection;

                        string javaScriptToExecute = @"
    (function()
    {
        var elmnt = document.querySelector('" + votingElement + @"');
        if (elmnt != null)
        {
            elmnt.scrollIntoView();
            window.scrollBy(0, -70)
            elmnt.click();
            console.log('https://www.youtube.com/watch?v=h6mJw50OdZ4&t=163');
            console.log('The first honest vote ever in a rotten borough !');
            console.log('CopyLeft 2019 StS');
        }
    })();
    ";


                            CommandResponse<EvaluateCommandResponse> evr = await chromeSession.SendAsync(new EvaluateCommand
                            {
                                Expression = javaScriptToExecute,
                                    // ContextId = 123
                                });

                            if (evr.Result.ExceptionDetails != null)
                                System.Console.WriteLine(evr.Result.ExceptionDetails);
                            else
                                System.Console.WriteLine("voted");
                        }

                        await System.Threading.Tasks.Task2.Delay(3000);

                        // tell the main thread we are done
                        screenshotDone.Set();
                    });

                }); // End Sub LoadEventFired 

                // wait for screenshoting thread to (start and) finish
                screenshotDone.Wait();

                System.Console.WriteLine("Exiting ..");
            } // End Using chromeProcess

        } // End Sub Main(string[] args) 


    } // End Class Program 


} // End Namespace SampleNet4 
