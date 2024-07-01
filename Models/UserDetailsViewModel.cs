using System.Collections.Generic;

namespace HassanProject.Models
{
    public class UserDetailsViewModel
    {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public bool EmailConfirmed { get; set; }
            public List<string> Roles { get; set; }

       
    }
}
