using EGullf.Services.Models.AzureStorage;
using EGullf.Services.Models.Utils;
using System;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace EGullf.Services.DA.AzureStorage
{
    public class FileReferenceDA
    {
        public RequestResult<object> InsUpd(FileReferenceModel model)
        {
            RequestResult<object> ER = new RequestResult<object>();

            using (var db = new EGULFEntities())
            {
                ObjectParameter Id = new ObjectParameter("FileReferenceId", typeof(int?));
                Id.Value = model.FileReferenceId;

                ER = db.sp_InsUpdFileReference(Id, model.FileName, model.Path, model.ContentType)
                    .Select(x => new RequestResult<object>()
                    {
                        Status = (bool)x.IsError ? Status.Error : Status.Success,
                        Message = x.Message
                    }).FirstOrDefault();

                if (ER == null)
                    model.FileReferenceId = Convert.ToInt32(Id.Value.ToString());

                return ER;
            }
        }

        public FileReferenceModel GetByReference(string reference)
        {
            using (var db = new EGULFEntities())
            {
                return db.sp_SelFileByReference(reference).Select(x => new FileReferenceModel()
                {
                    ContentType = x.ContentType,
                    FileName = x.FileName,
                    FileReferenceId = x.FileReferenceId,
                    Path = x.Path
                }).FirstOrDefault();
            }
        }

        public RequestResult<object> Del(string reference)
        {
            RequestResult<object> ER = new RequestResult<object>();

            using (var db = new EGULFEntities())
            {
                ER = db.sp_DelFileByReference(reference)
                 .Select(x => new RequestResult<object>()
                 {
                     Status = (bool)x.IsError ? Status.Error : Status.Success,
                     Message = x.Message
                 }).FirstOrDefault();
            }

            return ER;
        }
    }
}
