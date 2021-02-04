using EGullf.Services.Models.Management;
using EGullf.Services.Models.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace EGullf.Services.DA.Management
{
    public class VesselAvailabilityDA
    {
        public RequestResult<VesselAvailabilityModel> InsUpd(VesselAvailabilityModel model)
        {
            RequestResult<VesselAvailabilityModel> ER = new RequestResult<VesselAvailabilityModel>() { Status = Status.Success };

            using (var db = new EGULFEntities())
            {
                ObjectParameter Id = new ObjectParameter("AvailabilityVesselId", typeof(int?));
                Id.Value = model.AvailabilityVesselId;

                ER = db.sp_InsUpdAvailabilityVessel(
                 Id, model.VesselId, model.StartDate, model.EndDate, model.ReasonId, model.ReasonDescription
                ).Select(x => new RequestResult<VesselAvailabilityModel>()
                {
                    Status = (bool)x.IsError ? Status.Error : Status.Success,
                    Message = x.Message,
                    Data = model
                }).FirstOrDefault();

                if (ER.Status == Status.Success)
                    ER.Data.VesselId = Convert.ToInt32(Id.Value.ToString());

                return ER;
            }
        }

        public List<VesselAvailabilityModel> Get(VesselAvailabilityModel filter)
        {
            using (var db = new EGULFEntities())
            {
                var data = db.sp_SelAvailabilityVesselById(filter.AvailabilityVesselId, filter.VesselId);

                return (from x in data
                        select new VesselAvailabilityModel()
                        {
                            AvailabilityVesselId = x.AvailabilityVesselId,
                            VesselId = x.VesselId,
                            StartDate = x.StartDate,
                            EndDate = x.EndDate,
                            ReasonId = x.ReasonId,
                            Reason = x.Reason,
                            ReasonDescription = x.ReasonDescription,

                        }).ToList();
            }
        }

        public RequestResult<VesselAvailabilityModel> Del(int Id)
        {
            RequestResult<VesselAvailabilityModel> ER = new RequestResult<VesselAvailabilityModel>() { Status = Status.Success };

            using (var db = new EGULFEntities())
            {
                ER = db.sp_DelAvailabilityVessel(Id).Select(x => new RequestResult<VesselAvailabilityModel>()
                {
                    Status = (bool)x.IsError ? Status.Error : Status.Success,
                    Message = x.Message
                }).FirstOrDefault();

                return ER;
            }
        }
    }
}
