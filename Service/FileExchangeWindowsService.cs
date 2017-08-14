using System.ServiceModel;
using System.ServiceProcess;

namespace FileExchangeService
{
    public class FileExchangeWindowsService : ServiceBase
    {
        private ServiceHost _serviceHost = null;

        public FileExchangeWindowsService()
        {
            ServiceName = "FileExchangeWindowsService";
        }

        public static void Main(string[] args)
        {
            ServiceBase.Run(new FileExchangeWindowsService());
        }

        protected override void OnStart(string[] args)
        {
            if (_serviceHost != null)
            {
                _serviceHost.Close();
            }

            _serviceHost = new ServiceHost(typeof(FileExchangeService));

            _serviceHost.Open();
        }

        protected override void OnStop()
        {
            if (_serviceHost != null)
            {
                _serviceHost.Close();
                _serviceHost = null;
            }
        }
    }
}
