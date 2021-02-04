using EGullf.Services.DA.AzureStorage;
using EGullf.Services.Models.AzureStorage;
using EGullf.Services.Models.Utils;
using EGullf.Services.Services.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;

namespace EGullf.Services.Services.AzureStorage
{
    public class FileServices
    {

        private CloudBlobContainer Container()
        {
            SystemVariableServices conf = new SystemVariableServices();
            string AzureStorage = conf.GetSystemVariableValue("AzureStorage");

            string storageConnectionString = "DefaultEndpointsProtocol=https;"
              + AzureStorage
              + ";EndpointSuffix=core.windows.net";

            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();
            return serviceClient.GetContainerReference("files");
        }

        public void UploadFromStream(string reference, Stream fileStream)
        {
            CloudBlockBlob blob = Container().GetBlockBlobReference(reference);
            blob.UploadFromStream(fileStream);
        }

        public MemoryStream DownloadToStream(string reference)
        {
            var blockBlob = Container().GetBlockBlobReference(reference);
            MemoryStream stream = new MemoryStream();
            blockBlob.DownloadToStream(stream);
            return stream;
        }

        public void DeleteStream(string reference)
        {
            var blockBlob = Container().GetBlockBlobReference(reference);
            blockBlob.DeleteIfExists();

        }

        public RequestResult<object> InsUpdFileReference(FileReferenceModel model)
        {
            FileReferenceDA da = new FileReferenceDA();
            return da.InsUpd(model);
        }

        public RequestResult<object> DelFileReference(string reference)
        {
            FileReferenceDA da = new FileReferenceDA();
            return da.Del(reference);
        }

        public FileReferenceModel GetFileReference(string reference)
        {
            FileReferenceDA da = new FileReferenceDA();
            return da.GetByReference(reference);
        }

        public void SaveFile(FileModel model)
        {
            UploadFromStream(model.Reference, model.FileContent);

            var res = InsUpdFileReference(model);
            if (res != null && res.Status == Status.Error)
                throw new Exception(res.Message);
        }

        public FileModel GetFile(string reference)
        {
            FileReferenceModel referenceModel = GetFileReference(reference);
            if (referenceModel == null)
                return null;
            FileModel resp = new FileModel();
            resp.ContentType = referenceModel.ContentType;
            resp.FileName = referenceModel.FileName;
            resp.FileReferenceId = referenceModel.FileReferenceId;
            resp.Path = referenceModel.Path;
            resp.FileContent = DownloadToStream(reference);
            return resp;
        }

        public void DeleteFile(string reference)
        {
            var res = DelFileReference(reference);
            if (res != null && res.Status == Status.Error)
                throw new Exception(res.Message);
            DeleteStream(reference);
        }
    }
}
