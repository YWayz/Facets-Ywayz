using Facets.Core.Security.Claims;

namespace Facets.Core.Security.ModulePermissions;

public sealed record PermissionSet(string DisplayName, string Key);

public sealed class AppModulePermissions
{
    public static IReadOnlyList<KeyValuePair<string, IReadOnlyList<PermissionSet>>> GetPermissionList()
    {
        return new List<KeyValuePair<string, IReadOnlyList<PermissionSet>>>
        {
            _eventPermissions,
            _userPermissions,
            _rolePermissions,
            _passCategoryPermissions,
            _registrationCounterPermissions,
            _visitorPermissions,
            _teamMemberPermissions,
            _reportPermissions,
            _passGenerationPermissions,
            _pavilionPermissions,
            _pavilionVisitorPermissions,
            _paymentSettingsPermissions
        }.ToList();
    }

    public static IReadOnlyList<string> GetPermissionKeys()
    {
        var permissionList = GetPermissionList();
        var keys = permissionList.SelectMany(s => s.Value).Select(s => s.Key).ToList();

        return keys;
    }

    private static readonly KeyValuePair<string, IReadOnlyList<PermissionSet>> _userPermissions =
        new(key: "User", value: new[]
        {
            new PermissionSet( "View", ApplicationClaimValues.User.View),
            new PermissionSet( "Create", ApplicationClaimValues.User.Create),
            new PermissionSet( "Edit", ApplicationClaimValues.User.Edit),
            new PermissionSet( "Delete", ApplicationClaimValues.User.Delete),
        });

    private static readonly KeyValuePair<string, IReadOnlyList<PermissionSet>> _rolePermissions =
        new(key: "Role", value: new[]
        {
            new PermissionSet( "View", ApplicationClaimValues.Role.View),
            new PermissionSet( "Create", ApplicationClaimValues.Role.Create),
            new PermissionSet( "Update", ApplicationClaimValues.Role.Update),
            new PermissionSet( "Delete", ApplicationClaimValues.Role.Delete)
        });

    private static readonly KeyValuePair<string, IReadOnlyList<PermissionSet>> _eventPermissions =
        new(key: "Event", value: new[]
        {
            new PermissionSet( "View", ApplicationClaimValues.Event.View),
            new PermissionSet( "Create", ApplicationClaimValues.Event.Create),
            new PermissionSet( "Update", ApplicationClaimValues.Event.Edit),
            new PermissionSet( "Delete", ApplicationClaimValues.Event.Delete),
            new PermissionSet( "Change Status", ApplicationClaimValues.Event.ToggleEventStatus)
        });

    private static readonly KeyValuePair<string, IReadOnlyList<PermissionSet>> _passCategoryPermissions =
        new(key: "Pass Category & Template", value: new[]
        {
            new PermissionSet( "View Pass Category", ApplicationClaimValues.PassCategory.ViewPassCategory),
            new PermissionSet( "Create Pass Category", ApplicationClaimValues.PassCategory.CreatePassCategory),
            new PermissionSet( "Update Pass Category", ApplicationClaimValues.PassCategory.EditPassCategory),
            new PermissionSet( "Delete Category", ApplicationClaimValues.PassCategory.DeletePassCategory),
            new PermissionSet( "View Pass Rate", ApplicationClaimValues.PassCategory.ViewPassRate),
            new PermissionSet( "Edit Pass Rate", ApplicationClaimValues.PassCategory.EditPassRate),
            new PermissionSet( "Manage Pass Template", ApplicationClaimValues.PassCategory.ManagePassTemplate),
        });

    private static readonly KeyValuePair<string, IReadOnlyList<PermissionSet>> _registrationCounterPermissions =
        new(key: "Registration Counter", value: new[]
        {
                new PermissionSet( "View", ApplicationClaimValues.RegistrationCounter.View),
                new PermissionSet( "Create", ApplicationClaimValues.RegistrationCounter.Create),
                new PermissionSet( "Update", ApplicationClaimValues.RegistrationCounter.Edit),
                new PermissionSet( "Delete", ApplicationClaimValues.RegistrationCounter.Delete),
                new PermissionSet( "Unlock", ApplicationClaimValues.RegistrationCounter.Unlock)
        });

    private static readonly KeyValuePair<string, IReadOnlyList<PermissionSet>> _visitorPermissions =
        new(key: "Visitors", value: new[]
        {
                new PermissionSet( "Onsite Register", ApplicationClaimValues.VisitorRegistration.OnsiteRegister),
                new PermissionSet( "Onsite Update", ApplicationClaimValues.VisitorRegistration.OnsiteUpdate),
                new PermissionSet( "Add / Remove from Blacklist", ApplicationClaimValues.Visitor.BlackList),
                new PermissionSet( "View", ApplicationClaimValues.VisitorRegistration.View),
                new PermissionSet( "Cancel Registration", ApplicationClaimValues.VisitorRegistration.CancelVisitorRegistration),
        });

    private static readonly KeyValuePair<string, IReadOnlyList<PermissionSet>> _teamMemberPermissions =
        new(key: "Team Members", value: new[]
        {
                new PermissionSet( "Register", ApplicationClaimValues.TeamMemberRegistration.Register),
                new PermissionSet( "Upload Attachments", ApplicationClaimValues.TeamMemberRegistration.UploadAttachments),
                new PermissionSet( "View Attachments", ApplicationClaimValues.TeamMemberRegistration.ViewAttachments),
                new PermissionSet( "View", ApplicationClaimValues.TeamMemberRegistration.View),
                new PermissionSet( "Generate Pass", ApplicationClaimValues.TeamMemberRegistration.GenerateTeamMemberPass),
                new PermissionSet( "Cancel Registration", ApplicationClaimValues.TeamMemberRegistration.CancelTeamMemberRegistration),
                new PermissionSet( "Update", ApplicationClaimValues.TeamMemberRegistration.UpdateTeamMember),
        });

    private static readonly KeyValuePair<string, IReadOnlyList<PermissionSet>> _reportPermissions =
      new(key: "Report", value: new[]
      {
            new PermissionSet( "Collection Report", ApplicationClaimValues.Report.GenerateCollectionReport),
            new PermissionSet( "Visitor Report", ApplicationClaimValues.Report.GenerateVisitorListReport),

      });

    private static readonly KeyValuePair<string, IReadOnlyList<PermissionSet>> _passGenerationPermissions =
      new(key: "Pass Generation", value: new[]
      {
            new PermissionSet( "View", ApplicationClaimValues.PassGeneration.PassGenerationView),
            new PermissionSet( "Visitor Pass Generation", ApplicationClaimValues.PassGeneration.VisitorPassGeneration),
            new PermissionSet( "Team Member Pass Generation", ApplicationClaimValues.PassGeneration.TeamMemberPassGeneration),
            new PermissionSet( "Pass Verification", ApplicationClaimValues.PassGeneration.PassVerification),
      });

    private static readonly KeyValuePair<string, IReadOnlyList<PermissionSet>> _pavilionPermissions =
    new(key: "Pavilion", value: new[]
    {
            new PermissionSet( "View", ApplicationClaimValues.Pavilion.View),
            new PermissionSet( "Create", ApplicationClaimValues.Pavilion.Create),
            new PermissionSet( "Update", ApplicationClaimValues.Pavilion.Edit),
            new PermissionSet( "Delete", ApplicationClaimValues.Pavilion.Delete),
            new PermissionSet( "Change Status", ApplicationClaimValues.Pavilion.TogglePavilionStatus),
            new PermissionSet( "View Pavilion Session", ApplicationClaimValues.Pavilion.ViewPavilionSession),
            new PermissionSet( "Create Pavilion Session", ApplicationClaimValues.Pavilion.CreatePavilionSession),
            new PermissionSet( "Update Pavilion Session", ApplicationClaimValues.Pavilion.EditPavilionSession),
            new PermissionSet( "Delete Pavilion Session", ApplicationClaimValues.Pavilion.SessionDelete),
            new PermissionSet( "View Pavilion Pass Rate", ApplicationClaimValues.Pavilion.RateView),
            new PermissionSet( "Edit Pavilion Pass Rate", ApplicationClaimValues.Pavilion.EditPassPavilionRate),
    });
    
    private static readonly KeyValuePair<string, IReadOnlyList<PermissionSet>> _pavilionVisitorPermissions =
    new(key: "Pavilion Visitor", value: new[]
    {
            new PermissionSet( "View", ApplicationClaimValues.PavilionVisitor.View),
    });
    
    private static readonly KeyValuePair<string, IReadOnlyList<PermissionSet>> _paymentSettingsPermissions =
    new(key: "Payment Settings", value: new[]
    {
            new PermissionSet("Update", ApplicationClaimValues.PaymentSetting.PaymentSettingsUpdate),
    });
}



