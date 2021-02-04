using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.DA.Configuration
{
    public class SystemVariableDA
    {


        public string GetSystemVariableValue(string VariableName)
        {
            using (var db = new EGULFEntities())
            {
                SystemVariable VariableData = new SystemVariable();


                VariableData = (from v in db.SystemVariable.ToList()
                                where v.VariableName == VariableName
                                select new SystemVariable
                                {
                                    VariableId = v.VariableId,
                                    VariableName = v.VariableName,
                                    Value = v.Value,
                                    Entity = v.Entity,
                                    Description = v.Description
                                }).FirstOrDefault();

                if (VariableData.VariableName != null)
                {
                    return VariableData.Value;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
