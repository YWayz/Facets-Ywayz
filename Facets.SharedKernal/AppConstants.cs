namespace Facets.SharedKernal;

public static class AppConstants
{
    public const string SriLankaTimeZone = "Sri Lanka Standard Time";

    public static class SuperAdmin
    {
        public static readonly Guid SuperUserId = Guid.Parse("b74ddd14-6340-4840-95c2-db12554843e5");
        public static readonly Guid SuperAdminRoleId = Guid.Parse("e24f4cd1-0759-440e-9a2b-6072880392b6");
        public const string SuperAdminRoleName = "superadmin";
    }

    public static class Database
    {
        public const string APIDbConnectionName = "MSSQLDbConnection";
    }

    public static class StringLengths
    {
        public const int FirstName = 250;
        public const int LastName = 250;
        public const int Email = 256;
        public const int PhoneNumber = 20;
        public const int PostalCode = 128;
        public const int Address = 1024;
        public const int State = 256;
        public const int City = 256;
        public const int Province = 256;
        public const int Notes = 1000;
        public const int Description = 1500;
        public const int IdentityNumber = 64;
        public const int ShortDescription = 512;
        public const int Color = 20;
    }


    public static class Administrator
    {
        public const string RoleName = "Administrator";
    }

    public static class PassType
    {
        public const string Visitor = "Visitor";
        public const string TeamMember = "Team Member";
    }

    public static class QueueStorage
    {
        public static class QueueName
        {
            public const string EmailQueue = "email-queue";
            public const string SMSQueue = "sms-queue";
        }
    }
    public static class FileExtension
    {
        public const string HTML = ".html";
        public const string PDF = ".pdf";

        public static readonly string[] ValidImageFileExtensions = { ".jpeg", ".jpg", ".png" };

    }

    public static class BlobStorage
    {
        public static class ContainerName
        {
            public const string EventLogos = "event-logos";
            public const string TeamMemberProfileImage = "team-member-profile-image";
            public const string UserProfileImages = "user-profile-images";

            public const string TeamMemberDocuments = "team-member-documents";
            public const string VisitorDocuments = "visitor-documents";
        }
    }

    public static class OTP
    {
        public const int ValidMinutes = 5;
        public const int Length = 6;
        public const string Characters = "0123456789";
    }

    public static class PublicSite
    {
        public const string PublicSiteUserId = "Public site user";
    }

    public static class RegistrationMethod
    {
        public const string Online = "Online";
        public const string OnSite = "Live";
    }

    public static class TextIt
    {
        public const string Success = "OK";

        public const int MaximumLengthOfSriLankaPhoneNumberWithoutCountryCode = 10;
    }

    public static class OnePay
    {
        public const string ApplicableCurrency = "LKR";

        public const decimal MinimumAmount = 100M;

        public static class ResponseCodes
        {
            public const int SuccessCode = 200; // was 1000 (v1) — v3 body success code
            public const int UnauthorizedDueToYourEncryptionIssue = 1001;
            public const int InvalidCserCredentials = 1002;
            public const int PleaseProvideRequiredData = 1003;
            public const int TransactionReferenceNumberLengthMustBe_10_20 = 1004;
            public const int InternalServerError = 1005;
            public const int MerchantInInvalidState = 1006;
            public const int MerchantNotSubscribeForThisService = 1007;
            public const int InvalidAppConfigurations = 1014;
            public const int DuplicateTransactionReference = 1015;
            public const int InvalidPhoneNumber = 1016;
            public const int InvalidEmailAddress = 1017;
            public const int InvalidURL = 1018;
            public const int TransactionsDoNotExist = 1019;
        }
    }
}
