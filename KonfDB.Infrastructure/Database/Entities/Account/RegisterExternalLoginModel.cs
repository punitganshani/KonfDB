#region Namespaces

using System.ComponentModel.DataAnnotations;

#endregion

namespace KonfDB.Infrastructure.Database.Entities.Account
{
    public class RegisterExternalLoginModel
    {
        public string UserName { get; set; }
        public string ExternalLoginData { get; set; }
    }
}