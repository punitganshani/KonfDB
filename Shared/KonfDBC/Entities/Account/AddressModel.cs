#region Namespaces

using System.ComponentModel.DataAnnotations;

#endregion

namespace KonfDB.Infrastructure.Database.Entities.Account
{
    public class AddressModel
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public ContactNumber ContactNumber { get; set; }
    }
}