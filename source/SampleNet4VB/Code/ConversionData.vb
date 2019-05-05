
Imports MasterDevs.ChromeDevTools.Protocol.Chrome.Browser


Namespace Portal_Convert.CdpConverter


    Public Class ConversionData


        Private Shared s_chromePath As String
        Public Property ChromiumActions As ChromiumActions_t = ChromiumActions_t.ConvertToImage
        Public Property ViewPortWidth As Integer = 1024
        Public Property ViewPortHeight As Integer = 768
        Public Property PageWidth As Double = 21.0
        Public Property PageHeight As Double = 29.7
        Public Property Html As String
        Public Property Exception As System.Exception
        Public Property PngData As Byte()
        Public Property PdfData As Byte()
        Public Property Version As GetVersionCommandResponse
        Private s_changeLock As Object = New Object()



        Public Property ChromePath As String
            Get
                If Not String.IsNullOrEmpty(s_chromePath) Then Return s_chromePath

                SyncLock s_changeLock

                    If System.Web.Hosting.HostingEnvironment.IsHosted Then
                        s_chromePath = System.Web.Hosting.HostingEnvironment.MapPath("~/External/Chromium/chrome.exe")
                    Else
                        s_chromePath = MasterDevs.ChromeDevTools.ChromeProcessFactoryHelper.DefaultChromePath
                    End If
                End SyncLock

                Return s_chromePath
            End Get
            Set(ByVal value As String)
                s_chromePath = value
            End Set
        End Property


        Public Sub New(ByVal html As String)
            Me.Html = html
        End Sub


        Public Sub New()
        End Sub


    End Class


End Namespace
