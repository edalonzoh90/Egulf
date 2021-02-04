using EGULF.Helpers;
using EGULF.Hubs;
using EGullf.Services.Models.Operation;
using EGullf.Services.Models.Utils;
using EGullf.Services.Services.Operation;
using Microsoft.AspNet.SignalR;
using System.Web.Mvc;
using static EGULF.Filters.SecurityFilterAction;

namespace EGULF.Controllers
{
    public class ChatController : Controller
    {
        // GET: Message
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Partial(int ReferenceId)
        {
            MessageServices services = new MessageServices();
            OfferServices offerServices = new OfferServices();
            MessageServices chatServices = new MessageServices();
            MessageModel filter = new MessageModel();

            chatServices.MarkAsReaded(
                new MessageModel() { ReferenceId = ReferenceId, From = SessionWeb.User.PersonId });

            OfferModel offer = offerServices.GetFirst(new OfferModel() { OfferId = ReferenceId });

            ViewBag.Alias = SessionWeb.User.PersonId == offer.ProjectAdmin.PersonId ? "PROJECT_OWNER"
                : SessionWeb.User.PersonId == offer.VesselAdmin.PersonId ? "VESSEL_OWNER" : "";
            ViewBag.From = SessionWeb.User.PersonId;
            ViewBag.To = SessionWeb.User.PersonId == offer.ProjectAdmin.PersonId ? offer.VesselAdmin.PersonId
                : SessionWeb.User.PersonId == offer.VesselAdmin.PersonId ? offer.ProjectAdmin.PersonId : -1;
            ViewBag.lstMsg = services.Get(filter);
            ViewBag.SessionPersonId = SessionWeb.User.PersonId;
            ViewBag.ReferenceId = ReferenceId;

            return View();
        }

        [HttpPost]
        [Session]
        public ActionResult Transaction(MessageModel model)
        {

            RequestResult<MessageModel> result = new RequestResult<MessageModel>() { Status = EGullf.Services.Models.Utils.Status.Success };
            MessageServices services = new MessageServices();
            result.Data = services.InsUpd(model).Data;

            if (result.Status == Status.Success)
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<AlertHub>();
                context.Clients.Group(string.Format("P{0}", result.Data.To))
                    .newMessage(result.Data);
            }

            return Json(result);
        }
    }
}