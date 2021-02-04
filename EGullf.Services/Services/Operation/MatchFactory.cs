using EGullf.Services.Models.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.Services.Operation
{
    public class MatchFactory
    {


        public IMatchable GetMatchable(TypeMatch Type)
        {
            if (Type == TypeMatch.Project)
                return new MatchModel();
            else
                return new MatchModel();
        }


    }
}
