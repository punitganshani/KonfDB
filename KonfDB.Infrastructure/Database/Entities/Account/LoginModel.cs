#region Namespaces



#endregion

namespace KonfDB.Infrastructure.Database.Entities.Account
{
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}