using EGullf.Services.Models.Operation;
using EGullf.Services.Models.Utils;
using System;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace EGullf.Services.DA.Operation
{
    public class SpecificInformationDA
    {
        public RequestResult<SpecificInformationModel> InsUpd(SpecificInformationModel model)
        {
            RequestResult<SpecificInformationModel> ER = new RequestResult<SpecificInformationModel>();

            using (var db = new EGULFEntities())
            {
                ObjectParameter Id = new ObjectParameter("MatchableId", typeof(int?));
                Id.Value = model.MatchableId;

                ER = db.sp_InsUpdSpecificInfo(Id, model.BHP, model.SubtypeId, model.BollardPullAhead, 
                    model.BollardPullAstern, model.NumberPassenger, model.AirCondition, model.MessRoom, 
                    model.ControlRoom, model.ConferenceRoom, model.Gymnasium, model.SwimingPool, 
                    model.Office, model.Hospital, model.CargoCapacity, model.PumpRates, model.Type, 
                    model.BollardPull, model.TankCapacity, model.DischargeRate, model.PemexCheck, 
                    model.DeckStrenght)
                    .Select(x => new RequestResult<SpecificInformationModel>()
                    {
                        Status = (bool)x.IsError ? Status.Error : Status.Success,
                        Message = x.Message
                    }).FirstOrDefault();

                if (ER.Status == Status.Success)
                    model.MatchableId = Convert.ToInt32(Id.Value.ToString());

                return ER;
            }
        }

        public SpecificInformationModel GetByReferenceId(int ReferenceId, int Type)
        {
            using (var db = new EGULFEntities())
            {
                var resp = db.sp_SelSpecificInfo(
                 ReferenceId, Type).ToList();

                return (from x in resp
                        select new SpecificInformationModel()
                        {
                            MatchableId = x.MatchableId,
                            BHP = x.BHP,
                            SubtypeId = x.SubtypeId,
                            BollardPullAhead = x.BollardPullAhead,
                            BollardPullAstern = x.BollardPullAstern,
                            NumberPassenger = x.NumberPassenger,
                            AirCondition = x.AirCondition,
                            MessRoom = x.MessRoom,
                            ControlRoom = x.ControlRoom,
                            ConferenceRoom = x.ConferenceRoom,
                            Gymnasium = x.Gym,
                            SwimingPool = x.Pool,
                            Office = x.Office,
                            Hospital = x.Hospital,
                            CargoCapacity = x.CargoCapacity,
                            PumpRates = x.PumpRates,
                            Type = x.Type,
                            BollardPull = x.BollardPull,
                            TankCapacity = x.TankCapacity,
                            DischargeRate = x.DischargeRate,
                            PemexCheck = x.PemexCheck,
                            DeckStrenght = x.DeckStrenght
                        }).FirstOrDefault();
            }
        }
    }
}
