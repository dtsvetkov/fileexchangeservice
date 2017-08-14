using System.Collections;
using System.Configuration;

namespace FileExchangerClient.ConfigurationSections
{
    public class ClientSettingsSection : ConfigurationSection
    {
        #region Private

        private static readonly ConfigurationPropertyCollection _properties;

        private static readonly ConfigurationProperty _downloadsPath;
        private static readonly ConfigurationProperty _chunkSize;

        private const string DownloadsPathPropertyName = "downloadsPath";
        private const string ChunkSizePropertyName = "chunkSize";

        #endregion

        static ClientSettingsSection()
        {
            _downloadsPath = new ConfigurationProperty(DownloadsPathPropertyName, typeof(string));
            _chunkSize = new ConfigurationProperty(ChunkSizePropertyName, typeof(int));

            _properties = new ConfigurationPropertyCollection { _downloadsPath, _chunkSize };
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get { return _properties; }
        }

        public void ClearCollection()
        {
            Properties.Clear();
        }

        public void RemoveCollectionElement(string elName)
        {
            Properties.Remove(elName);
        }

        public IEnumerator GetCollectionEnumerator()
        {
            return (Properties.GetEnumerator());
        }

        #region Properties

        #region DownloadsPath

        public string DownloadsPath
        {
            get { return (string)this[DownloadsPathPropertyName]; }
            set { this[DownloadsPathPropertyName] = value; }
        }

        #endregion

        #region ChunkSize

        public int ChunkSize
        {
            get { return (int)this[ChunkSizePropertyName]; }
            set { this[ChunkSizePropertyName] = value; }
        }

        #endregion

        #endregion
    }
}
