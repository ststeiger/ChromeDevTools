
namespace Portal_Convert.CdpConverter
{


    public class ConnectionInfo
        : System.IDisposable
    {
        public MasterDevs.ChromeDevTools.IChromeProcess ChromeProcess;
        public MasterDevs.ChromeDevTools.ChromeSessionInfo SessionInfo;

        // TODO: Finalizer nur überschreiben, wenn Dispose(bool disposing) weiter oben Code für die Freigabe nicht verwalteter Ressourcen enthält.
        // ~ConnectionInfo() {
        //   // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
        //   Dispose(false);
        // }


        // Dieser Code wird hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        void System.IDisposable.Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
            if (this.ChromeProcess != null)
                ChromeProcess.Dispose();

            // TODO: Auskommentierung der folgenden Zeile aufheben, wenn der Finalizer weiter oben überschrieben wird.
            // GC.SuppressFinalize(this);
        }

    }


}
