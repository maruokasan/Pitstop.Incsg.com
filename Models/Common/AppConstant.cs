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
        public static class Publish
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
                public const string HaileckApplication = "Application";
                //Appraisal//
                public const string HaileckAppraisal = "Appraisal";
                //SSP//
                public const string HaileckSSP = "SSP";

            }

            public static class ClaimValue
            {
                //UserMaster//
                public const string SetRoles = "Set Roles";
                public const string CreateEditAccounts = "Create/Edit Accounts";
                public const string SetDeactiveAccountSettings = "Set Deactive Account Settings";
                public const string ViewReports = "View Reports";

                //SSP//
                public const string ViewPublishDashboard = "View Publish Dashboard";
                // public const string PublishAnnouncement = "Publish Announcement";
                // public const string PublishHighlight = "Publish Highlight";
                public const string EditContact = "Edit Contact";


                //Recruitment//
                public const string ViewEmploymentForm = "View Employment Form";
                public const string ViewEmploymentDashboard = "View Employment Dahsboard";
                public const string ViewAssesmentForm = "View Assesment Form";
                public const string ViewRecuitmentDashboard = "View Assesment Dashboard";

                //Appraisal//
                public const string ViewAllForms = "View All Forms";
                public const string ViewAppraisalDashboard = "View Appraisal Dashboard";
                public const string ViewPAFMDashboard = "View PAFM Dashboard";
                public const string ViewPAFDDashboard = "View PAFD Dashboard";
                public const string ViewAPAMDashboard = "View APAM Dashboard";
                public const string ViewAPADDashboard = "View APAD Dashboard";
                public const string ViewPeerAFDashboard = "View PeerAF Dashboard";
                public const string ViewSAFDashboard = "View SAF Dashboard";
                public const string ViewPAFM = "View PAFM";
                public const string ViewPAFD = "View PAFD";
                public const string ViewAPAM = "View APAM";
                public const string ViewAPAD = "View APAD";
                public const string ViewPeerAF = "View PeerAF";
                public const string ViewSAF = "View SAF";

                public const string ViewFormsManager = "View Forms Manager";
                public const string EditJobcode = "Edit Jobcode";
            }
        }

        public static class CookiesName
        {
            public const string Username = "Username";
            public const string UserId = "UserId";
            public const string RolePermission = "RolePermission";
            public const string domain = "haileck.com";
            public const string Jobcode = "Jobcode";
        }
    }
}
