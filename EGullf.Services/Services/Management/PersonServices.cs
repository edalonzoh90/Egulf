using EGullf.Services.DA.Management;
using EGullf.Services.Models.AzureStorage;
using EGullf.Services.Models.Management;
using EGullf.Services.Models.Utils;
using EGullf.Services.Models.Mail;
using EGullf.Services.Services.AzureStorage;
using EGullf.Services.Services.Images;
using EGullf.Services.Services.Mail;
using Security.Models;
using Security.Sevices;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Collections.Generic;
using EGullf.Services.Services.Configuration;

namespace EGullf.Services.Services.Management
{
    public class PersonServices
    {

        public RequestResult<object> SingUp(PersonModel personParameters, User userParameters)
        {
            TransactionOptions scopeOptions = new TransactionOptions();
            //scopeOptions.IsolationLevel = IsolationLevel.ReadCommitted;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, scopeOptions))
            {
                try
                {
                    UserServices UserServ = new UserServices();

                    User responseValidateUser = UserServ.valExistingUsername(userParameters);
                    if (responseValidateUser.UserId > 0)
                    {
                        return new RequestResult<object> { Status = Status.Warning, Message = "MsgUserExisting" };
                    }
                    else
                    {
                        userParameters.UserId = 0;
                        User responseInsUpdUser = UserServ.insUpdUser(userParameters);
                        if (responseInsUpdUser.UserId > 0)
                        {
                            RequestResult<PersonModel> personIdEmailExisting = valExistingEmail(personParameters);
                            if (personIdEmailExisting.Data.PersonId > 0)
                            {
                                return new RequestResult<object> { Status = Status.Warning, Message = "MsgEmailExisting" };
                            }
                            else
                            {
                                personParameters.UserId = responseInsUpdUser.UserId;
                                RequestResult<object> personResponse = insUpdPerson(personParameters);
                                if (personResponse.Status == Status.Error)
                                    throw new Exception("An unespected error was detected on create person");


                                MailServices MailServ = new MailServices();
                                ITemplate factory = new TemplateMessagesFactory();
 
                                SystemVariableServices SVS = new SystemVariableServices();
                                Dictionary<string, string[]> param = new Dictionary<string, string[]>();
                                string EgulfUrl = SVS.GetSystemVariableValue("EgulfWeb");                          
                                param.Add("{Btn_url}", new string[] { EgulfUrl });

                                MailServ.SendMail(factory.GetTemplate(personParameters.Email, "WelcomeT",param));
                                //MailServ.wellcomeEmail(personParameters.Email);

                                ts.Complete();
                                return personResponse;
                            }
                        }
                        else
                        {
                            throw new Exception("An unespected error was detected on create user");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    return new RequestResult<object>() { Status = Status.Error, Message = ex.Message };
                }
            }
        }


        public RequestResult<object> ValidateExistingEmail(string Email)
        {
            RequestResult<PersonModel> personIdEmailExisting = valExistingEmail(new PersonModel() { Email = Email});
            if (personIdEmailExisting.Data.PersonId > 0)
            {
                return new RequestResult<object> { Status = Status.Warning };
            }
            else
            {
                return new RequestResult<object> { Status = Status.Success };
            }
        }


        public RequestResult<object> RecoverAccount(string SecureEmail)
        {
            TransactionOptions scopeOptions = new TransactionOptions();
            //scopeOptions.IsolationLevel = IsolationLevel.ReadCommitted;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, scopeOptions))
            {
                try
                {
                    SystemVariableServices SV = new SystemVariableServices();
                    var Key = Crypto.ToBase64(SV.GetSystemVariableValue("CryptoKey")).ToString().Substring(0, 16);
                    var Iv = Crypto.ToBase64(SV.GetSystemVariableValue("CryptoIV")).ToString().Substring(0, 16);
                    var Email = Crypto.DecryptString(SecureEmail, Key, Iv);

                    RequestResult<PersonModel> ValidAccount = valExistingEmail(new PersonModel() { Email = Email });
                    if (ValidAccount.Data.PersonId > 0)
                    {
                        PersonModel PersonData = GetPerson((int)ValidAccount.Data.PersonId);

                        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
                        Random randomChar = new Random();
                        var NewPassword = new string(Enumerable.Repeat(chars, 10).Select(s => s[randomChar.Next(s.Length)]).ToArray());
                        var NewSecurePassword = Crypto.EncryptString(NewPassword, Key,Iv);

                        UserServices UserServ = new UserServices();
                        User userData = UserServ.SelUser(new User() { UserId = PersonData.UserId });
                        userData.Password = NewSecurePassword;
                        User responseInsUpdUser = UserServ.insUpdUser(userData);
                        if (responseInsUpdUser.UserId > 0)
                        {
                            MailServices MailServ = new MailServices();
                            ITemplate factory = new TemplateMessagesFactory();

                            SystemVariableServices SVS = new SystemVariableServices();
                            Dictionary<string, string[]> param = new Dictionary<string, string[]>();
                            string EgulfUrl = SVS.GetSystemVariableValue("EgulfWeb");
                            param.Add("{Btn_url}", new string[] { EgulfUrl });
                            param.Add("{Text}", new string[] { userData.UserName, NewPassword });

                            MailServ.SendMail(factory.GetTemplate(Email, "ResetAccount", param));

                            ts.Complete();
                            return new RequestResult<object> { Status = Status.Success };
                        }
                        else
                        {
                            throw new Exception("An unexpected error occurred at restore account process");
                        }
                    }
                    else
                    {
                        return new RequestResult<object> { Status = Status.Warning };
                    }
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    return new RequestResult<object> { Status = Status.Error, Message = ex.Message };
                }
            }
        }


        public RequestResult<object> ValidateExistingUsername(string Username)
        {
            UserServices UserServ = new UserServices();
            User responseValidateUser = UserServ.valExistingUsername(new User() { UserName = Username });
            if (responseValidateUser.UserId > 0)
            {
                return new RequestResult<object> { Status = Status.Warning };
            }
            else
            {
                return new RequestResult<object> { Status = Status.Success };
            }
        }


        public RequestResult<object> SaveProfile(UserPersonModel parameters)
        {
            TransactionOptions scopeOptions = new TransactionOptions();
            //scopeOptions.IsolationLevel = IsolationLevel.ReadCommitted;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, scopeOptions))
            {
                try
                {

                    FileModel parametersFile = new FileModel();
                    if (parameters.File != null && parameters.File.Length > 0)
                    {
                        ImagesServices ImageServ = new ImagesServices();
                        Stream ProcessedImage = ImageServ.ResizeProfileImage(parameters.File);
                        ProcessedImage.Position = 0;

                        var FileNameExtension = ".jpg";//Path.GetExtension(parameters.FileName);
                        parameters.FileName = "profileimage-" + parameters.UserId + FileNameExtension;
                        var ProfileImgPath = "users/" + parameters.UserId + "/images/profile/";
                      
                        parametersFile.FileReferenceId = parameters.FileReferenceId ?? 0;
                        parametersFile.FileName = parameters.FileName;
                        parametersFile.ContentType = "image/jpeg";//parameters.ContentType;
                        parametersFile.Path = ProfileImgPath;
                        parametersFile.FileContent = ProcessedImage;//parameters.File;

                        FileServices FileServ = new FileServices();
                        FileServ.SaveFile(parametersFile);

                        parameters.FileReferenceId = parametersFile.FileReferenceId;
                    }

                    //here we send person data
                    PersonDA PersonDA = new PersonDA();
                    PersonModel parametersPerson = new PersonModel();
                    parametersPerson.PersonId = parameters.PersonId;
                    parametersPerson.FirstName = parameters.FirstName;
                    parametersPerson.LastName = parameters.LastName;
                    parametersPerson.Email = parameters.Email;
                    parametersPerson.PhoneNumber = parameters.PhoneNumber;
                    parametersPerson.Skype = parameters.Skype;
                    parametersPerson.UserId = parameters.UserId;
                    parametersPerson.CompanyId = parameters.CompanyId;
                    parametersPerson.FileReferenceId = parameters.FileReferenceId;
                    RequestResult<object> responsePerson = PersonDA.insUpdPerson(parametersPerson);
                    if (responsePerson.Status == Status.Error)
                        throw new Exception(responsePerson.Message);

                    ts.Complete();
                    return responsePerson;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    return new RequestResult<object>() { Status = Status.Error, Message = ex.Message };
                }
            }
        }


        public FileModel GetUserImage(int UserId)
        {
            UserPersonModel parametersUserPerson = new UserPersonModel();
            parametersUserPerson.UserId = UserId;
            var responseUserPersonData = getFirstUserPerson(parametersUserPerson);

            if (!string.IsNullOrEmpty(responseUserPersonData.FileName) && !String.IsNullOrEmpty(responseUserPersonData.Path))
            {
                FileServices FileServ = new FileServices();
                var FileData = FileServ.GetFile(responseUserPersonData.Path + responseUserPersonData.FileName);
                if (!string.IsNullOrEmpty(FileData.FileName))
                {
                    return FileData;
                }
            }

            return null;
        }


        public RequestResult<object> DeleteUserImage(int UserId)
        {
            TransactionOptions scopeOptions = new TransactionOptions();
            //scopeOptions.IsolationLevel = IsolationLevel.ReadCommitted;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, scopeOptions))
            {
                try
                {
                    RequestResult<object> result = new RequestResult<object>();
                    UserPersonModel parametersUserPerson = new UserPersonModel();
                    parametersUserPerson.UserId = UserId;
                    var responseUserPersonData = getFirstUserPerson(parametersUserPerson);

                    if (responseUserPersonData.FileReferenceId != null || (!string.IsNullOrEmpty(responseUserPersonData.FileName) && !String.IsNullOrEmpty(responseUserPersonData.Path)))
                    {
                        result = RemoveUserImage(new UserPersonModel() { UserId = UserId });

                        FileServices FileServ = new FileServices();
                        FileServ.DeleteFile(responseUserPersonData.Path + responseUserPersonData.FileName);
                
                        ts.Complete();
                        return result;
                    }
                    return new RequestResult<object>() { Status = Status.Success };
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    return new RequestResult<object>() { Status = Status.Error, Message = ex.Message };
                }

            }
        }


        public RequestResult<object> RemoveUserImage(UserPersonModel parameters)
        {
            PersonDA PersonDA = new PersonDA();
            return PersonDA.RemoveUserImage(parameters);
        }

        public UserPersonModel getFirstUserPerson(UserPersonModel parameters)
        {
            PersonDA PersonDA = new PersonDA();
            var resp = PersonDA.getUserPerson(parameters);

            return resp.FirstOrDefault();
        }

        public List<UserPersonModel> getUserPerson(UserPersonModel parameters)
        {
            PersonDA PersonDA = new PersonDA();
            var resp = PersonDA.getUserPerson(parameters);

            return resp;
        }


        public PersonModel GetPerson(int PersonId)
        {
            PersonDA personDA = new PersonDA();
            return personDA.GetPerson(PersonId);
        }


        public RequestResult<object> insUpdPerson(PersonModel parameters)
        {
            PersonDA PersonDA = new PersonDA();
            RequestResult<object> response = PersonDA.insUpdPerson(parameters);
            return response;
        }


        public RequestResult<PersonModel> valExistingEmail(PersonModel parameters)
        {
            PersonDA PersonDA = new PersonDA();
            RequestResult<PersonModel> response = PersonDA.valExistingEmail(parameters);
            return response;
        }






    }


}
