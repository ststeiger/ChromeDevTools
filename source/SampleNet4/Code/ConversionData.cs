
using MasterDevs.ChromeDevTools.Protocol.Chrome.Browser;


namespace Portal_Convert.CdpConverter
{


    public class ConversionData
    {
        private static string s_chromePath;
        public string RemoteDebuggingUrl { get; set; } = "http://localhost";
        public uint RemoteDebuggingPort { get; set; } = 9222;

        public string RemoteDebuggingUri
        {
            get {
                return this.RemoteDebuggingUrl
                    + ":"
                    + this.RemoteDebuggingPort.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
        }


        public ChromiumActions_t ChromiumActions { get; set; } = ChromiumActions_t.ConvertToImage;

        public int ViewPortWidth { get; set; } = 1024;
        public int ViewPortHeight { get; set; } = 768;

        public double PageWidth { get; set; } = 210;
        public double PageHeight { get; set; } = 297;


        public string Html { get; set; }


        public System.Exception Exception { get; set; }
        public byte[] PngData { get; set; }
        public byte[] PdfData { get; set; }


        public GetVersionCommandResponse Version { get; set; }


        private object s_changeLock = new object();

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
                        s_chromePath = MasterDevs.ChromeDevTools.ChromeProcessFactoryHelper.DefaultChromePath;
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
            :this("")
        { }


    } // End Class ConversionData 


}
