using EGullf.Services.Models.Operation;
using EGullf.Services.Models.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace EGullf.Services.DA.Operation
{
    public class CabinSpecificationDA
    {
        public List<CabinSpecificationModel> GetByReferenceId(int ReferenceId, int Type)
        {
            using (var db = new EGULFEntities())
            {
                var resp = db.sp_SelCabinSpecification(ReferenceId, Type).ToList();

                return (from x in resp
                        select new CabinSpecificationModel()
                        {
                            CabinSpecificationId = x.CabinSpecificationId,
                            ReferenceId = x.ReferenceId,
                            CabinType = x.CabinType,
                            CabinQuantity = x.CabinQuantity,
                            Type = x.Type
                        }).ToList();
            }
        }

        public RequestResult<CabinSpecificationModel> InsUpd(CabinSpecificationModel model)
        {
            RequestResult<CabinSpecificationModel> ER = new RequestResult<CabinSpecificationModel>();

            using (var db = new EGULFEntities())
            {
                ObjectParameter Id = new ObjectParameter("CabinSpecificationId", typeof(int?));
                Id.Value = model.CabinSpecificationId;

                ER = db.sp_InsUpdCabinSpecification(Id, model.ReferenceId, model.CabinType, model.CabinQuantity, model.Type)
                    .Select(x => new RequestResult<CabinSpecificationModel>()
                    {
                        Status = (bool)x.IsError ? Status.Error : Status.Success,
                        Message = x.Message
                    }).FirstOrDefault();

                if (ER.Status == Status.Success)
                    model.CabinSpecificationId = Convert.ToInt32(Id.Value.ToString());

                return ER;
            }
        }
    }
}
