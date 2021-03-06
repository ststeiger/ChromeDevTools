﻿
Imports MasterDevs.ChromeDevTools
Imports MasterDevs.ChromeDevTools.Protocol.Chrome.Browser
Imports MasterDevs.ChromeDevTools.Protocol.Chrome.Page
Imports MasterDevs.ChromeDevTools.Protocol.Chrome.Target

Namespace Portal_Convert.CdpConverter


    Public Class ChromiumBasedConverter


        Private Delegate Function UnitConversion_t(ByVal value As Double) As Double




        Public Shared Sub KillHeadlessChromes(ByVal writer As System.IO.TextWriter)
            Dim allProcesses As System.Diagnostics.Process() = System.Diagnostics.Process.GetProcesses()
            Dim exeName As String = "\chrome.exe"

            If System.Environment.OSVersion.Platform = System.PlatformID.Unix Then
                exeName = "/chrome"
            End If

            For i As Integer = 0 To allProcesses.Length - 1
                Dim proc As System.Diagnostics.Process = allProcesses(i)
                Dim commandLine As String = ProcessUtils.GetCommandLine(proc)
                If String.IsNullOrEmpty(commandLine) Then Continue For
                commandLine = commandLine.ToLowerInvariant()
                If commandLine.IndexOf(exeName, System.StringComparison.InvariantCultureIgnoreCase) = -1 Then Continue For

                If commandLine.IndexOf("--headless", System.StringComparison.InvariantCultureIgnoreCase) <> -1 Then
                    writer.WriteLine($"Killing process {proc.Id} with command line ""{commandLine}""")
                    ProcessUtils.KillProcessAndChildren(proc.Id)
                End If
            Next

            writer.WriteLine($"Finished killing headless chromes")
        End Sub


        Public Shared Sub KillHeadlessChromes()
            KillHeadlessChromes(System.Console.Out)
        End Sub


        Private Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function


        Public Shared Function KillHeadlessChromesWeb() As System.Collections.Generic.List(Of String)
            Dim ls As System.Collections.Generic.List(Of String) = New System.Collections.Generic.List(Of String)()
            Dim sb As System.Text.StringBuilder = New System.Text.StringBuilder()

            Using sw As System.IO.StringWriter = New System.IO.StringWriter(sb)
                KillHeadlessChromes(sw)
            End Using

            Using tr As System.IO.TextReader = New System.IO.StringReader(sb.ToString())
                Dim thisLine As String = Nothing

                While (__Assign(thisLine, tr.ReadLine())) IsNot Nothing
                    ls.Add(thisLine)
                End While
            End Using

            sb.Length = 0
            sb = Nothing
            Return ls
        End Function


        Private Shared Async Function InternalConnect(ByVal ci As ConnectionInfo, ByVal remoteDebuggingUri As String) As System.Threading.Tasks.Task
            ci.ChromeProcess = New RemoteChromeProcess(remoteDebuggingUri)
            ci.SessionInfo = Await ci.ChromeProcess.StartNewSession()
        End Function


        Private Shared Async Function ConnectToChrome(ByVal chromePath As String, ByVal remoteDebuggingUri As String) As System.Threading.Tasks.Task(Of ConnectionInfo)
            Dim ci As ConnectionInfo = New ConnectionInfo()

            Try
                Await InternalConnect(ci, remoteDebuggingUri)
            Catch ex As System.Exception

                If ex.InnerException IsNot Nothing AndAlso Object.ReferenceEquals(ex.InnerException.[GetType](), GetType(System.Net.WebException)) Then

                    If (CType(ex.InnerException, System.Net.WebException)).Status = System.Net.WebExceptionStatus.ConnectFailure Then
                        Dim chromeProcessFactory As MasterDevs.ChromeDevTools.IChromeProcessFactory = New MasterDevs.ChromeDevTools.ChromeProcessFactory(New FastStubbornDirectoryCleaner(), chromePath)
                        Dim persistentChromeProcess As MasterDevs.ChromeDevTools.IChromeProcess = chromeProcessFactory.Create(9222, True)

                        ' await cannot be used inside catch ...
                        ' Await InternalConnect(ci, remoteDebuggingUri)
                        InternalConnect(ci, remoteDebuggingUri).Wait()
                        Return ci
                    End If
                End If

                System.Console.WriteLine(chromePath)
                System.Console.WriteLine(ex.Message)
                System.Console.WriteLine(ex.StackTrace)

                If ex.InnerException IsNot Nothing Then
                    System.Console.WriteLine(ex.InnerException.Message)
                    System.Console.WriteLine(ex.InnerException.StackTrace)
                End If

                System.Console.WriteLine(ex.[GetType]().FullName)
                Throw
            End Try

            Return ci
        End Function


        Private Shared Async Function ClosePage(ByVal chromeSession As MasterDevs.ChromeDevTools.IChromeSession, ByVal frameId As String, ByVal headLess As Boolean) As System.Threading.Tasks.Task
            Dim closeTargetTask As System.Threading.Tasks.Task(Of MasterDevs.ChromeDevTools.CommandResponse(Of CloseTargetCommandResponse)) = chromeSession.SendAsync(New CloseTargetCommand() With {
                .TargetId = frameId
            })

            ' await will block forever if headless    
            If Not headLess Then
                Dim closeTargetResponse As MasterDevs.ChromeDevTools.CommandResponse(Of CloseTargetCommandResponse) = Await closeTargetTask
                System.Console.WriteLine(closeTargetResponse)
            Else
                System.Console.WriteLine(closeTargetTask)
            End If
        End Function


        Public Shared Async Function ConvertDataAsync(ByVal conversionData As ConversionData) As System.Threading.Tasks.Task
            Dim chromeSessionFactory As MasterDevs.ChromeDevTools.IChromeSessionFactory = New MasterDevs.ChromeDevTools.ChromeSessionFactory()


            Using connectionInfo As ConnectionInfo = Await ConnectToChrome(conversionData.ChromePath, conversionData.RemoteDebuggingUri)
                Dim chromeSession As MasterDevs.ChromeDevTools.IChromeSession = chromeSessionFactory.Create(connectionInfo.SessionInfo.WebSocketDebuggerUrl)

                Await chromeSession.SendAsync(New SetDeviceMetricsOverrideCommand With {
                    .Width = conversionData.ViewPortWidth,
                    .Height = conversionData.ViewPortHeight,
                    .Scale = 1
                })

                Dim navigateResponse As MasterDevs.ChromeDevTools.CommandResponse(Of NavigateCommandResponse) = Await chromeSession.SendAsync(New NavigateCommand With {
                    .Url = "about:blank"
                })

                System.Console.WriteLine("NavigateResponse: " & navigateResponse.Id)
                Dim setContentResponse As MasterDevs.ChromeDevTools.CommandResponse(Of SetDocumentContentCommandResponse) = Await chromeSession.SendAsync(New SetDocumentContentCommand() With {
                    .FrameId = navigateResponse.Result.FrameId,
                    .Html = conversionData.Html
                })

                Dim cm2inch As UnitConversion_t = Function(ByVal centimeters As Double) centimeters * 0.393701
                Dim mm2inch As UnitConversion_t = Function(ByVal milimeters As Double) milimeters * 0.0393701

                Dim printCommand2 As PrintToPDFCommand = New PrintToPDFCommand() With {
                    .Scale = 1,
                    .MarginTop = 0,
                    .MarginLeft = 0,
                    .MarginRight = 0,
                    .MarginBottom = 0,
                    .PrintBackground = True,
                    .Landscape = False,
                    .PaperWidth = mm2inch(conversionData.PageWidth),
                    .PaperHeight = mm2inch(conversionData.PageHeight) ' 
                }

                '.PaperWidth = cm2inch(conversionData.PageWidth),
                '.PaperHeight = cm2inch(conversionData.PageHeight)


                If conversionData.ChromiumActions.HasFlag(ChromiumActions_t.GetVersion) Then

                    Try
                        System.Diagnostics.Debug.WriteLine("Getting browser-version")
                        Dim version As MasterDevs.ChromeDevTools.CommandResponse(Of GetVersionCommandResponse) = Await chromeSession.SendAsync(New GetVersionCommand())
                        System.Diagnostics.Debug.WriteLine("Got browser-version")
                        conversionData.Version = version.Result
                    Catch ex As System.Exception
                        conversionData.Exception = ex
                        System.Diagnostics.Debug.WriteLine(ex.Message)
                    End Try
                End If

                If conversionData.ChromiumActions.HasFlag(ChromiumActions_t.ConvertToImage) Then

                    Try
                        System.Diagnostics.Debug.WriteLine("Taking screenshot")
                        Dim screenshot As MasterDevs.ChromeDevTools.CommandResponse(Of CaptureScreenshotCommandResponse) = Await chromeSession.SendAsync(New CaptureScreenshotCommand With {
                            .Format = "png"
                        })
                        System.Diagnostics.Debug.WriteLine("Screenshot taken.")
                        conversionData.PngData = System.Convert.FromBase64String(screenshot.Result.Data)
                    Catch ex As System.Exception
                        conversionData.Exception = ex
                        System.Diagnostics.Debug.WriteLine(ex.Message)
                    End Try
                End If

                If conversionData.ChromiumActions.HasFlag(ChromiumActions_t.ConvertToPdf) Then

                    Try
                        System.Diagnostics.Debug.WriteLine("Printing PDF")
                        Dim pdf As MasterDevs.ChromeDevTools.CommandResponse(Of PrintToPDFCommandResponse) = Await chromeSession.SendAsync(printCommand2)
                        System.Diagnostics.Debug.WriteLine("PDF printed.")
                        conversionData.PdfData = System.Convert.FromBase64String(pdf.Result.Data)
                    Catch ex As System.Exception
                        conversionData.Exception = ex
                        System.Diagnostics.Debug.WriteLine(ex.Message)
                    End Try
                End If


                System.Console.WriteLine("Closing page")
                Await ClosePage(chromeSession, navigateResponse.Result.FrameId, True)
                System.Console.WriteLine("Page closed")

            End Using ' connectionInfo

        End Function ' ConvertDataAsync


        Public Shared Sub ConvertData(ByVal conversionData As ConversionData)
            ConvertDataAsync(conversionData).Wait()
        End Sub


    End Class


End Namespace
