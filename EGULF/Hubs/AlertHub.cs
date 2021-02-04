using Microsoft.AspNet.SignalR;
using Security.Sevices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EGULF.Hubs
{
    public class AlertHub : Hub
    {

        public void BroadcastMessage(string Message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<AlertHub>();
            context.Clients.All.broadcastMessage(Message);
        }

        //public void SendAlert(string who, string message)
        //{
        //    Clients.Group(who).newMessage(message);
        //}

        public override Task OnConnected()
        {
            UserServices service = new UserServices();
            List<string> lstGroups = service.GetAlertGroupByUsername(Context.User.Identity.Name);
            foreach (string group in lstGroups) 
                Groups.Add(Context.ConnectionId, group);

            return base.OnConnected();
        }

    }
}