using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using FileExchangerClient.Annotations;
using FileExchangerClient.FileExchangeService;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using ReactiveUI;
using FileExchangerClient.Settings;
using System.IO;

namespace FileExchangerClient.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly FileExchangeServiceClient _client = new FileExchangeServiceClient();
        private readonly ClientSettings _settings;

        public MainWindowViewModel(ClientSettings settings)
        {
            _settings = settings;
        }

        private List<FileViewModel> _filesList;
        public List<FileViewModel> FilesList
        {
            get { return _filesList; }
            set { this.RaiseAndSetIfChanged(ref _filesList, value); }
        }

        private ICommand _updateFilesListCommand;
        public ICommand UpdateFilesListCommand
        {
            get
            {
                return _updateFilesListCommand ?? (_updateFilesListCommand = new DelegateCommand(UpdateFilesListExecute));
            }
        }

        private void UpdateFilesListExecute()
        {
            var serverFiles = _client.GetFilesList();
            var localFiles = Directory.GetFiles(_settings.DownloadsPath).Select(Path.GetFileName);
            
            FilesList = serverFiles.Union(localFiles).Distinct().Select(fileName => new FileViewModel(fileName, _client, _settings)).ToList();
        }        
    }
}
