﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Security.DA
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ModelContainer : DbContext
    {
        public ModelContainer()
            : base("name=ModelContainer")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
    
        public virtual ObjectResult<sp_SelResourceByUserId_Result> sp_SelResourceByUserId(Nullable<int> userId)
        {
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SelResourceByUserId_Result>("sp_SelResourceByUserId", userIdParameter);
        }
    
        public virtual ObjectResult<sp_ValUserLogin_Result> sp_ValUserLogin(string userName, string password, ObjectParameter errorCode, ObjectParameter error)
        {
            var userNameParameter = userName != null ?
                new ObjectParameter("UserName", userName) :
                new ObjectParameter("UserName", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("Password", password) :
                new ObjectParameter("Password", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_ValUserLogin_Result>("sp_ValUserLogin", userNameParameter, passwordParameter, errorCode, error);
        }
    
        public virtual ObjectResult<sp_insUpdUser_Result> sp_insUpdUser(ObjectParameter userId, string userName, string password)
        {
            var userNameParameter = userName != null ?
                new ObjectParameter("UserName", userName) :
                new ObjectParameter("UserName", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("Password", password) :
                new ObjectParameter("Password", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_insUpdUser_Result>("sp_insUpdUser", userId, userNameParameter, passwordParameter);
        }
    
        public virtual int sp_valUsername(ObjectParameter userId, string userName)
        {
            var userNameParameter = userName != null ?
                new ObjectParameter("UserName", userName) :
                new ObjectParameter("UserName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_valUsername", userId, userNameParameter);
        }
    
        public virtual ObjectResult<sp_SelAlertGroupByUserName_Result> sp_SelAlertGroupByUserName(string userName)
        {
            var userNameParameter = userName != null ?
                new ObjectParameter("UserName", userName) :
                new ObjectParameter("UserName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SelAlertGroupByUserName_Result>("sp_SelAlertGroupByUserName", userNameParameter);
        }
    
        public virtual ObjectResult<sp_SelUser_Result> sp_SelUser(Nullable<int> userId, string username)
        {
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            var usernameParameter = username != null ?
                new ObjectParameter("Username", username) :
                new ObjectParameter("Username", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SelUser_Result>("sp_SelUser", userIdParameter, usernameParameter);
        }
    
        public virtual ObjectResult<sp_SelRoleByUser_Result> sp_SelRoleByUser(Nullable<int> userId)
        {
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SelRoleByUser_Result>("sp_SelRoleByUser", userIdParameter);
        }
    
        public virtual ObjectResult<sp_InsUpdUserRole_Result> sp_InsUpdUserRole(Nullable<int> userId, Nullable<int> roleId)
        {
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            var roleIdParameter = roleId.HasValue ?
                new ObjectParameter("RoleId", roleId) :
                new ObjectParameter("RoleId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_InsUpdUserRole_Result>("sp_InsUpdUserRole", userIdParameter, roleIdParameter);
        }
    }
}