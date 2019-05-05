
Namespace Portal_Convert.CdpConverter


    Public Class FastStubbornDirectoryCleaner
        Implements MasterDevs.ChromeDevTools.IDirectoryCleaner


        Public Sub Delete(ByVal dir As System.IO.DirectoryInfo) Implements MasterDevs.ChromeDevTools.IDirectoryCleaner.Delete

            Dim t As System.Threading.Tasks.Task(Of Integer) = System.Threading.Tasks.Task2.Run(
                Function()

                    While True

                        Try
                            dir.Delete(True)
                            Return 0
                        Catch ex As System.Exception

                            Try
                                Dim fis As System.IO.FileInfo() = dir.GetFiles("*.*", System.IO.SearchOption.AllDirectories)

                                For Each fi As System.IO.FileInfo In fis
                                    System.IO.File.SetAttributes(fi.FullName, System.IO.FileAttributes.Normal)
                                Next

                            Catch ex2 As System.Exception
                                System.Console.WriteLine(System.Environment.NewLine)
                                System.Console.WriteLine("Error removing write-protection:")
                                System.Console.WriteLine(ex2.Message)
                            End Try

                            System.Console.WriteLine(ex.Message)
                            System.Threading.Thread.Sleep(500)
                            System.Console.WriteLine("Repeat")
                        End Try
                    End While

                    Return 0
                End Function)
        End Sub


    End Class


End Namespace
