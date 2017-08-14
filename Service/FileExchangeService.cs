using FileExchangeService.ConfigurationSections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FileExchangeService
{
    public enum ErrorCode
    {
        NoError,
        InternalError,
    }

    [DataContract]
    public class ResponseInfo
    {
        [DataMember]
        public ErrorCode ErrorCode
        {
            get;
            set;
        }

        [DataMember]
        public string Message
        {
            get;
            set;
        }
    }

    [ServiceContract]
    public interface IFileExchangeService
    {
        [OperationContract]
        List<string> GetFilesList();

        [OperationContract]
        FileInfo GetFileInfo(string fileName);

        [OperationContract]
        byte[] GetFileChunk(string fileName, int offset, int length);

        [OperationContract]
        ResponseInfo CreateFile(string fileName);

        [OperationContract]
        ResponseInfo LoadFileChunk(string fileName, byte[] bytes);
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class FileExchangeService : IFileExchangeService
    {
        private static readonly string RepositoryPath;

        static FileExchangeService()
        {
            var serviceSettings = System.Configuration.ConfigurationManager.GetSection("serviceSettings") as ServiceSettingsSection;
            RepositoryPath = serviceSettings != null ? serviceSettings.RepositoryPath : Path.GetTempPath();
        }

        public List<string> GetFilesList()
        {
            return Directory.GetFiles(RepositoryPath).Select(Path.GetFileName).ToList();
        }

        public FileInfo GetFileInfo(string fileName)
        {
            string filePath = Path.Combine(RepositoryPath, fileName);
            if (File.Exists(filePath))
            {
                FileInfo fi = new FileInfo(filePath);
                return fi;
            }

            return null;
        }

        public byte[] GetFileChunk(string fileName, int offset, int length)
        {
            string filePath = Path.Combine(RepositoryPath, fileName);

            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read)))
            {
                reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                byte[] data = reader.ReadBytes(length);

                return data;
            }
        }

        public ResponseInfo CreateFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                return new ResponseInfo { ErrorCode = ErrorCode.InternalError, Message = "Файл уже существует" };
            }

            File.Create(fileName);
            return new ResponseInfo { ErrorCode = ErrorCode.NoError };
        }

        public ResponseInfo LoadFileChunk(string fileName, byte[] chunk)
        {
            try
            {
                string filePath = Path.Combine(RepositoryPath, fileName);

                using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write)))
                {
                    writer.BaseStream.Seek(0, SeekOrigin.End);
                    writer.Write(chunk);
                }

                return new ResponseInfo { ErrorCode = ErrorCode.NoError };
            }
            catch (Exception e)
            {
                return new ResponseInfo { ErrorCode = ErrorCode.InternalError, Message = e.Message };
            }
        }
    }
}
