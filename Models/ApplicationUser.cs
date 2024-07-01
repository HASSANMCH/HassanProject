using Microsoft.AspNetCore.Identity;

namespace HassanProject.Models
{
    public class ApplicationUser :IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string? VerificationCode { get; set; }


    }
}
