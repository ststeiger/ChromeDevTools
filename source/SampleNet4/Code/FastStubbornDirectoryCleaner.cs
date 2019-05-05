
namespace Portal_Convert.CdpConverter
{


    public class FastStubbornDirectoryCleaner
        : MasterDevs.ChromeDevTools.IDirectoryCleaner
    {

        public void Delete(System.IO.DirectoryInfo dir)
        {
            System.Threading.Tasks.Task<int> t = System.Threading.Tasks.Task2.Run(delegate ()
            {
                while (true)
                {
                    try
                    {
                        dir.Delete(true);
                        return 0;
                    }
                    catch (System.Exception ex)
                    {
                        try
                        {
                            System.IO.FileInfo[] fis = dir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

                            foreach (System.IO.FileInfo fi in fis)
                            {
                                System.IO.File.SetAttributes(fi.FullName, System.IO.FileAttributes.Normal);
                            }
                        }
                        catch (System.Exception ex2)
                        {
                            System.Console.WriteLine(System.Environment.NewLine);
                            System.Console.WriteLine("Error removing write-protection:");
                            System.Console.WriteLine(ex2.Message);
                        }

                        System.Console.WriteLine(ex.Message);
                        System.Threading.Thread.Sleep(500);
                        System.Console.WriteLine("Repeat");
                    }
                }

            }
            );

        } // End Sub Delete 


    } // End Class DiagnosticDirectoryCleaner 


} // End Namespace Portal_Convert.CdpConverter
