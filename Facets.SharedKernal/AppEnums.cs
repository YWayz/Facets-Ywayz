namespace Facets.SharedKernal;

public static class AppEnums
{
    public enum Title
    {
        None = 0,
        Mr,
        Mrs,
        Ms,
        Miss
    }

    public enum EventStatus
    {
        None = 0,
        Inactive,
        Active
    }

    public enum VisitorIdentityType
    {
        None = 0,
        NIC,
        Passport
    }

    public enum AttachmentType
    {
        None = 0,
        NIC,
        OtherAttachment,
        ProfileImage
    }

    public enum PaymentMethod
    {
        None = 0,
        NoPaymentNeeded,
        Cash,
        Card
    }

    public enum PassCategoryType
    {
        None = 0,
        AssocifyMember,
        LocalVisitor,
        LocalBuyer,
        ForeignVisitor,
        ForeignBuyer,
        TeamMember,
        Custom_Visitor,
        LocalAndForeignVisitor,
    }

    public enum VisitorStatus
    {
        None = 0,
        Active,
        BlackListed
    }

    public enum PaymentStatus
    {
        None = 0,
        Free,
        Unpaid,
        Paid,
    }

    public enum DiscountType
    {
        None = 0,
        NoDiscount,
        EarlyRegistrationDiscountedRate,
        OnlineRegistrationDiscountedRate,
        EntireEventDiscountedRate
    }

    public enum TeamMemberStatus
    {
        None = 0,
        Active,
        Cancelled,
        BlackListed
    }

    public enum VisitorActivityType
    {
        None = 0,
        Registered,
        RegistrationCancelled,
        PassGenerated,
        MarkedAsBlacklisted,
        RemovedFromBlackList
    }

    public enum TeamMemberActivityType
    {
        None = 0,
        Registered,
        RegistrationCancelled,
        PassGenerated,
    }

    public enum PassType
    {
        None = 0,
        TeamMember,
        Visitor
    }

    public enum TemplateSizeType
    {
        None = 0,
        A6,
        A7
    }

    public enum OTPType
    {
        None = 0,
        NewVisitorOnlineRegistration,
        PublicSearchVisitorDetails
    }

    public enum RateType
    {
        None = 0,
        FlatRate,
        PerDayRate
    }

    public enum VisitorPassCategoryType
    {
        None = 0,
        PerDayPass,
        CommonPass
    }

    public enum PavilionStatus
    {
        None = 0,
        Inactive,
        Active
    }

    public enum InvoiceLineItemType
    {

        None = 0,
        VisitorEventAttendance,
        VisitorPavilionSessionAttendance
    }

    public enum OnSitePayingMode
    {
        None = 0,
        PayAtRegistration,
        PayAtPassGeneration
    }

    public enum CounterType
    {
        None = 0,
        RegistrationOnly,
        PaymentOnly,
        RegistrationAndPayment
    }

    public enum MemberBlacklistStatus
    {
        None = 0,
        Active,
        BlackListed
    }
}
