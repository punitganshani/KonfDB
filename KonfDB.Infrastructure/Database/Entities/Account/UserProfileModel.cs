#region Namespaces

using System;
using System.ComponentModel.DataAnnotations;

#endregion

namespace KonfDB.Infrastructure.Database.Entities.Account
{
    public class UserProfileModel
    {
        public UserProfileModel()
        {
            Address = new AddressModel();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public AddressModel Address { get; set; }
        #region Metadata
        public int UserId { get; set; }
        public DateTime DateOfRegistration { get; set; }
        public ContactNumber ResidentialNumber { get; set; }
        public ContactNumber OfficeNumber { get; set; }
        #endregion
    }
}