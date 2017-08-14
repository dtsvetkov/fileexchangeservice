using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace FileExchangeService
{
    [RunInstaller(true)]
    public class FileExchangeServiceInstaller : Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public FileExchangeServiceInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
            service.ServiceName = "FileExchangeWindowsService";
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}
