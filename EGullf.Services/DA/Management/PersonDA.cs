using EGullf.Services.Models.Management;
using EGullf.Services.Models.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.DA.Management
{
    public class PersonDA
    {

        public PersonModel GetPerson(int PersonId)
        {
            var UserPersonData = getUserPerson(new UserPersonModel() { PersonId = PersonId }).FirstOrDefault();

            return new PersonModel()
            {
                PersonId = UserPersonData.PersonId,
                FirstName = UserPersonData.FirstName,
                LastName = UserPersonData.LastName,
                UserId  = UserPersonData.UserId,
                PhoneNumber = UserPersonData.PhoneNumber,
                Email = UserPersonData.Email,
                Skype = UserPersonData.Skype,
                FileReferenceId = UserPersonData.FileReferenceId,
                CompanyId = UserPersonData.CompanyId
            };
        }


        public List<UserPersonModel> getUserPerson(UserPersonModel parameters)
        {
            using (var db = new EGULFEntities())
            {
                var result = db.sp_selUserPerson(parameters.PersonId,
                                                 parameters.UserId, parameters.Username, parameters.Email, parameters.CompanyId )
                                                 .Select(x => new UserPersonModel()
                                                 {
                                                     PersonId = x.PersonId,
                                                     FirstName = x.FirstName,
                                                     LastName = x.LastName,
                                                     UserId = (int)x.UserId,
                                                     Username = x.UserName,
                                                     PhoneNumber = x.PhoneNumber,
                                                     Email = x.Email,
                                                     Skype = x.Skype,
                                                     CompanyId = x.CompanyId,
                                                     CompanyName = x.CompanyName,
                                                     FileReferenceId = x.FileReferenceId,
                                                     FileName = x.FileName,
                                                     Path = x.Path,
                                                     ContentType = x.ContentType
                                                 }).ToList();

                return result;
            }
        }


        public RequestResult<object> insUpdPerson(PersonModel parameters)
        {
            using (var db = new EGULFEntities())
            {
                ObjectParameter personId = new ObjectParameter("personId", typeof(int?));
                personId.Value = parameters.PersonId;

                var result = db.sp_insUpdPerson(personId,
                                                parameters.FirstName,
                                                parameters.LastName,
                                                parameters.UserId,
                                                parameters.PhoneNumber,
                                                parameters.Email,
                                                parameters.Skype,
                                                parameters.FileReferenceId,
                                                parameters.CompanyId).Select(x => new ErrorResult()
                                                {
                                                    IsError = x.IsError,
                                                    Message = x.Message,
                                                    Line = x.Line,
                                                    Subject = x.Subject
                                                }).FirstOrDefault();

                if (result != null && !string.IsNullOrEmpty(result.Message))
                {
                    if (result.IsError == true)
                        return new RequestResult<object>() { Status = Status.Error, Message = result.Message };
                    else
                        return new RequestResult<object>() { Status = Status.Warning, Message = result.Message };
                }
                else
                {
                    parameters.PersonId = Convert.ToInt32(personId.Value.ToString());
                    return new RequestResult<object>() { Status = Status.Success, Data = parameters };
                }
            }
        }


        public RequestResult<PersonModel> valExistingEmail(PersonModel parameters)
        {
            using (var db = new EGULFEntities())
            {
                ObjectParameter personId = new ObjectParameter("personId",typeof(int?));
                personId.Value = parameters.PersonId;

                var result = db.sp_valEmail(personId, parameters.Email);

                parameters.PersonId = Convert.ToInt32(personId.Value.ToString());
                return new RequestResult<PersonModel>()
                {
                    Data = parameters
                };
            }
        }


        public RequestResult<object> RemoveUserImage(UserPersonModel parameters)
        {
            using (var db = new EGULFEntities())
            {
                var result = db.sp_DelPersonFileReference(parameters.PersonId, parameters.UserId);
                return new RequestResult<object>(){ Status = Status.Success };
            }
        }






    }
}
