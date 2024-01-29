namespace Pitstop.Models
{
    public class UserSettingsModel
    {
        public UserSettingsModel()
        {
            ChangePassword = new ChangePasswordModel();
            ChangeMobileNumber = new ChangeMobileNumberModel();
        }

        public ChangePasswordModel ChangePassword { get; set; }
        public ChangeMobileNumberModel ChangeMobileNumber { get; set; }

        public class ChangePasswordModel
        {
            public string Current { get; set; }
            public string NewPassword { get; set; }
            public string ConfirmNewPassword { get; set; }
        }

        public class ChangeMobileNumberModel
        {
            public string MobileNumber { get; set; }
        }
    }
}
