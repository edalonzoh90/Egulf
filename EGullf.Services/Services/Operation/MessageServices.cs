using EGullf.Services.DA.Operation;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Operation;
using EGullf.Services.Models.Utils;
using System;
using System.Collections.Generic;

namespace EGullf.Services.Services.Operation
{
    public class MessageServices
    {
        public RequestResult<MessageModel> InsUpd(MessageModel model)
        {
            MessageDA DA = new MessageDA();
            return DA.InsUpd(model);
        }

        public RequestResult<MessageModel> MarkAsReaded(MessageModel model)
        {
            MessageDA DA = new MessageDA();
            return DA.MarkAsReaded(model);
        }

        public List<MessageModel> Get(MessageModel filter)
        {
            PagerModel pager = new PagerModel();
            pager.Start = 0;
            pager.Offset = Int32.MaxValue - 1;
            return Get(pager, filter);
        }

        public List<MessageModel> Get(PagerModel pager, MessageModel filter)
        {
            MessageDA DA = new MessageDA();
            return DA.Get(pager, filter);
        }
    }
}

