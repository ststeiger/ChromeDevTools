using MasterDevs.ChromeDevTools;


namespace SampleNet4.Trash
{
    class TrashBin
    {
        public static void KillHeadless()
        {

        }

        public static string GetExampleText()
        {
            string html = @"<!doctype html>
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
";
            if (System.Environment.OSVersion.Platform != System.PlatformID.Unix)
            {
                // html = System.IO.File.ReadAllText(@"D:\not_that.svg", System.Text.Encoding.UTF8);
                // html = System.IO.File.ReadAllText(@"D:\1556974960567.svg", System.Text.Encoding.UTF8);
                html = System.IO.File.ReadAllText(@"D:\htmlToPdf.htm", System.Text.Encoding.UTF8);
                // html = System.IO.File.ReadAllText(@"D:\svgToImage.htm", System.Text.Encoding.UTF8);
            }

            return html;
        }


        private static async void StartNew()
        {
            KillHeadless();

            // STEP 1 - Run Chrome
            MasterDevs.ChromeDevTools.IChromeProcessFactory chromeProcessFactory = 
                new MasterDevs.ChromeDevTools.ChromeProcessFactory(new StubbornDirectoryCleaner());

            //using (MasterDevs.ChromeDevTools.IChromeProcess chromeProcess = chromeProcessFactory.Create(9222, true))
            using (MasterDevs.ChromeDevTools.IChromeProcess chromeProcess = chromeProcessFactory.Create(9222, false))
            {
                // STEP 2 - Create a debugging session
                MasterDevs.ChromeDevTools.ChromeSessionInfo[] sessionInfos = await chromeProcess.GetSessionInfo();

                MasterDevs.ChromeDevTools.ChromeSessionInfo sessionInfo = (sessionInfos != null && sessionInfos.Length > 0) ?
                      sessionInfos[sessionInfos.Length - 1]
                    : new MasterDevs.ChromeDevTools.ChromeSessionInfo();

                MasterDevs.ChromeDevTools.IChromeSessionFactory chromeSessionFactory = new MasterDevs.ChromeDevTools.ChromeSessionFactory();

                MasterDevs.ChromeDevTools.IChromeSession chromeSession = chromeSessionFactory.Create(sessionInfo.WebSocketDebuggerUrl);
            } // End Using chromeProcess 


        }


    }
}
