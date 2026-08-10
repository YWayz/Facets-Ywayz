using Facets.Core.Security.Claims;

namespace Facets.Core.Security.AuthPolicies;

public sealed class ApplicationAuthPolicy
{
    public const string HasAccessToEvent = "HasAccessToEvent";
    public const string PublicSiteUser = "PublicSiteUser";

    public sealed class UserPolicy
    {
        public const string Create = ApplicationClaimValues.User.Create;
        public const string View = ApplicationClaimValues.User.View;
        public const string Edit = ApplicationClaimValues.User.Edit;
        public const string Delete = ApplicationClaimValues.User.Delete;
    }

    public sealed class RolePolicy
    {
        public const string Create = ApplicationClaimValues.Role.Create;
        public const string View = ApplicationClaimValues.Role.View;
        public const string Delete = ApplicationClaimValues.Role.Delete;
        public const string Update = ApplicationClaimValues.Role.Update;
    }

    public sealed class EventPolicy
    {
        public const string Create = ApplicationClaimValues.Event.Create;
        public const string View = ApplicationClaimValues.Event.View;
        public const string Delete = ApplicationClaimValues.Event.Delete;
        public const string Edit = ApplicationClaimValues.Event.Edit;
        public const string ToggleEventStatus = ApplicationClaimValues.Event.ToggleEventStatus;
    }

    public sealed class RegistrationCounterPolicy
    {
        public const string Create = ApplicationClaimValues.RegistrationCounter.Create;
        public const string View = ApplicationClaimValues.RegistrationCounter.View;
        public const string Delete = ApplicationClaimValues.RegistrationCounter.Delete;
        public const string Edit = ApplicationClaimValues.RegistrationCounter.Edit;
        public const string Unlock = ApplicationClaimValues.RegistrationCounter.Unlock;
    }

    public sealed class PassCategoryPolicy
    {
        public const string CreatePassCategory = ApplicationClaimValues.PassCategory.CreatePassCategory;
        public const string ViewPassCategory = ApplicationClaimValues.PassCategory.ViewPassCategory;
        public const string DeletePassCategory = ApplicationClaimValues.PassCategory.DeletePassCategory;
        public const string EditPassCategory = ApplicationClaimValues.PassCategory.EditPassCategory;
        public const string ViewPassRate = ApplicationClaimValues.PassCategory.ViewPassRate;
        public const string EditPassRate = ApplicationClaimValues.PassCategory.EditPassRate;
        public const string Manage = ApplicationClaimValues.PassCategory.ManagePassTemplate;
    }

    public sealed class VisitorPolicy
    {
        public const string OnsiteRegistration = ApplicationClaimValues.VisitorRegistration.OnsiteRegister;
        public const string OnsiteUpdate = ApplicationClaimValues.VisitorRegistration.OnsiteUpdate;
        public const string View = ApplicationClaimValues.VisitorRegistration.View;
        public const string Blacklist = ApplicationClaimValues.Visitor.BlackList;
        public const string CancelRegistration = ApplicationClaimValues.VisitorRegistration.CancelVisitorRegistration;
    }

    public sealed class TeamMemberPolicy
    {
        public const string Register = ApplicationClaimValues.TeamMemberRegistration.Register;
        public const string UploadAttachments = ApplicationClaimValues.TeamMemberRegistration.UploadAttachments;
        public const string ViewAttachments = ApplicationClaimValues.TeamMemberRegistration.ViewAttachments;
        public const string View = ApplicationClaimValues.TeamMemberRegistration.View;
        public const string UpdateTeamMember = ApplicationClaimValues.TeamMemberRegistration.UpdateTeamMember;
        public const string GenerateTeamMemberPass = ApplicationClaimValues.TeamMemberRegistration.GenerateTeamMemberPass;
        public const string CancelTeamMemberRegistration = ApplicationClaimValues.TeamMemberRegistration.CancelTeamMemberRegistration;
    }

    public sealed class ReportPolicy
    {
        public const string VisitorReport = ApplicationClaimValues.Report.GenerateVisitorListReport;
        public const string CollectionReport = ApplicationClaimValues.Report.GenerateCollectionReport;
    }

    public sealed class PassGenerationPolicy
    {
        public const string PassGenerationView = ApplicationClaimValues.PassGeneration.PassGenerationView;
        public const string VisitorPassGeneration = ApplicationClaimValues.PassGeneration.VisitorPassGeneration;
        public const string PassVerification = ApplicationClaimValues.PassGeneration.PassVerification;
        public const string TeamMemberPassGeneration = ApplicationClaimValues.PassGeneration.TeamMemberPassGeneration;
    }

    public sealed class PavilionPolicy
    {
        public const string Create = ApplicationClaimValues.Pavilion.Create;
        public const string View = ApplicationClaimValues.Pavilion.View;
        public const string Delete = ApplicationClaimValues.Pavilion.Delete;
        public const string Edit = ApplicationClaimValues.Pavilion.Edit;
        public const string CreatePavilionSession = ApplicationClaimValues.Pavilion.CreatePavilionSession;
        public const string ViewPavilionSession = ApplicationClaimValues.Pavilion.ViewPavilionSession;
        public const string EditPavilionSession = ApplicationClaimValues.Pavilion.EditPavilionSession;
        public const string TogglePavilionStatus = ApplicationClaimValues.Pavilion.TogglePavilionStatus;
        public const string SessionDelete = ApplicationClaimValues.Pavilion.SessionDelete;
        public const string RateView = ApplicationClaimValues.Pavilion.RateView;
        public const string EditPassPavilionRate = ApplicationClaimValues.Pavilion.EditPassPavilionRate;
    }

    public sealed class PavilionVisitorPolicy
    {
        public const string View = ApplicationClaimValues.PavilionVisitor.View;
    }

    public sealed class PaymentSettingPolicy
    {
        public const string PaymentSettingsUpdate = ApplicationClaimValues.PaymentSetting.PaymentSettingsUpdate;
    }
}
