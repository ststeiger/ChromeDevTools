
using MasterDevs.ChromeDevTools;
using MasterDevs.ChromeDevTools.Protocol.Chrome.Page;
using MasterDevs.ChromeDevTools.Protocol.Chrome.DOM;


namespace SampleNet4 
{

    


    internal class Program
    {

        private delegate double UnitConversion_t(double value);
        const int ViewPortWidth = 1440;
        const int ViewPortHeight = 900;



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
                return false;
            }
        }


        [System.STAThread]
        private static void NotMain(string[] args)
        {
#if false
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
#endif
            Portal_Convert.CdpConverter.ChromiumBasedConverter.KillHeadlessChromes();
            

            System.Threading.Tasks.Task2.Run(async () =>
            {
                // synchronization
                System.Threading.ManualResetEventSlim screenshotDone = new System.Threading.ManualResetEventSlim();

                // STEP 1 - Run Chrome
                IChromeProcessFactory chromeProcessFactory = new ChromeProcessFactory(new StubbornDirectoryCleaner());
                using (IChromeProcess chromeProcess = chromeProcessFactory.Create(9222, true))
                {
                    // STEP 2 - Create a debugging session
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




                    
                    CommandResponse<NavigateCommandResponse> navigateResponse = await chromeSession.SendAsync(new NavigateCommand
                    {
                        // Url = "http://www.google.com"
                        Url = "about:blank"
                    });
                    System.Console.WriteLine("NavigateResponse: " + navigateResponse.Id);

                    CommandResponse<SetDocumentContentCommandResponse> setContentResponse = await chromeSession.SendAsync(new SetDocumentContentCommand()
                    {
                        FrameId = navigateResponse.Result.FrameId,
                        Html = @"<!doctype html>
<html lang=""en"">
<head>
	<meta charset=""utf-8"">
<title>your title here</title>
</head>
<body bgcolor=""ffffff"">
<center><img src=""clouds.jpg"" align=""bottom""> </center>

<hr>
<a href=""http://somegreatsite.com"">link name</a>
is a link to another nifty site
<h1>this is a header</h1>
<h2>this is a medium header</h2>
send me mail at <a href=""mailto:support@yourcompany.com"">
support@yourcompany.com</a>.
<p> this is a new paragraph!
<p> <b>this is a new paragraph!</b>
<br /> <b><i>this is a new sentence without a paragraph break, in bold italics.</i></b>
<hr>
</body>
</html>
" }
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
                        PaperWidth = cm2inch(21),
                        PaperHeight = cm2inch(29.7),
                    };

                    await System.Threading.Tasks.Task2.Delay(300);



                    try
                    {
                        System.Console.WriteLine("Printing PDF");
                        CommandResponse<PrintToPDFCommandResponse> pdf = await chromeSession.SendAsync(printCommand2);
                        System.Console.WriteLine("PDF printed.");

                        byte[] pdfData = System.Convert.FromBase64String(pdf.Result.Data);
                        System.IO.File.WriteAllBytes("output.pdf", pdfData);
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine(ex.Message);
                    }


                    System.Console.WriteLine("Taking screenshot");
                    CommandResponse<CaptureScreenshotCommandResponse> screenshot =
                        await chromeSession.SendAsync(new CaptureScreenshotCommand { Format = "png" });
                    System.Console.WriteLine("Screenshot taken.");


                    byte[] screenshotData = System.Convert.FromBase64String(screenshot.Result.Data);
                    System.IO.File.WriteAllBytes("output.png", screenshotData);
                    System.Console.WriteLine("Screenshot stored");



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

                            long documentNodeId = (await chromeSession.SendAsync(new GetDocumentCommand())).Result.Root.NodeId;
                            long bodyNodeId =
                                (await chromeSession.SendAsync(new QuerySelectorCommand
                                {
                                    NodeId = documentNodeId,
                                    Selector = "body"
                                })).Result.NodeId;

                            long height = (await chromeSession.SendAsync(new GetBoxModelCommand { NodeId = bodyNodeId })).Result.Model.Height;

                            await chromeSession.SendAsync(new SetDeviceMetricsOverrideCommand
                            {
                                Width = ViewPortWidth,
                                Height = height,
                                Scale = 1
                            });

                            /*
                            System.Console.WriteLine("Taking screenshot");
                            CommandResponse<CaptureScreenshotCommandResponse> screenshot =
                                await chromeSession.SendAsync(new CaptureScreenshotCommand { Format = "png" });
                            System.Console.WriteLine("Screenshot taken.");


                            byte[] screenshotData = System.Convert.FromBase64String(screenshot.Result.Data);
                            System.IO.File.WriteAllBytes("output.png", screenshotData);
                            System.Console.WriteLine("Screenshot stored");
                            */


                            PrintToPDFCommand printCommand = new PrintToPDFCommand()
                            {
                                Scale = 1,
                                MarginTop = 0,
                                MarginLeft = 0,
                                MarginRight = 0,
                                MarginBottom = 0,
                                PrintBackground = true,
                                Landscape = false,
                                PaperWidth = cm2inch(21),
                                PaperHeight = cm2inch(29.7),
                            };

                            await System.Threading.Tasks.Task2.Delay(300);



                            try
                            {
                                System.Console.WriteLine("Printing PDF");
                                CommandResponse<PrintToPDFCommandResponse> pdf = await chromeSession.SendAsync(printCommand);
                                System.Console.WriteLine("PDF printed.");

                                byte[] pdfData = System.Convert.FromBase64String(pdf.Result.Data);
                                System.IO.File.WriteAllBytes("output.pdf", pdfData);
                            }
                            catch (System.Exception ex)
                            {
                                System.Console.WriteLine(ex.Message);
                            }
                            

                            // tell the main thread we are done
                            screenshotDone.Set();
                        });
                    });

                    // wait for screenshoting thread to (start and) finish
                    screenshotDone.Wait();

                    System.Console.WriteLine("Exiting ..");
                }
            }).Wait();
        } // End Sub Main(string[] args) 


    } // End Class Program 


} // End Namespace SampleNet4 
