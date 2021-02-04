using EGullf.Services.DA.Example;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Example;
using System.Collections.Generic;
using System;
using EGullf.Services.Models.Utils;

namespace EGullf.Services.Services.Example
{
    public class NotificationServices
    {
        public List<NotificationModel> Get(PagerModel pager, NotificationModel filter)
        {
            NotificationDA da = new NotificationDA();
            return da.Get(pager, filter);
        }

        public RequestResult<object> InsUpd(NotificationModel model)
        {
            NotificationDA da = new NotificationDA();
            return da.InsUpd(model);
        }
    }
}
