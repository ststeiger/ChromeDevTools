
Namespace Portal_Convert.CdpConverter


    Public Class ConnectionInfo
        Implements System.IDisposable

        Public ChromeProcess As MasterDevs.ChromeDevTools.IChromeProcess
        Public SessionInfo As MasterDevs.ChromeDevTools.ChromeSessionInfo


        Public Sub Dispose() Implements IDisposable.Dispose
            If Me.ChromeProcess IsNot Nothing Then ChromeProcess.Dispose()
        End Sub

    End Class


End Namespace
