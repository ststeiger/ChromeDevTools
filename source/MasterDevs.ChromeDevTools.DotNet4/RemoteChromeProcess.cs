
using System.Threading.Tasks;


namespace MasterDevs.ChromeDevTools
{


    public class RemoteChromeProcess 
        : IChromeProcess
    {
        
        private readonly System.Net.Http.HttpClient http;

        // System.Uri IChromeProcess.RemoteDebuggingUri => throw new System.NotImplementedException();
        public System.Uri RemoteDebuggingUri { get; }


        public RemoteChromeProcess(System.Uri remoteDebuggingUri)
        {
            RemoteDebuggingUri = remoteDebuggingUri;

            http = new System.Net.Http.HttpClient
            {
                BaseAddress = RemoteDebuggingUri
            };

        } // End Constructor  


        public RemoteChromeProcess(string remoteDebuggingUri)
            : this(new System.Uri(remoteDebuggingUri))
        { } // End Constructor 
        

        // void System.IDisposable.Dispose()
        public virtual void Dispose()
        {
            http.Dispose();
        }

        // Task<ChromeSessionInfo[]> IChromeProcess.GetSessionInfo()
        public async Task<ChromeSessionInfo[]> GetSessionInfo()
        {
            string json = await http.GetStringAsync("/json");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ChromeSessionInfo[]>(json);
        }

        // Task<ChromeSessionInfo> IChromeProcess.StartNewSession()
        public async Task<ChromeSessionInfo> StartNewSession()
        {
            // string link = http.BaseAddress.AbsoluteUri + "json/new";
            // string json = await http.GetStringAsync(link);

            string json = await http.GetStringAsync("/json/new");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ChromeSessionInfo>(json);
        }
        

    }


}
