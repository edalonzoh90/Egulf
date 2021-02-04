using System.IO;

namespace EGullf.Services.Models.AzureStorage
{
    public class FileModel : FileReferenceModel
    {
        string _reference;

        public Stream FileContent { get; set; }

        public string Reference
        {
            set
            {
                _reference = value;
            }
            get
            {
                if (string.IsNullOrEmpty(_reference))
                    return Path + FileName;
                else
                    return _reference;
            }
        }
    }
}
