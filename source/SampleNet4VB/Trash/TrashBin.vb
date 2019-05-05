
Namespace SampleNet4VB.Trash


    Class TrashBin


        Public Shared Sub KillHeadless()
        End Sub


        Public Shared Function GetExampleText() As String
            Dim html As String = "<!doctype html>
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
"

            If System.Environment.OSVersion.Platform <> System.PlatformID.Unix Then
                html = System.IO.File.ReadAllText("D:\htmlToPdf.htm", System.Text.Encoding.UTF8)
            End If

            Return html
        End Function


        Private Shared Async Sub StartNew()
            KillHeadless()

            Dim chromeProcessFactory As MasterDevs.ChromeDevTools.IChromeProcessFactory =
                New MasterDevs.ChromeDevTools.ChromeProcessFactory(New MasterDevs.ChromeDevTools.StubbornDirectoryCleaner())

            Using chromeProcess As MasterDevs.ChromeDevTools.IChromeProcess = chromeProcessFactory.Create(9222, False)
                Dim sessionInfos As MasterDevs.ChromeDevTools.ChromeSessionInfo() = Await chromeProcess.GetSessionInfo()
                Dim sessionInfo As MasterDevs.ChromeDevTools.ChromeSessionInfo = If((sessionInfos IsNot Nothing AndAlso sessionInfos.Length > 0), sessionInfos(sessionInfos.Length - 1), New MasterDevs.ChromeDevTools.ChromeSessionInfo())
                Dim chromeSessionFactory As MasterDevs.ChromeDevTools.IChromeSessionFactory = New MasterDevs.ChromeDevTools.ChromeSessionFactory()
                Dim chromeSession As MasterDevs.ChromeDevTools.IChromeSession = chromeSessionFactory.Create(sessionInfo.WebSocketDebuggerUrl)
            End Using

        End Sub


    End Class


End Namespace
