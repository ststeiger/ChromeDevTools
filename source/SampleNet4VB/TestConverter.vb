
Imports SampleNet4VB.Portal_Convert.CdpConverter


Namespace SampleNet4VB


    Public Class TestConverter


        <System.STAThread>
        Public Shared Sub Test()
            ChromiumBasedConverter.KillHeadlessChromes()

            Dim html As String = Trash.TrashBin.GetExampleText()

            Dim conversionData As ConversionData = New ConversionData(html)

            conversionData.ChromiumActions = ChromiumActions_t.ConvertToImage Or ChromiumActions_t.ConvertToPdf
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
            If conversionData.PngData IsNot Nothing Then System.IO.File.WriteAllBytes("output.png", conversionData.PngData)
            If conversionData.PdfData IsNot Nothing Then System.IO.File.WriteAllBytes("output.pdf", conversionData.PdfData)


            System.Console.WriteLine(System.Environment.NewLine)
            System.Console.WriteLine(" --- Press any key to continue --- ")
            System.Console.ReadKey()
        End Sub ' Main


    End Class


End Namespace
