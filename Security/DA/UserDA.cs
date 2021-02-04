using Security.Models;
using Security.Sevices;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace Security.DA
{
    public class UserDA
    {
        public Validate ValidateLogin(User entity)
        {
            using (var db = new ModelContainer())
            {
                Validate validate = new Validate();
                ObjectParameter ErrorCode = new ObjectParameter("ErrorCode", typeof(int));
                ObjectParameter Error = new ObjectParameter("Error", typeof(string));

                var collection = db.sp_ValUserLogin(entity.UserName, entity.Password, ErrorCode, Error);

                foreach(var item in collection)
                {
                    entity.PersonId = item.PersonId;
                    entity.CompanyId = item.CompanyId;
                    entity.UserId = item.UserId;
                    Role r = new Role();
                    r.RoleId = item.RoleId;
                    r.RoleName = item.RoleName;
                    entity.Roles.Add(r);
                }

                validate.ErrorCode = Int32.Parse(ErrorCode.Value.ToString());
                validate.Error = Error.Value.ToString();

                return validate;
            }
        }

        public List<string> GetAlertGroupByUsername(string username)
        {
            using (var db = new ModelContainer())
            {
                List<string> lstGroup = new List<string>();

                var collection = db.sp_SelAlertGroupByUserName(username).ToList();

                if (collection.Count() > 0)
                {
                    var first = collection.FirstOrDefault();
                    lstGroup.Add(first.UserGroup);
                    lstGroup.Add(first.RolGroup);
                    lstGroup.Add(first.PersonGroup);
                    if (first.CompanyGroup != null)
                        lstGroup.Add(first.CompanyGroup);
                    foreach (var item in collection)
                    {
                        lstGroup.Add(item.RolGroup);
                        if (item.CompanyRolGroup != null)
                            lstGroup.Add(item.CompanyRolGroup);
                    }
                }

                return lstGroup;
            }
        }

        public List<Resource> GetResourcesByUserId(int UserId)
        {
            using (var db = new ModelContainer())
            {
                var collection = db.sp_SelResourceByUserId(UserId);

                var resp = (from x in collection
                            select new Resource()
                            {
                                DisplayName = x.DisplayName,
                                Icon = x.Icon,
                                Position = x.Position,
                                ResourceId = x.ResourceId,
                                Url = x.Url,
                                ParentResourceId = x.ParentResourceId,
                                AddMenu = x.Menu
                            }).ToList();

                return resp;
            }
        }

        public User SelUser(User parameters)
        {
            using (var db = new ModelContainer())
            {
                User result = db.sp_SelUser(parameters.UserId,
                                            parameters.UserName).Select(x => new User()
                                            {
                                                UserId = x.UserId,
                                                UserName = x.Username,
                                                Password = x.Password
                                            }).FirstOrDefault();

                return result;
            }
        }

        public User insUpdUser(User parameters)
        {
            using (var db = new ModelContainer())
            {
                ObjectParameter UserId = new ObjectParameter("userId", typeof(int?));
                UserId.Value = parameters.UserId;

                //parameters.Password = Crypto.ToBase64(parameters.Password).ToString();
                var result = db.sp_insUpdUser(UserId,
                                              parameters.UserName,
                                              parameters.Password
                                              ).FirstOrDefault();

                if (result != null && result.IsError == true)
                {
                    throw new Exception(result.Message);
                }

                parameters.UserId = Convert.ToInt32(UserId.Value.ToString());
                return parameters;    
            }
        }

        public User valExistingUsername(User parameters)
        {
            using (var db = new ModelContainer())
            {
                ObjectParameter UserId = new ObjectParameter("userId", typeof(int?));
                UserId.Value = parameters.UserId;

                var result = db.sp_valUsername(UserId, parameters.UserName);

                parameters.UserId = Convert.ToInt32(UserId.Value.ToString());
                return parameters;
            }
        }

        public Role GetRole(User user)
        {
            using (var db = new ModelContainer())
            {
                var resp = db.sp_SelRoleByUser(user.UserId).ToList();
                return (from x in resp
                        select new Role()
                        {
                            RoleId = x.RoleId,
                            RoleName = x.RoleName,
                        }).FirstOrDefault();
            }
        }

        public User UpdateRoleUser(int? userId, int? roleId)
        {
            using (var db = new ModelContainer())
            {
                var result = db.sp_InsUpdUserRole(userId, roleId).FirstOrDefault();

                if (result != null && result.IsError == true)
                {
                    throw new Exception(result.Message);
                }
                return new User() { UserId = userId};
            }
        }
    }
}
