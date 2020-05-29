
namespace MasterDevs.ChromeDevTools
{


    public static class ChromeProcessFactoryHelper
    {


        public static string[] PossibleUnixExecutables
        {
            get
            {
                string global_install = "/usr/bin";
                string local_install = "/home/" + System.Environment.UserName + "/.config/google-chrome/default";
                string mac_install = "Users/" + System.Environment.UserName + "/Library/Application Support/Google/Chrome/Default";

                if("root".Equals(System.Environment.UserName, System.StringComparison.InvariantCultureIgnoreCase))
                    local_install = "/root/.config/google-chrome/default";

                string[] executables = new string[]
                {
                    "google-chrome",
                    "google-chrome-stable",
                    "chromium-browser",
                    "chromium"
                };


                // Linux: /home/<username>/.config/google-chrome/default
                System.Collections.Generic.List<string> ls = new System.Collections.Generic.List<string>();
                ls.AddRange(executables);

                foreach(string exe in executables)
                {
                    ls.Add(System.IO.Path.Combine(global_install, exe));
                    ls.Add(System.IO.Path.Combine(local_install, exe));
                    ls.Add(System.IO.Path.Combine(mac_install, exe));
                }

                return ls.ToArray();
            }
        }


        public static string[] PossibleWindowsExecutables
        {
            get
            {

                // Windows 7, 8.1, and 10: C:\Users\<username>\AppData\Local\Google\Chrome\User Data\Default
                // Mac OS X El Capitan: Users/<username>/Library/Application Support/Google/Chrome/Default
                // Linux: /home/<username>/.config/google-chrome/default

                string progx86 = ProgramFilesx86();
                string progFiles = System.Environment.GetEnvironmentVariable("ProgramFiles");

                // string appData = System.Environment.ExpandEnvironmentVariables("%AppData%\stuff"); // Roaming
                string appDataChromium = System.Environment.ExpandEnvironmentVariables(@"%LocalAppData%\Chromium\Application\chrome.exe");
                string appDataChrome = System.Environment.ExpandEnvironmentVariables(@"%LocalAppData%\Google\Application\chrome.exe");

                return new string[] {
                    System.IO.Path.Combine(progFiles, @"\Google\Chrome\Application\chrome.exe"),
                    System.IO.Path.Combine(progx86, @"\Google\Chrome\Application\chrome.exe"),
                    appDataChrome,
                    appDataChromium

                };
            }
        }

        private static string ProgramFilesx86()
        {
            if (8 == System.IntPtr.Size
                || (!string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return System.Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return System.Environment.GetEnvironmentVariable("ProgramFiles");
        }


        public static string DefaultChromePath
        {
            get
            {
                // Windows: C:\Users\<username>\AppData\Local\Google\Chrome\User Data\Default
                // Linux: /home/<username>/.config/google-chrome/default
                // Mac: Users/<username>/Library/Application Support/Google/Chrome/Default

                if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
                {
                    foreach (string possibleExecutable in PossibleUnixExecutables)
                    {
                        if (System.IO.File.Exists(possibleExecutable))
                            return possibleExecutable;
                    }

                    throw new System.IO.FileNotFoundException("chrome/chromium not found...");
                }
                
                foreach (string possibleExecutable in PossibleWindowsExecutables)
                {
                    if (System.IO.File.Exists(possibleExecutable))
                        return possibleExecutable;
                }

                throw new System.IO.FileNotFoundException("Chrome.exe not found...");
            }
        }


    }


}
