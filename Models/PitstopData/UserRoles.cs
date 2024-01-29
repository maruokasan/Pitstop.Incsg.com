using System.ComponentModel.DataAnnotations;

namespace Pitstop.Models.PitstopData
{
    public class UserRoles
    {
        [Key]
        public string UserId { get; set; }
        [Key]
        public string RoleId { get; set; }

         public User User { get; set; }
    }
}
