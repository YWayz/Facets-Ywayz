export const SuperAdminPermissions = {
    all: "all"
}

export const UserPermissions = {
    create: "user.create",
    view: "user.view",
    edit: "user.edit",
    delete: "user.delete"
}

export const RolePermissions = {
    create: "role.create",
    view: "role.view",
    updateRoleClaim: "role.roleclaim.update",
    delete: "role.delete",
    update: "role.update"
}

export const EventPermissions = {
    create: "event.create",
    view: "event.view",
    delete: "event.delete",
    edit: "event.edit",
    toggleEventStatus: "event.status.toggle"
}

export const RegistrationCounterPermissions = {
    create: "registrationCounter.create",
    view: "registrationCounter.view",
    delete: "registrationCounter.delete",
    edit: "registrationCounter.edit",
    toggleUnlock: "registrationCounter.unlock",
}

export const VisitorRegistrationPermissions = {
    onSiteRegister: "visitor.registration.on.site",
    onSiteUpdate: "visitor.update.on.site",
    register: "visitorRegistration.register",
    generateVisitorPass: "visitorRegistration.generate.visitor-pass",
    manageVisitor: "visitorRegistration.manage.visitor",
    cancelVisitorRegistration: "visitorRegistration.cancel.visitor-registration",
    checkPassValidity: "visitorRegistration.check.pass-validity",
    view: "visitor.view",
    cancelRegistration: "visitorRegistration.cancel.visitor-registration"
}

export const VisitorPermissions = {
    blackList: "visitor.blacklisted"
}

export const TeamMemberRegistrationPermissions = {
    register: "teamMemberRegistration.register",
    update: "teamMemberRegistration.update",
    uploadAttachments: "teamMemberRegistration.upload-attachments",
    viewAttachments: "teamMemberRegistration.view-attachments",
    view: "teamMemberRegistration.view",
    generateTeamMemberPass: "teamMemberRegistration.generate.team-member-pass",
    cancelTeamMemberRegistration: "teamMemberRegistration.cancel",
    updateTeamMember: "teamMemberRegistration.update",
}

export const PassCategoryPermissions = {
    viewPassRate: "pass.category.rate.view",
    editPassRate: "pass.category.rate.edit",
    create: "pass.category.create",
    view: "pass.category.view",
    edit: "pass.category.edit",
    delete: "pass.category.delete",
    managePassTemplate: "pass.template.manage",
}

export const ReportsPermissions = {
    visitorReport: "report.generate.visitor-list",
    collectionReport: "report.generate.collection"
}

export const PassGenerationPermissions = {
    view: "passGeneration.view",
    visitorPassGeneration: "passGeneration.visitor.generate",
    teamMemberPassGeneration: "passGeneration.teammember.generate",
    verification : "passGeneration.passVerification"
}

export const PavilionPermissions = {
    create: "pavilion.create",
    view: "pavilion.view",
    delete: "pavilion.delete",
    edit: "pavilion.edit",
    createPavilionSession: "pavilion.session.create",
    viewPavilionSession: "pavilion.session.view",
    editPavilionSession: "pavilion.session.edit",
    deletePavilionSession: "pavilion.session.delete",
    togglePavilionStatus: "pavilion.status.toggle",
    rateView: "pavilion.rate.view",
    editPassPavilionRate: "pavilion.rate.edit",
}

export const PavilionVisitorPermissions = {
    view: "pavilionVisitor.view",
}

export const PaymentSettingsPermissions = {
    paymentSettingsUpdate: "paymentSettings.update"
}