using EGullf.Services.DA.Operation;
using EGullf.Services.Models.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.Services.Operation
{
    public class MatchServices
    {


        public List<IMatchable> GetMatchProject(IMatchable Matchable)
        {
            MatchFactoryDA factoryMatch = new MatchFactoryDA();
            IMatchDA match = factoryMatch.GetMatch(TypeMatch.Project);
            return match.Get(Matchable);
        }



    }
}
