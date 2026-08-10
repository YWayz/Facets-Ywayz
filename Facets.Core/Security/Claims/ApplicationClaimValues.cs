namespace Facets.Core.Security.Claims;

public sealed class ApplicationClaimValues
{
    public sealed class SuperAdmin
    {
        public const string All = "all";
    }

    public sealed class User
    {
        public const string Create = "user.create";
        public const string View = "user.view";
        public const string Edit = "user.edit";
        public const string Delete = "user.delete";
    }

    public sealed class Role
    {
        public const string Create = "role.create";
        public const string View = "role.view";
        public const string Delete = "role.delete";
        public const string Update = "role.update";
    }

    public sealed class Event
    {
        public const string Create = "event.create";
        public const string View = "event.view";
        public const string Delete = "event.delete";
        public const string Edit = "event.edit";
        public const string ToggleEventStatus = "event.status.toggle";
    }

    public sealed class RegistrationCounter
    {
        public const string Create = "registrationCounter.create";
        public const string View = "registrationCounter.view";
        public const string Delete = "registrationCounter.delete";
        public const string Edit = "registrationCounter.edit";
        public const string Unlock = "registrationCounter.unlock";
    }

    public sealed class VisitorRegistration
    {
        public const string OnsiteRegister = "visitor.registration.on.site";
        public const string OnsiteUpdate = "visitor.update.on.site";
        public const string Register = "visitorRegistration.register";
        public const string GenerateVisitorPass = "visitorRegistration.generate.visitor-pass";
        public const string ManageVisitor = "visitorRegistration.manage.visitor";
        public const string CancelVisitorRegistration = "visitorRegistration.cancel.visitor-registration";
        public const string CheckPassValidity = "visitorRegistration.check.pass-validity";
        public const string View = "visitor.view";
    }

    public sealed class Visitor
    {
        public const string BlackList = "visitor.blacklisted";
    }

    public sealed class TeamMemberRegistration
    {
        public const string Register = "teamMemberRegistration.register";
        public const string UploadAttachments = "teamMemberRegistration.upload-attachments";
        public const string ViewAttachments = "teamMemberRegistration.view-attachments";
        public const string View = "teamMemberRegistration.view";
        public const string GenerateTeamMemberPass = "teamMemberRegistration.generate.team-member-pass";
        public const string CancelTeamMemberRegistration = "teamMemberRegistration.cancel";
        public const string UpdateTeamMember = "teamMemberRegistration.update";
    }

    public sealed class Report
    {
        public const string GenerateVisitorListReport = "report.generate.visitor-list";
        public const string GenerateCollectionReport = "report.generate.collection";
    }

    public sealed class PassCategory
    {
        public const string CreatePassCategory = "pass.category.create";
        public const string ViewPassCategory = "pass.category.view";
        public const string EditPassCategory = "pass.category.edit";
        public const string ViewPassRate = "pass.category.rate.view";
        public const string EditPassRate = "pass.category.rate.edit";
        public const string DeletePassCategory = "pass.category.delete";
        public const string ManagePassTemplate = "pass.template.manage";
    }

    public sealed class PassGeneration
    {
        public const string PassGenerationView = "passGeneration.view";
        public const string VisitorPassGeneration = "passGeneration.visitor.generate";
        public const string TeamMemberPassGeneration = "passGeneration.teammember.generate";
        public const string PassVerification = "passGeneration.passVerification";
    }

    public sealed class Pavilion
    {
        public const string Create = "pavilion.create";
        public const string View = "pavilion.view";
        public const string Delete = "pavilion.delete";
        public const string Edit = "pavilion.edit";
        public const string CreatePavilionSession = "pavilion.session.create";
        public const string ViewPavilionSession = "pavilion.session.view";
        public const string EditPavilionSession = "pavilion.session.edit";
        public const string TogglePavilionStatus = "pavilion.status.toggle";
        public const string SessionDelete = "pavilion.session.delete";
        public const string RateView = "pavilion.rate.view";
        public const string EditPassPavilionRate = "pavilion.rate.edit";
    }

    public sealed class PavilionVisitor
    {
        public const string View = "pavilionVisitor.view";
    }

    public sealed class PaymentSetting
    {
        public const string PaymentSettingsUpdate = "paymentSettings.update";
    }
}
