using Security.DA;
using Security.Models;
using System.Collections.Generic;
using System.Linq;

namespace Security.Sevices
{
    public class UserServices
    {
        UserDA da = new UserDA();

        public Validate ValidateLogin(User user)
        {
            Validate validate = da.ValidateLogin(user);

            if (validate.ErrorCode == 0)
                validate.Data = GetResourcesByUserId((int)user.UserId);

            return validate;
        }


        public List<Resource> GetResourcesByUserId(int UserId)
        {
            return da.GetResourcesByUserId(UserId);
        }

        public List<string> GetAlertGroupByUsername(string username)
        {
            return da.GetAlertGroupByUsername(username);
        }

        //Get user menu by rol resource defined by security system
        public List<Resource> GetUserMenu(int UserId)
        {
            //we get the resurces list from database
            List<Resource> Data = GetResourcesByUserId(UserId);
            List<Resource> Menu = new List<Resource>();

            //we separate the list by parends and children
            List<Resource> Parents = (from i in Data.ToList()
                                      where i.ParentResourceId == null
                                      && i.AddMenu == true
                                      select new Resource() {
                                          ParentResourceId = i.ParentResourceId,
                                          ResourceId = i.ResourceId,
                                          DisplayName = i.DisplayName,
                                          Icon = i.Icon,
                                          Url = i.Url,
                                          Position = i.Position,
                                          Children = i.Children
                                      }).ToList();

            List<Resource> Children = (from i in Data.ToList()
                                       where i.ParentResourceId != null
                                       && i.ParentResourceId > 0
                                       select new Resource()
                                       {
                                           ParentResourceId = i.ParentResourceId,
                                           ResourceId = i.ResourceId,
                                           DisplayName = i.DisplayName,
                                           Icon = i.Icon,
                                           Url = i.Url,
                                           Position = i.Position,
                                           Children = i.Children
                                       }).ToList();

            //we iterate each item in parents and we get his childrens with a recursive function
            foreach (var Parent in Parents)
            {
                Resource MenuItem = ProcessMenu(Parent, Children);
                Menu.Add(MenuItem);
            }

            return Menu;
        }


        //Recursive function to get children of parent items in menu
        private Resource ProcessMenu(Resource Parent, List<Resource> Children)
        {
            Resource MenuItem = Parent;

            int ParentId = (int)MenuItem.ResourceId;
            List<Resource> ParentChildren = (from i in Children.ToList()
                                             where i.ParentResourceId == ParentId
                                             select new Resource()
                                             {
                                                 ParentResourceId = i.ParentResourceId,
                                                 ResourceId = i.ResourceId,
                                                 DisplayName = i.DisplayName,
                                                 Icon = i.Icon,
                                                 Url = i.Url,
                                                 Position = i.Position,
                                                 Children = i.Children
                                             }).ToList();

            if (ParentChildren.Count() > 0)
            {
                foreach (var Child in ParentChildren)
                {
                    Child.Children = ProcessMenu(Child, Children).Children;
                }

                MenuItem.Children = ParentChildren;
            };

            return MenuItem;
        }

        public User SelUser(User parameters)
        {
            UserDA UserDA = new UserDA();
            return UserDA.SelUser(parameters);
        }

        public User insUpdUser(User parameters)
        {
            UserDA UserDA = new UserDA();
            var result = UserDA.insUpdUser(parameters);
            return result;
        }

        public User valExistingUsername(User parameters)
        {
            UserDA UserDA = new UserDA();
            var result = UserDA.valExistingUsername(parameters);
            return result;
        }

        public User ChangePassword(User parameters)
        {
            User UserInfo = SelUser(parameters);
            if (UserInfo.Password == parameters.Password)
            {
                UserInfo.Password = parameters.NewPassword;
                var result = insUpdUser(UserInfo);
                return result;
            }
            else
            {
                return null;
            }
        }

        public Role GetRole(User user)
        {
            UserDA da = new UserDA();
            return da.GetRole(user);
        }
        
        public User UpdateRoleUser(int? userId, int? roleId)
        {
            UserDA da = new UserDA();
            return da.UpdateRoleUser(userId, roleId);
        }


    }
}
