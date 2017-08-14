using FileExchangerClient.FileExchangeService;
using FileExchangerClient.Settings;
using Microsoft.Practices.Prism.Commands;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FileExchangerClient.ViewModels
{
    public class FileViewModel : ViewModelBase
    {
        private readonly string _localFilePath;
        private readonly FileExchangeServiceClient _client;
        private readonly ClientSettings _settings;

        private bool _stopSynchronizing;

        private long _localFileSize;
        private long _remoteFileSize;

        public FileViewModel(string fileName, FileExchangeServiceClient client, ClientSettings settings)
        {
            Name = fileName;

            _client = client;
            _settings = settings;
            _localFilePath = Path.Combine(_settings.DownloadsPath, Name);

            _localFileSize = 0;
            _remoteFileSize = 0;

            if (File.Exists(_localFilePath))
            {
                _localFileSize = new FileInfo(_localFilePath).Length;
            }
            var remoteFileInfo = _client.GetFileInfo(fileName);
            if (remoteFileInfo != null)
            {
                _remoteFileSize = remoteFileInfo.Length;
            }

            if (_remoteFileSize == _localFileSize)
            {
                SynchronizedPercent = 100;
            }
            else
            {
                SynchronizedPercent = (Math.Min(_remoteFileSize, _localFileSize) / (double)Math.Max(_remoteFileSize, _localFileSize)) * 100;
            }
        }

        #region Properties

        public string Name
        {
            get; private set;
        }

        private double _synchronizedPercent;
        public double SynchronizedPercent
        {
            get { return _synchronizedPercent; }
            private set { this.RaiseAndSetIfChanged(ref _synchronizedPercent, value); }
        }

        private bool _isSynchronizing;
        public bool IsSynchronizing
        {
            get { return _isSynchronizing; }
            private set { this.RaiseAndSetIfChanged(ref _isSynchronizing, value); }
        }

        #endregion

        #region Commands

        private DelegateCommand _synchronizeCommand;
        public DelegateCommand SynchronizeCommand
        {
            get
            {
                return _synchronizeCommand ?? (_synchronizeCommand = new DelegateCommand(SynchronizeExecute, CanExecuteSynchronize));
            }
        }

        private void SynchronizeExecute()
        {
            if (IsSynchronizing)
            {
                _stopSynchronizing = true;
                return;
            }

            IsSynchronizing = true;
            _stopSynchronizing = false;

            if (_localFileSize < _remoteFileSize)
            {
                Download();
            }
            else
            {
                Upload();
            }
        }

        private void Download()
        {
            LoadOperation(() =>
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(_localFilePath, FileMode.OpenOrCreate, FileAccess.Write)))
                {
                    while (!_stopSynchronizing && _localFileSize < _remoteFileSize)
                    {
                        var chunk = _client.GetFileChunk(Name, (int)_localFileSize, _settings.ChunkSize);

                        writer.BaseStream.Seek(0, SeekOrigin.End);
                        writer.Write(chunk);

                        _localFileSize += chunk.Length;

                        SynchronizedPercent = (_localFileSize / (double)_remoteFileSize) * 100;

                        Thread.Sleep(100);
                    }
                }
            });
        }

        private void Upload()
        {
            LoadOperation(() =>
            {
                using (BinaryReader reader = new BinaryReader(File.Open(_localFilePath, FileMode.Open, FileAccess.Read)))
                {
                    while (!_stopSynchronizing && _remoteFileSize < _localFileSize)
                    {
                        reader.BaseStream.Seek(_remoteFileSize, SeekOrigin.Begin);

                        byte[] data = reader.ReadBytes(_settings.ChunkSize);

                        var response = _client.LoadFileChunk(Name, data);
                        if (response.ErrorCode != ErrorCode.NoError)
                        {
                            _stopSynchronizing = true;
                        }
                        else
                        {
                            _remoteFileSize += data.Length;
                        }

                        SynchronizedPercent = (_remoteFileSize / (double)_localFileSize) * 100;

                        Thread.Sleep(100);
                    }
                }
            });
        }

        private void LoadOperation(Action operation)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                operation();

                _stopSynchronizing = false;
                IsSynchronizing = false;

                Application.Current.Dispatcher.Invoke(() => SynchronizeCommand.RaiseCanExecuteChanged());
            });
        }

        private bool CanExecuteSynchronize()
        {
            return SynchronizedPercent < 100;
        }

        #endregion
    }
}
