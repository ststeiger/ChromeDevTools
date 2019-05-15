
Namespace Portal_Convert.CdpConverter


    Friend Class WindowsProcess


        ' <System.Runtime.InteropServices.DllImport("ProcCmdLine32.dll", CharSet:=System.Runtime.InteropServices.CharSet.Unicode, EntryPoint:="GetProcCmdLine")>
        ' Private Shared Function GetProcCmdLine32(nProcId As UInteger, sb As System.Text.StringBuilder, dwSizeBuf As UInteger) As Boolean
        ' End Function

        ' <System.Runtime.InteropServices.DllImport("ProcCmdLine64.dll", CharSet:=System.Runtime.InteropServices.CharSet.Unicode, EntryPoint:="GetProcCmdLine")>
        ' Private Shared Function GetProcCmdLine64(nProcId As UInteger, sb As System.Text.StringBuilder, dwSizeBuf As UInteger) As Boolean
        ' End Function

        Private Declare Function GetProcCmdLine32 Lib "ProcCmdLine32.dll" Alias "GetProcCmdLine" (ByVal nProcId As UInteger, ByVal sb As System.Text.StringBuilder, ByVal dwSizeBuf As UInteger) As Boolean
        Private Declare Function GetProcCmdLine64 Lib "ProcCmdLine64.dll" Alias "GetProcCmdLine" (ByVal nProcId As UInteger, ByVal sb As System.Text.StringBuilder, ByVal dwSizeBuf As UInteger) As Boolean


        Public Shared Function GetCommandLineByMissingDll(ByVal proc As System.Diagnostics.Process) As String
            Dim retValue As String = Nothing
            Dim sb As System.Text.StringBuilder = New System.Text.StringBuilder(&HFFFF)

            Select Case System.IntPtr.Size
                Case 4
                    GetProcCmdLine32(CUInt(proc.Id), sb, CUInt(sb.Capacity))
                Case 8
                    GetProcCmdLine64(CUInt(proc.Id), sb, CUInt(sb.Capacity))
            End Select

            retValue = sb.ToString()
            sb.Length = 0
            sb = Nothing
            Return retValue
        End Function


        Public Shared Function GetCommandLine(ByVal process As System.Diagnostics.Process) As String
            Dim cmdLine As String = Nothing

            Using searcher As System.Management.ManagementObjectSearcher = New System.Management.ManagementObjectSearcher($"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {process.Id}")
                Dim matchEnum As System.Management.ManagementObjectCollection.ManagementObjectEnumerator = searcher.[Get]().GetEnumerator()

                If matchEnum.MoveNext() Then
                    cmdLine = matchEnum.Current("CommandLine")?.ToString()
                End If
            End Using

            Return cmdLine
        End Function

        Public Shared Sub KillProcessAndChildren(ByVal pid As Integer)
            If pid = 0 Then
                Return
            End If

            Dim searcher As System.Management.ManagementObjectSearcher = New System.Management.ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" & pid)
            Dim moc As System.Management.ManagementObjectCollection = searcher.[Get]()

            For Each mo As System.Management.ManagementObject In moc
                KillProcessAndChildren(System.Convert.ToInt32(mo("ProcessID")))
            Next

            Try
                Dim proc As System.Diagnostics.Process = System.Diagnostics.Process.GetProcessById(pid)
                proc.Kill()
            Catch __unusedArgumentException1__ As System.ArgumentException
            End Try
        End Sub

        Private Shared Sub EndProcessTree(ByVal imageName As String)
            System.Diagnostics.Process.Start(New System.Diagnostics.ProcessStartInfo With {
                .FileName = "taskkill",
                .Arguments = $"/im {imageName} /f /t",
                .CreateNoWindow = True,
                .UseShellExecute = False
            }).WaitForExit()
        End Sub
    End Class

    Friend Class UnixProcess
        Public Shared Function GetCommandLine(ByVal process As System.Diagnostics.Process) As String
            Dim file As String = $"/proc/{process.Id}/cmdline"
            If Not System.IO.File.Exists(file) Then Return ""
            Dim commandLine As String = System.IO.File.ReadAllText(file, System.Text.Encoding.UTF8)
            If String.IsNullOrEmpty(commandLine) Then Return commandLine
            commandLine = commandLine.Replace(ChrW(0), " "c)
            commandLine = commandLine.Trim(vbFormFeed, vbVerticalTab, vbTab, " "c, vbCr, vbLf)
            Return commandLine
        End Function

        Public Shared Sub KillProcessAndChildren(ByVal pid As Integer)
            System.Diagnostics.Process.Start(New System.Diagnostics.ProcessStartInfo("kill", $"-TERM -{pid}"))
        End Sub
    End Class

    Friend Class ProcessUtils
        Public Shared Function GetCommandLine(ByVal process As System.Diagnostics.Process) As String
            If System.Environment.OSVersion.Platform = System.PlatformID.Unix Then Return UnixProcess.GetCommandLine(process)
            Return WindowsProcess.GetCommandLine(process)
        End Function

        Public Shared Sub KillProcessAndChildren(ByVal pid As Integer)
            If System.Environment.OSVersion.Platform = System.PlatformID.Unix Then
                UnixProcess.KillProcessAndChildren(pid)
            Else
                WindowsProcess.KillProcessAndChildren(pid)
            End If
        End Sub

        Public Shared Sub KillProcessAndChildren(ByVal proc As System.Diagnostics.Process)
            KillProcessAndChildren(proc.Id)
        End Sub
    End Class


End Namespace
