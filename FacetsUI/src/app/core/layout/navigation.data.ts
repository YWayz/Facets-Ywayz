import { EventPermissions, PassGenerationPermissions, PavilionVisitorPermissions, ReportsPermissions, SuperAdminPermissions, TeamMemberRegistrationPermissions, UserPermissions, VisitorRegistrationPermissions } from "../extensions/permission-constants";

export function getMenuItems() {
    return [
        {
            link_name: "Event",
            link: "/admin/event",
            icon: "badge",
            permissions: [SuperAdminPermissions.all, EventPermissions.view],
            sub_menu: []
        },
        {
            link_name: "Visitors",
            link: null,
            icon: "portrait",
            permissions: [SuperAdminPermissions.all, VisitorRegistrationPermissions.onSiteRegister, VisitorRegistrationPermissions.onSiteUpdate, VisitorRegistrationPermissions.view],
            sub_menu: [
                {
                    link_name: "Registration",
                    link: "/admin/visitor/register",
                    permissions: [SuperAdminPermissions.all, VisitorRegistrationPermissions.onSiteRegister, VisitorRegistrationPermissions.onSiteUpdate]
                },
                {
                    link_name: "Pass Generation",
                    link: "/admin/visitor/pass-generation",
                    permissions: [SuperAdminPermissions.all, PassGenerationPermissions.view],
                },
                {
                    link_name: "Visitors Managment",
                    link: "/admin/visitor/visitor-management",
                    permissions: [SuperAdminPermissions.all, VisitorRegistrationPermissions.view],
                },
                {
                    link_name: "Pavilion Visitor",
                    link: "/admin/visitor/pavilion-visitor",
                    permissions: [SuperAdminPermissions.all, PavilionVisitorPermissions.view],
                }
            ]
        },
        {
            link_name: "Teams",
            link: "/admin/team-member",
            icon: "groups",
            permissions: [SuperAdminPermissions.all, TeamMemberRegistrationPermissions.view],
            sub_menu: []
        },
        {
            link_name: "Reports",
            link: null,
            icon: "analytics",
            permissions: [SuperAdminPermissions.all, ReportsPermissions.collectionReport, ReportsPermissions.visitorReport],
            sub_menu: [
                {
                    link_name: "Visitor Detail",
                    link: "/admin/report/visitor",
                    permissions: [SuperAdminPermissions.all, ReportsPermissions.visitorReport],
                }, {
                    link_name: "Visitor Collection",
                    link: "/admin/report/collection",
                    permissions: [SuperAdminPermissions.all, ReportsPermissions.collectionReport],
                },
                {
                    link_name: "Attendance",
                    link: "/admin/report/attendance",
                    permissions: [SuperAdminPermissions.all, ReportsPermissions.collectionReport],
                }
            ]
        },
        {
            link_name: "General Settings",
            link: null,
            icon: "settings",
            permissions: [SuperAdminPermissions.all, UserPermissions.view],
            sub_menu: [
                {
                    link_name: "User Management",
                    link: "/admin/user-management",
                    permissions: [SuperAdminPermissions.all, UserPermissions.view],
                }
            ]
        },
        {
            link_name: "Pass Verification",
            link: null,
            icon: "qr_code_2",
            permissions: [SuperAdminPermissions.all, PassGenerationPermissions.verification],
            sub_menu: [
                {
                    link_name: "Event Pass Verification",
                    link: "/admin/visitor/pass-verification/event",
                    permissions: [SuperAdminPermissions.all, PassGenerationPermissions.verification],
                },
                {
                    link_name: "Pavilion Pass Verification",
                    link: "/admin/visitor/pass-verification/pavilion",
                    permissions: [SuperAdminPermissions.all, PassGenerationPermissions.verification],
                }
            ]
        }
    ];
}
