using EGullf.Services.DA.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.Services.Configuration
{
    public class SystemVariableServices
    {
        #region "Instances"
       
        #endregion

        public string GetSystemVariableValue(string VariableName)
        {
            SystemVariableDA SystemVariableDA = new SystemVariableDA();
            string resp = SystemVariableDA.GetSystemVariableValue(VariableName);
            return resp;
        }


    }
}
