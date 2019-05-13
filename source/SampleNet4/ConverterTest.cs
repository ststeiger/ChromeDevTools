
using Portal_Convert.CdpConverter;


namespace SampleNet4
{



    public class WebClientEx 
        : System.Net.WebClient
    {
        public WebClientEx(System.Net.CookieContainer container)
        {
            this.container = container;

            this.Headers.Add("Accept-Language", "de-CH,de;q=0.9");
            this.Headers.Add("pragma", "no-cache");
            this.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.131 Safari/537.36");
        }


        public WebClientEx()
            :this(new System.Net.CookieContainer())
        { }


        public System.Net.CookieContainer CookieContainer
        {
            get { return container; }
            set { container = value; }
        }

        private System.Net.CookieContainer container = new System.Net.CookieContainer();

        protected override System.Net.WebRequest GetWebRequest(System.Uri address)
        {
            System.Net.WebRequest r = base.GetWebRequest(address);
            System.Net.HttpWebRequest request = r as System.Net.HttpWebRequest;
            if (request != null)
            {
                request.CookieContainer = container;
            }
            return r;
        }

        protected override System.Net.WebResponse GetWebResponse(System.Net.WebRequest request, System.IAsyncResult result)
        {
            System.Net.WebResponse response = base.GetWebResponse(request, result);
            ReadCookies(response);
            return response;
        }

        protected override System.Net.WebResponse GetWebResponse(System.Net.WebRequest request)
        {
            System.Net.WebResponse response = base.GetWebResponse(request);
            ReadCookies(response);
            return response;
        }

        private void ReadCookies(System.Net.WebResponse r)
        {
            System.Net.HttpWebResponse response = r as System.Net.HttpWebResponse;
            if (response != null)
            {
                System.Net.CookieCollection cookies = response.Cookies;
                container.Add(cookies);
            }
        }
    }


    class ConverterTest
    {


        [System.STAThread]
        private static void Main(string[] args)
        {
            // SampleNet4.Program.NotMain(args);

            // ProcessExtensions.Test();
            // Trash.Piping.ReadLibs();

            ChromiumBasedConverter.KillHeadlessChromes();


            string html = Trash.TrashBin.GetExampleText();

            ConversionData conversionData = new ConversionData(html);
            // width = "5590" height = "4850" >
            conversionData.ViewPortWidth = 5590;
            conversionData.ViewPortHeight = 4850;


            conversionData.ChromiumActions = ChromiumActions_t.ConvertToImage | ChromiumActions_t.ConvertToPdf | ChromiumActions_t.GetVersion;

            //  width="264px" height="115.167401087568px" 
            conversionData.ViewPortWidth = 264;
            conversionData.ViewPortHeight = 116;


            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.Write("ChromePath: ");
            System.Console.WriteLine(conversionData.ChromePath);

            System.Console.Write("PageWidth: ");
            System.Console.WriteLine(conversionData.PageWidth);
            System.Console.Write("PageHeight: ");
            System.Console.WriteLine(conversionData.PageHeight);

            System.Console.Write("ViewPortWidth: ");
            System.Console.WriteLine(conversionData.ViewPortWidth);
            System.Console.Write("ViewPortHeight: ");
            System.Console.WriteLine(conversionData.ViewPortHeight);


            ChromiumBasedConverter.ConvertData(conversionData);
            
            if (conversionData.Version != null)
            {
                System.Console.Write("Product: ");
                System.Console.WriteLine(conversionData.Version.Product);
                System.Console.Write("JsVersion: ");
                System.Console.WriteLine(conversionData.Version.JsVersion);
                System.Console.Write("UserAgent: ");
                System.Console.WriteLine(conversionData.Version.UserAgent);
                System.Console.Write("Revision: ");
                System.Console.WriteLine(conversionData.Version.Revision);
                System.Console.Write("ProtocolVersion: ");
                System.Console.WriteLine(conversionData.Version.ProtocolVersion);
            }

            if (conversionData.PngData != null)
            {
                System.Console.WriteLine("Writing png data");
                System.IO.File.WriteAllBytes("output.png", conversionData.PngData);
                System.Console.WriteLine("Wrote PNG data");
            }

            if (conversionData.PdfData != null)
            {
                System.Console.WriteLine("Writing PDF data");
                System.IO.File.WriteAllBytes("output.pdf", conversionData.PdfData);
                System.Console.WriteLine("Wrote PDF data");
            }

            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub Main 


    }


}
