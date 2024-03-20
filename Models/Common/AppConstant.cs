namespace Pitstop.Models
{
    public static class AppConstant
    {
        public static class SubmitTypeUserSettings
        {
            public const string ChangePassword = "ChangePassword";
            public const string ChangeMobileNumber = "ChangeMobileNumber";
        }

        public static class SubmitCreateUpdateUsers
        {
            public const string Save = "Save";
            public const string Update = "Update";
        }
        public static class SubmitCreateUpdatePosition
        {
            public const string Save = "Save";
            public const string Update = "Update";
        }
        public static class DataControl
        {
            public const string DeactiveAccountInactiveForNumberOfDays = "DeactiveAccountInactiveForNumberOfDays";
            public const string EmailSubmitType = "EmailSubmitType";

            public class EmailSubmitTypeData
            {
                public const string RequestResetPassword = "RequestResetPassword";
                public const string CreateUserPassword = "CreateUserPassword";
                public const string NewGeneratedPassword = "NewGeneratedPassword";
                public const string InactiveForNumberOfDays = "InactiveForNumberOfDays";
                public const string AdminResetPassword = "AdminResetPassword";

            }
        }
        public static class RoleClaim
        {
            public static class ClaimType
            {
                //Recruitment//
                public const string Pitstop = "Pitstop";
            }

            public static class ClaimValue
            {
                //UserMaster//
                public const string SetRoles = "Set Roles";
                public const string CreateEditAccounts = "Create/Edit Accounts";
                public const string SetDeactiveAccountSettings = "Set Deactive Account Settings";
                public const string ViewReports = "View Reports";
            }
        }

        public static class CookiesName
        {
            public const string Username = "Username";
            public const string UserId = "UserId";
            public const string RolePermission = "RolePermission";
            public const string domain = "pitstop.com";
        }
    }
}
