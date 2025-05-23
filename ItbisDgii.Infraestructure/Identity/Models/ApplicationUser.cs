using Microsoft.AspNetCore.Identity;

namespace ItbisDgii.Infraestructure.Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
    }
}
