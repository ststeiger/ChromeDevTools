
namespace SampleNet4.Trash
{


    public class Piping
    {


        public static string PipeString(string input, string fileName, string arguments)
        {
            System.Diagnostics.ProcessStartInfo startInfo =
                new System.Diagnostics.ProcessStartInfo();

            startInfo.FileName = fileName;
            startInfo.Arguments = arguments;

            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;


            string output = null;

            using (System.Diagnostics.Process process = new System.Diagnostics.Process())
            {
                process.StartInfo = startInfo;
                process.Start();

                process.StandardInput.WriteLine(input);

                //System.IO.TextWriter sw = new System.IO.StreamWriter(
                //      process.StandardInput.BaseStream
                //    , System.Text.Encoding.ASCII);
                // sw.WriteLine("äöü");

                // process.StandardInput.BaseStream.Write(buffer, 0, buffer.Length);
                process.StandardInput.Flush();
                process.StandardInput.Close();

                // System.IO.TextReader tr = new System.IO.StreamReader(process.StandardOutput.BaseStream, System.Text.Encoding.UTF7);
                // tr.ReadToEnd();
                // process.StandardOutput.BaseStream.Read(buffer, 0, buffer.Length);
                output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            } // End Using process 

            return output;
        } // End Function PipeString 


        public static string GetOutput(string fileName, string arguments)
        {
            string output = null;

            using (System.Diagnostics.Process p = new System.Diagnostics.Process())
            {
                // Redirect the output stream of the child process.
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = fileName;
                p.StartInfo.Arguments = arguments;

                p.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;

                p.Start();
                // Do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // p.WaitForExit();
                // Read the output stream first and then wait.
                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
            } // End Using p 

            return output;
        } // End Function GetOutput 


        public static void ReadLibs()
        {
            // https://askubuntu.com/questions/17823/how-to-list-all-installed-packages
            // dpkg --get-selections | grep -v deinstall | grep "^lib" >> package_list.txt

            // string input = System.IO.File.ReadAllText("package_list.txt", System.Text.Encoding.UTF8);

            string input = GetOutput("dpkg", "--get-selections");
            string grepped = PipeString(input, "grep", "-v deinstall");
            string package_list = PipeString(grepped, "grep", "^lib");
            System.Console.WriteLine(package_list);


            int counter = 0;
            string line;

            // Read the file and display it line by line.  
            using (System.IO.StreamReader file =
                new System.IO.StreamReader(@"/root/github/package_list.txt"
                , System.Text.Encoding.UTF8))
            {
                while ((line = file.ReadLine()) != null)
                {
                    System.Console.WriteLine(line);
                    counter++;
                } // Whend 

                file.Close();
            } // End Using file 

            System.Console.WriteLine("There were {0} lines.", counter);
        } // End Sub ReadLibs 


    } // End Class Piping


} // End Namespace SampleNet4.Trash
