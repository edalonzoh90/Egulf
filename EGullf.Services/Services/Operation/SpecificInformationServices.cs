using EGullf.Services.DA.Operation;
using EGullf.Services.Models.Operation;
using EGullf.Services.Models.Utils;

namespace EGullf.Services.Services.Operation
{
    public class SpecificInformationServices
    {
        public RequestResult<SpecificInformationModel> InsUpd(SpecificInformationModel model)
        {
            SpecificInformationDA specificInformationDA = new SpecificInformationDA();
            RequestResult<SpecificInformationModel> resp = new RequestResult<SpecificInformationModel>() { Status = Status.Success };
            return specificInformationDA.InsUpd(model);
        }

        public SpecificInformationModel GetByReferenceId(int ReferenceId, int TypeId)
        {
            SpecificInformationDA specificInformationDA = new SpecificInformationDA();
            return specificInformationDA.GetByReferenceId(ReferenceId, TypeId);
        }
    }
}
