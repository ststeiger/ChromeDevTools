
Imports SampleNet4VB.Portal_Convert.CdpConverter


Namespace SampleNet4VB


    Public Class TestConverter
        
        
        ' https://stackoverflow.com/questions/8869065/program-to-convert-time-in-seconds-to-hhmmss-format
        Function FormatSeconds(ByVal oseconds As object) As String
            If oseconds Is Nothing OrElse oseconds Is System.DbNull.Value Then
                Return ""
            End If
            
            
            
            Dim iSeconds As Integer = CInt(oseconds)
            
            Dim tspan As System.TimeSpan = System.TimeSpan.FromSeconds(iSeconds)
            Dim retValue As String = ""
            
            retValue += tspan.Days.ToString().PadLeft(2, "0"c)
            retValue+=":"
            retValue += tspan.Hours.ToString().PadLeft(2, "0"c)
            retValue+=":"
            retValue += tspan.Minutes.ToString().PadLeft(2, "0"c)
            retValue+=":"
            retValue += tspan.Seconds.ToString().PadLeft(2, "0"c)
            
            Return retValue
        End Sub

    
    
    
        <System.STAThread>
        Public Shared Sub Test()
            ChromiumBasedConverter.KillHeadlessChromes()

            Dim html As String = Trash.TrashBin.GetExampleText()

            Dim conversionData As ConversionData = New ConversionData(html)

            conversionData.ChromiumActions = ChromiumActions_t.ConvertToImage Or ChromiumActions_t.ConvertToPdf Or ChromiumActions_t.GetVersion
            conversionData.ViewPortWidth = 5590
            conversionData.ViewPortHeight = 4850
            conversionData.ViewPortWidth = 264
            conversionData.ViewPortHeight = 116

            System.Console.WriteLine(System.Environment.NewLine)
            System.Console.Write("ChromePath: ")
            System.Console.WriteLine(conversionData.ChromePath)
            System.Console.Write("PageWidth: ")
            System.Console.WriteLine(conversionData.PageWidth)
            System.Console.Write("PageHeight: ")
            System.Console.WriteLine(conversionData.PageHeight)
            System.Console.Write("ViewPortWidth: ")
            System.Console.WriteLine(conversionData.ViewPortWidth)
            System.Console.Write("ViewPortHeight: ")
            System.Console.WriteLine(conversionData.ViewPortHeight)

            ChromiumBasedConverter.ConvertData(conversionData)

            If conversionData.Version IsNot Nothing Then
                System.Console.Write("Product: ")
                System.Console.WriteLine(conversionData.Version.Product)
                System.Console.Write("JsVersion: ")
                System.Console.WriteLine(conversionData.Version.JsVersion)
                System.Console.Write("UserAgent: ")
                System.Console.WriteLine(conversionData.Version.UserAgent)
                System.Console.Write("Revision: ")
                System.Console.WriteLine(conversionData.Version.Revision)
                System.Console.Write("ProtocolVersion: ")
                System.Console.WriteLine(conversionData.Version.ProtocolVersion)
            End If

            If conversionData.PngData IsNot Nothing Then
                System.Console.WriteLine("Writing png data")
                System.IO.File.WriteAllBytes("output.png", conversionData.PngData)
                System.Console.WriteLine("Wrote PNG data")
            End If

            If conversionData.PdfData IsNot Nothing Then
                System.Console.WriteLine("Writing PDF data")
                System.IO.File.WriteAllBytes("output.pdf", conversionData.PdfData)
                System.Console.WriteLine("Wrote PDF data")
            End If

            System.Console.WriteLine(System.Environment.NewLine)
            System.Console.WriteLine(" --- Press any key to continue --- ")
            System.Console.ReadKey()
        End Sub ' Main


    End Class


End Namespace
