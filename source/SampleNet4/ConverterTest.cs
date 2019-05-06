
using Portal_Convert.CdpConverter;


namespace SampleNet4
{


    class ConverterTest
    {


        [System.STAThread]
        private static void Main(string[] args)
        {
            // ProcessExtensions.Test();
            // Trash.Piping.ReadLibs();
            ChromiumBasedConverter.KillHeadlessChromes();


            string html = Trash.TrashBin.GetExampleText();

            ConversionData conversionData = new ConversionData(html);
            // width = "5590" height = "4850" >
            conversionData.ViewPortWidth = 5590;
            conversionData.ViewPortHeight = 4850;


            conversionData.ChromiumActions = ChromiumActions_t.ConvertToImage | ChromiumActions_t.ConvertToPdf;

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

            if (conversionData.PngData != null)
                System.IO.File.WriteAllBytes("output.png", conversionData.PngData);

            if (conversionData.PdfData != null)
                System.IO.File.WriteAllBytes("output.pdf", conversionData.PdfData);

            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub Main 


    }


}
