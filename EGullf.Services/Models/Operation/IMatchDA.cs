using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.Models.Operation
{
    public interface IMatchDA
    {
        List<IMatchable> Get(IMatchable Matchable);
    }
}
