using EGullf.Services.DA.Operation;
using EGullf.Services.Models.Operation;
using EGullf.Services.Models.Utils;
using System.Collections.Generic;

namespace EGullf.Services.Services.Operation
{
    public class CabinSpecificationServices
    {
        public List<CabinSpecificationModel> GetByReferenceId(int ReferenceId, int Type)
        {
            CabinSpecificationDA specificInformationDA = new CabinSpecificationDA();
            return specificInformationDA.GetByReferenceId(ReferenceId, Type);
        }

        public RequestResult<CabinSpecificationModel> InsUpd(CabinSpecificationModel model)
        {
            CabinSpecificationDA specificInformationDA = new CabinSpecificationDA();
            RequestResult<CabinSpecificationModel> resp = new RequestResult<CabinSpecificationModel>() { Status = Status.Success };
            return specificInformationDA.InsUpd(model);
        }
    }
}
