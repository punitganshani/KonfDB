#region Namespaces

using System.ComponentModel.DataAnnotations;
using KonfDB.Infrastructure.Database.Enums;

#endregion

namespace KonfDB.Infrastructure.Database.Entities.Account
{
    public class ContactNumber
    {
        public ContactType Type { get; set; }
        public string Contact { get; set; }
    }
}