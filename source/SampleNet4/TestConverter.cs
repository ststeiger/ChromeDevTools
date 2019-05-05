
using MasterDevs.ChromeDevTools;
using MasterDevs.ChromeDevTools.Protocol.Chrome.Page;
using MasterDevs.ChromeDevTools.Protocol.Chrome.Target;
// using MasterDevs.ChromeDevTools.Protocol.Chrome.DOM;


namespace SampleNet4
{


    internal class TestConverter
    {


        public static void KillHeadless()
        {
            System.Diagnostics.Process[] allProcesses = System.Diagnostics.Process.GetProcesses();

            string exeName = @"\chrome.exe";

            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
            {
                exeName = "/chrome";
            } // End if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)


            for (int i = 0; i < allProcesses.Length; ++i)
            {
                System.Diagnostics.Process proc = allProcesses[i];
                string commandLine = ProcessUtils.GetCommandLine(proc); // GetCommandLineOfProcess(proc);

                if (string.IsNullOrEmpty(commandLine))
                    continue;

                commandLine = commandLine.ToLowerInvariant();

                if (commandLine.IndexOf(exeName, System.StringComparison.InvariantCultureIgnoreCase) == -1)
                    continue;

                if (commandLine.IndexOf(@"--headless", System.StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    System.Console.WriteLine($"Killing process {proc.Id} with command line \"{commandLine}\"");
                    ProcessUtils.KillProcessAndChildren(proc.Id);
                } // End if (commandLine.IndexOf(@"--headless") != -1)

            } // Next i 

            System.Console.WriteLine($"Finished killing headless chromes");
        } // End Sub KillHeadless 


        [System.STAThread]
        private static void Main(string[] args)
        {
            // Trash.Piping.ReadLibs();
            KillHeadless();


            string html = Trash.TrashBin.GetExampleText();

            ConversionData conversionData = new ConversionData(html);
            // width = "5590" height = "4850" >
            conversionData.ViewPortWidth = 5590;
            conversionData.ViewPortHeight = 4850;


            //  width="264px" height="115.167401087568px" 
            conversionData.ViewPortWidth = 264;
            conversionData.ViewPortHeight = 116;

            System.Console.WriteLine(conversionData.ChromePath);



            AsyncMain(conversionData).Wait();

            System.IO.File.WriteAllBytes("output.png", conversionData.PngData);
            System.IO.File.WriteAllBytes("output.pdf", conversionData.PdfData);

            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub Main 


        public class FastStubbornDirectoryCleaner 
            : MasterDevs.ChromeDevTools.IDirectoryCleaner
        {

            public void Delete(System.IO.DirectoryInfo dir)
            {
                System.Threading.Tasks.Task<int> t  = System.Threading.Tasks.Task2.Run(delegate()
                    {
                        while (true)
                        {
                            try
                            {
                                dir.Delete(true);
                                return 0;
                            }
                            catch (System.Exception ex)
                            {
                                try
                                {
                                    System.IO.FileInfo[] fis = dir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

                                    foreach (System.IO.FileInfo fi in fis)
                                    {
                                        System.IO.File.SetAttributes(fi.FullName, System.IO.FileAttributes.Normal);
                                    }
                                }
                                catch (System.Exception ex2)
                                {
                                    System.Console.WriteLine(System.Environment.NewLine);
                                    System.Console.WriteLine("Error removing write-protection:");
                                    System.Console.WriteLine(ex2.Message);
                                }

                                System.Console.WriteLine(ex.Message);
                                System.Threading.Thread.Sleep(500);
                                System.Console.WriteLine("Repeat");
                            }
                        }

                    }
                );

            } // End Sub Delete 
            
        } // End Class DiagnosticDirectoryCleaner 


        public class ConnectionInfo
            : System.IDisposable
        {
            public MasterDevs.ChromeDevTools.IChromeProcess ChromeProcess;
            public MasterDevs.ChromeDevTools.ChromeSessionInfo SessionInfo;
            
            // TODO: Finalizer nur überschreiben, wenn Dispose(bool disposing) weiter oben Code für die Freigabe nicht verwalteter Ressourcen enthält.
            // ~ConnectionInfo() {
            //   // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
            //   Dispose(false);
            // }
            
            
            // Dieser Code wird hinzugefügt, um das Dispose-Muster richtig zu implementieren.
            void System.IDisposable.Dispose()
            {
                // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
                if (this.ChromeProcess != null)
                    ChromeProcess.Dispose();

                // TODO: Auskommentierung der folgenden Zeile aufheben, wenn der Finalizer weiter oben überschrieben wird.
                // GC.SuppressFinalize(this);
            }

        }


        private static async System.Threading.Tasks.Task InternalConnect(ConnectionInfo ci, string remoteDebuggingUri)
        {
            ci.ChromeProcess = new RemoteChromeProcess(remoteDebuggingUri);
            ci.SessionInfo = await ci.ChromeProcess.StartNewSession();
            /*
            MasterDevs.ChromeDevTools.ChromeSessionInfo[] sessionInfos = await ci.ChromeProcess.GetSessionInfo();

            ci.SessionInfo = (sessionInfos != null && sessionInfos.Length > 0) ?
                sessionInfos[sessionInfos.Length - 1]
                : await ci.ChromeProcess.StartNewSession()
            ;
            */
        } // End Function InternalConnect 
        

        public static async System.Threading.Tasks.Task<ConnectionInfo> ConnectToChrome(string chromePath, string remoteDebuggingUri)
        {
            ConnectionInfo ci = new ConnectionInfo();

            try
            {
                await InternalConnect(ci, remoteDebuggingUri);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine(ex.GetType().FullName);
                System.Console.WriteLine(chromePath);


                MasterDevs.ChromeDevTools.IChromeProcessFactory chromeProcessFactory =
                        new MasterDevs.ChromeDevTools.ChromeProcessFactory(new FastStubbornDirectoryCleaner(), chromePath);

                // Create a durable process
                // MasterDevs.ChromeDevTools.IChromeProcess persistentChromeProcess = chromeProcessFactory.Create(9222, false);
                MasterDevs.ChromeDevTools.IChromeProcess persistentChromeProcess = chromeProcessFactory.Create(9222, true);

                await InternalConnect(ci, remoteDebuggingUri);
            }

            return ci;
        } // End Function ConnectToChrome 


        [System.Flags]
        public enum ChromiumActions_t
        {
            None = 0,
            ConvertToImage = 1,
            ConvertToPdf = 2,
            // Option3 = 4,
            // Option4 = 8
        }


        public class ConversionData 
        {
            private static string s_chromePath;


            private static double cm2inch(double centimeters)
            {
                return centimeters * 0.393701;
            }

            private static double mm2inch(double centimeters)
            {
                return centimeters * 0.0393701;
            }



            public ChromiumActions_t ChromiumActions { get; set; } = ChromiumActions_t.ConvertToImage;

            public int ViewPortWidth { get; set; } = 1024;
            public int ViewPortHeight { get; set; } = 768;

            public double PageWidth { get; set; } = cm2inch(21);
            public double PageHeight { get; set; } = cm2inch(29.7);


            public string Html { get; set; }


            public  System.Exception Exception { get; set; }
            public byte[] PngData { get; set; }
            public byte[] PdfData { get; set; }

            private  object s_changeLock = new object();

            public string ChromePath
            {
                get
                {
                    if (!string.IsNullOrEmpty(s_chromePath))
                        return s_chromePath;

                    lock (s_changeLock)
                    { 
                        // "~/External/Chromium/x86-" + (System.IntPtr.Size * 8).ToString()
                        if (System.Web.Hosting.HostingEnvironment.IsHosted)
                            s_chromePath = System.Web.Hosting.HostingEnvironment.MapPath(
                                "~/External/Chromium/chrome.exe"
                            );
                        else
                            s_chromePath = ChromeProcessFactoryHelper.DefaultChromePath;
                    } // End lock (s_changeLock) 

                    return s_chromePath;
                }
                set
                {
                    s_chromePath = value;
                }
            } // End Property ChromePath 


            public ConversionData(string html)
            {
                this.Html = html;
            }

            public ConversionData()
            { }


        } // End Class ConversionData 


        private static async System.Threading.Tasks.Task AsyncMain(ConversionData conversionData)
        {
#if false
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
#endif

            // http://localhost:9222/
            // http://localhost:9222/json

            string rcp = "http://localhost:9222";
            MasterDevs.ChromeDevTools.IChromeSessionFactory chromeSessionFactory = new MasterDevs.ChromeDevTools.ChromeSessionFactory();

            using (ConnectionInfo connectionInfo = await ConnectToChrome(conversionData.ChromePath, rcp) )
            {
                MasterDevs.ChromeDevTools.IChromeSession chromeSession = chromeSessionFactory.Create(connectionInfo.SessionInfo.WebSocketDebuggerUrl);

                // STEP 3 - Send a command
                //
                // Here we are sending a commands to tell chrome to set the viewport size 
                // and navigate to the specified URL
                await chromeSession.SendAsync(new SetDeviceMetricsOverrideCommand
                {
                    Width = conversionData.ViewPortWidth,
                    Height = conversionData.ViewPortHeight,
                    Scale = 1
                });

                MasterDevs.ChromeDevTools.CommandResponse<NavigateCommandResponse> navigateResponse =
                    await chromeSession.SendAsync(new NavigateCommand
                    {
                        // Url = "http://www.google.com"
                        Url = "about:blank"
                    });
                System.Console.WriteLine("NavigateResponse: " + navigateResponse.Id);

                MasterDevs.ChromeDevTools.CommandResponse<SetDocumentContentCommandResponse> setContentResponse =
                    await chromeSession.SendAsync(new SetDocumentContentCommand()
                    {
                        FrameId = navigateResponse.Result.FrameId,
                        Html = conversionData.Html
                    }
                );


                PrintToPDFCommand printCommand2 = new PrintToPDFCommand()
                {
                    Scale = 1,
                    MarginTop = 0,
                    MarginLeft = 0,
                    MarginRight = 0,
                    MarginBottom = 0,
                    PrintBackground = true,
                    Landscape = false,
                    // PaperWidth = cm2inch(21),
                    // PaperHeight = cm2inch(29.7),

                    PaperWidth = conversionData.PageWidth,
                    PaperHeight = conversionData.PageHeight
                };

                // await System.Threading.Tasks.Task2.Delay(300);





                if (conversionData.ChromiumActions.HasFlag(ChromiumActions_t.ConvertToImage))
                {

                    try
                    {
                        System.Diagnostics.Debug.WriteLine("Taking screenshot");

                        MasterDevs.ChromeDevTools.CommandResponse<CaptureScreenshotCommandResponse> screenshot =
                            await chromeSession.SendAsync(new CaptureScreenshotCommand { Format = "png" });

                        System.Diagnostics.Debug.WriteLine("Screenshot taken.");

                        conversionData.PngData = System.Convert.FromBase64String(screenshot.Result.Data);
                    }
                    catch (System.Exception ex)
                    {
                        conversionData.Exception = ex;
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }

                } // End if (conversionData.ChromiumActions.HasFlag(ChromiumActions_t.ConvertToImage)) 


                if (conversionData.ChromiumActions.HasFlag(ChromiumActions_t.ConvertToPdf))
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("Printing PDF");

                        MasterDevs.ChromeDevTools.CommandResponse<PrintToPDFCommandResponse> pdf =
                            await chromeSession.SendAsync(printCommand2);

                        System.Diagnostics.Debug.WriteLine("PDF printed.");

                        conversionData.PdfData = System.Convert.FromBase64String(pdf.Result.Data);
                    }
                    catch (System.Exception ex)
                    {
                        conversionData.Exception = ex;
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }

                } // End if (conversionData.ChromiumActions.HasFlag(ChromiumActions_t.ConvertToPdf)) 





#if false // NOT_HEADLESS
                System.Console.WriteLine("Closing page");
                MasterDevs.ChromeDevTools.CommandResponse<CloseTargetCommandResponse> closeTargetResponse =
                    await chromeSession.SendAsync(
                    new CloseTargetCommand()
                    {
                        TargetId = navigateResponse.Result.FrameId
                    }
                );
                System.Console.WriteLine("Page closed");
                System.Console.WriteLine(closeTargetResponse);
#endif

            } // End Using remoteChromeProcess 


            return;
        } // End Sub AsyncMain 


    } // End Class 


} // End Namespace 
