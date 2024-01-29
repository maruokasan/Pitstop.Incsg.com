using Pitstop.Models.PitstopData;

namespace Pitstop.Models
{
    public class CreateUpdateUserMasterDataModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string EmailId { get; set; }
        public string MobileNumber { get; set; }
        public string AccessTypeId { get; set; }
        public string AccessTypeName { get; set; }
        public bool IsActive { get; set; }
        public string ApplicationNames { get; set; }
        public string JobcodeId {get; set;}
        public List<string> UserSystemApps { get; set; }
        public List<RoleModel> AccessTypeList { get; set; }

        // Navigation properties
        public ICollection<UserRoles> UserRoles { get; set; }
    }
}
