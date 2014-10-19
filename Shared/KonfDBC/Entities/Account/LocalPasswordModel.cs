#region Namespaces

using System.ComponentModel.DataAnnotations;

#endregion

namespace KonfDB.Infrastructure.Database.Entities.Account
{
    public class LocalPasswordModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}