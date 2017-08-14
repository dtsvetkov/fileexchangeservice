using System.Collections;
using System.Configuration;

namespace FileExchangeService.ConfigurationSections
{
    public class ServiceSettingsSection : ConfigurationSection
    {
        #region Private

        private static readonly ConfigurationPropertyCollection _properties;

        private static readonly ConfigurationProperty _repositoryPath;

        private const string RepositoryPathPropertyName = "repositoryPath";

        #endregion

        static ServiceSettingsSection()
        {
            _repositoryPath = new ConfigurationProperty(RepositoryPathPropertyName, typeof(string));

            _properties = new ConfigurationPropertyCollection { _repositoryPath };
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

        #region RepositoryPath

        public string RepositoryPath
        {
            get { return (string)this[RepositoryPathPropertyName]; }
            set { this[RepositoryPathPropertyName] = value; }
        }

        #endregion

        #endregion
    }
}
