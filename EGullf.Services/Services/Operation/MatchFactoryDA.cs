using EGullf.Services.DA.Operation;
using EGullf.Services.Models.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.Services.Operation
{
    public class MatchFactoryDA
    {


        public IMatchDA GetMatch(TypeMatch Type)
        {
            if (Type == TypeMatch.Project)
                return new MatchProjectDA();
            else
                return new MatchProjectDA();
        }



   }
}
