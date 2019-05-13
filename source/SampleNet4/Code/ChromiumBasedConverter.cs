
using MasterDevs.ChromeDevTools;
using MasterDevs.ChromeDevTools.Protocol.Chrome.Browser;
using MasterDevs.ChromeDevTools.Protocol.Chrome.Page;
using MasterDevs.ChromeDevTools.Protocol.Chrome.Target;

namespace Portal_Convert.CdpConverter
{


    public class ChromiumBasedConverter
    {

        private delegate double UnitConversion_t(double value);


        public static void KillHeadlessChromes(System.IO.TextWriter writer)
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
                    writer.WriteLine($"Killing process {proc.Id} with command line \"{commandLine}\"");
                    ProcessUtils.KillProcessAndChildren(proc.Id);
                } // End if (commandLine.IndexOf(@"--headless") != -1)

            } // Next i 

            writer.WriteLine($"Finished killing headless chromes");
        } // End Sub KillHeadless 
        

        public static void KillHeadlessChromes()
        {
            KillHeadlessChromes(System.Console.Out);
        }


        public static System.Collections.Generic.List<string> KillHeadlessChromesWeb()
        {
            System.Collections.Generic.List<string> ls = new System.Collections.Generic.List<string>();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            using (System.IO.StringWriter sw = new System.IO.StringWriter(sb))
            {
                KillHeadlessChromes(sw);
            } // End Using sw 

            // "abc".Replace("\r\n", "\n").Replace("\r", "\n");
            // "abc".Replace("" & vbCrLf, "" & vbLf).Replace("" & vbCr, "" & vbLf)
            // "abc".Split(vbLf);
            using (System.IO.TextReader tr = new System.IO.StringReader(sb.ToString()))
            {
                string thisLine = null;
                while ((thisLine = tr.ReadLine()) != null)
                {
                    ls.Add(thisLine);
                } // Whend 
            } // End Using tr 

            sb.Length = 0;
            sb = null;

            return ls;
        } // End Function KillHeadlessChromesWeb 


        private static async System.Threading.Tasks.Task InternalConnect(ConnectionInfo ci, string remoteDebuggingUri)
        {
            ci.ChromeProcess = new RemoteChromeProcess(remoteDebuggingUri);
            ci.SessionInfo = await ci.ChromeProcess.StartNewSession();
        } // End Function InternalConnect 


        private static async System.Threading.Tasks.Task<ConnectionInfo> ConnectToChrome(string chromePath, string remoteDebuggingUri)
        {
            ConnectionInfo ci = new ConnectionInfo();

            try
            {
                await InternalConnect(ci, remoteDebuggingUri);
            }
            catch (System.Exception ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Net.WebException)))
                {

                    if (((System.Net.WebException)ex.InnerException).Status == System.Net.WebExceptionStatus.ConnectFailure)
                    {
                        MasterDevs.ChromeDevTools.IChromeProcessFactory chromeProcessFactory =
                                new MasterDevs.ChromeDevTools.ChromeProcessFactory(new FastStubbornDirectoryCleaner(), chromePath);

                        // Create a durable process
                        // MasterDevs.ChromeDevTools.IChromeProcess persistentChromeProcess = chromeProcessFactory.Create(9222, false);
                        MasterDevs.ChromeDevTools.IChromeProcess persistentChromeProcess = chromeProcessFactory.Create(9222, true);

                        await InternalConnect(ci, remoteDebuggingUri);
                        return ci;
                    } // End if (((System.Net.WebException)ex.InnerException).Status == System.Net.WebExceptionStatus.ConnectFailure)

                } // End if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Net.WebException)))

                System.Console.WriteLine(chromePath);
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine(ex.StackTrace);

                if (ex.InnerException != null)
                {
                    System.Console.WriteLine(ex.InnerException.Message);
                    System.Console.WriteLine(ex.InnerException.StackTrace);
                } // End if (ex.InnerException != null)

                System.Console.WriteLine(ex.GetType().FullName);

                throw;
            } // End Catch 

            return ci;
        } // End Function ConnectToChrome 


        
        public static async System.Threading.Tasks.Task ConvertDataAsync(ConversionData conversionData)
        {
#if false
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
#endif

            MasterDevs.ChromeDevTools.IChromeSessionFactory chromeSessionFactory = new MasterDevs.ChromeDevTools.ChromeSessionFactory();

            using (ConnectionInfo connectionInfo = await ConnectToChrome(conversionData.ChromePath, conversionData.RemoteDebuggingUri))
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


                


                // private static double cm2inch(double centimeters) { return centimeters * 0.0393701; }
                UnitConversion_t cm2inch = delegate (double centimeters) { return centimeters * 0.393701; };
                // private static double mm2inch(double milimeters) { return milimeters * 0.0393701; }
                UnitConversion_t mm2inch = delegate (double milimeters) { return milimeters * 0.0393701; };


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

                    // PaperWidth = cm2inch(conversionData.PageWidth),
                    // PaperHeight = cm2inch(conversionData.PageHeight)

                    PaperWidth = mm2inch(conversionData.PageWidth),
                    PaperHeight = mm2inch(conversionData.PageHeight)
                };

                // await System.Threading.Tasks.Task2.Delay(300);


                if (conversionData.ChromiumActions.HasFlag(ChromiumActions_t.GetVersion))
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("Getting browser-version");

                        MasterDevs.ChromeDevTools.CommandResponse<GetVersionCommandResponse> version =
                            await chromeSession.SendAsync(new GetVersionCommand());

                        System.Diagnostics.Debug.WriteLine("Got browser-version");

                        conversionData.Version = version.Result;
                    }
                    catch (System.Exception ex)
                    {
                        conversionData.Exception = ex;
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }

                } // End if (conversionData.ChromiumActions.HasFlag(ChromiumActions_t.GetVersion)) 



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


                System.Console.WriteLine("Closing page");
                await ClosePage(chromeSession, navigateResponse.Result.FrameId, true);
                System.Console.WriteLine("Page closed");

            } // End Using connectionInfo 

        } // End Sub ConvertDataAsync  


        
        private static async System.Threading.Tasks.Task ClosePage(MasterDevs.ChromeDevTools.IChromeSession chromeSession, string frameId, bool headLess)
        {
            System.Threading.Tasks.Task<MasterDevs.ChromeDevTools.CommandResponse<CloseTargetCommandResponse>> closeTargetTask = chromeSession.SendAsync(
                new CloseTargetCommand()
                {
                    TargetId = frameId
                }
            );

            // await will block forever if headless    
            if (!headLess)
            {
                MasterDevs.ChromeDevTools.CommandResponse<CloseTargetCommandResponse> closeTargetResponse = await closeTargetTask;
                System.Console.WriteLine(closeTargetResponse);
            }
            else
            {
                System.Console.WriteLine(closeTargetTask);
            }

        } // End Task ClosePage 


        public static void ConvertData(ConversionData conversionData)
        {
            ConvertDataAsync(conversionData).Wait();
        } // End Sub ConvertData 


    } // End Class ChromiumBasedConverter 


}
