namespace Clc.Polaris.Api.Models
{
    public class ItemCheckOutResult : PapiResponseCommon
    {
        private const int KnownPatronBlockFlags =
            (int)(
                CheckoutPatronBlockReasons.TooManyOverdueItems |
                CheckoutPatronBlockReasons.TooManyLongOverdueItems |
                CheckoutPatronBlockReasons.TooManyOutstandingClaims |
                CheckoutPatronBlockReasons.TooManyTotalClaims |
                CheckoutPatronBlockReasons.OwesMoreThanSecondLevelFineAmount |
                CheckoutPatronBlockReasons.CollectionAgencyBlock |
                CheckoutPatronBlockReasons.AddressCheckBlock |
                CheckoutPatronBlockReasons.VerifyBorrowerBlock |
                CheckoutPatronBlockReasons.TooManyLostItems |
                CheckoutPatronBlockReasons.PatronCodeBlocked |
                CheckoutPatronBlockReasons.LibraryAssignedBlock |
                CheckoutPatronBlockReasons.FreeTextBlock |
                CheckoutPatronBlockReasons.ExceededMoneyOwedEstimatedFinesOnly |
                CheckoutPatronBlockReasons.ExceededMoneyOwedCurrentAndEstimatedFines |
                CheckoutPatronBlockReasons.CheckoutLimitedToTransactingBranchPatrons |
                CheckoutPatronBlockReasons.PatronAccountSecured);

        private const int KnownItemBlockFlags =
            (int)(
                CheckoutItemBlockReasons.PatronReachedTotalItemsOutForPatronType |
                CheckoutItemBlockReasons.PatronReachedTotalItemsPermittedForMaterialType |
                CheckoutItemBlockReasons.PatronReachedCourseReserveItemLimitForPatronType |
                CheckoutItemBlockReasons.StatusBindingBlocked |
                CheckoutItemBlockReasons.StatusInProgressBlocked |
                CheckoutItemBlockReasons.StatusInRepairBlocked |
                CheckoutItemBlockReasons.StatusLostBlocked |
                CheckoutItemBlockReasons.StatusMissingBlocked |
                CheckoutItemBlockReasons.StatusOnOrderBlocked |
                CheckoutItemBlockReasons.StatusInTransitBlocked |
                CheckoutItemBlockReasons.StatusUnavailableBlocked |
                CheckoutItemBlockReasons.StatusWithdrawnBlocked |
                CheckoutItemBlockReasons.StatusRoutedBlocked |
                CheckoutItemBlockReasons.ItemFreeTextBlock |
                CheckoutItemBlockReasons.ItemLibraryAssignedBlock |
                CheckoutItemBlockReasons.ItemFromAnotherBranch |
                CheckoutItemBlockReasons.StatusClaimMissingPartsBlocked |
                CheckoutItemBlockReasons.ItemMaterialTypeBlocked |
                CheckoutItemBlockReasons.ItemHeldForAnotherPatron |
                CheckoutItemBlockReasons.ItemFillsRequestForAnotherPatron |
                CheckoutItemBlockReasons.ItemAlreadyCheckedOutToAnotherPatron |
                CheckoutItemBlockReasons.ChargeForCheckout |
                CheckoutItemBlockReasons.IntegratedElectronicItem |
                CheckoutItemBlockReasons.PatronDoesNotMeetMinimumAgeRequirement |
                CheckoutItemBlockReasons.StatusDamagedBlocked);

        private const int KnownRenewalBlockFlags =
            (int)(
                CheckoutRenewalBlockReasons.PatronCodeBlock |
                CheckoutRenewalBlockReasons.LibraryAssignedBlock |
                CheckoutRenewalBlockReasons.FreeTextBlock |
                CheckoutRenewalBlockReasons.OwesMoreThanSecondLevelFineAmount |
                CheckoutRenewalBlockReasons.CollectionAgencyBlock |
                CheckoutRenewalBlockReasons.VerifyBorrowerBlock |
                CheckoutRenewalBlockReasons.TooManyLostItems |
                CheckoutRenewalBlockReasons.TooManyOutstandingClaims |
                CheckoutRenewalBlockReasons.TooManyTotalClaims |
                CheckoutRenewalBlockReasons.RegistrationExpiresBeforeDueDate |
                CheckoutRenewalBlockReasons.ItemLibraryAssignedBlock |
                CheckoutRenewalBlockReasons.ItemFreeTextBlock |
                CheckoutRenewalBlockReasons.ItemAlreadyOverdue |
                CheckoutRenewalBlockReasons.ItemAlreadyLongOverdue |
                CheckoutRenewalBlockReasons.ItemRenewalLimitReached |
                CheckoutRenewalBlockReasons.ItemFillsRequestForAnotherPatron |
                CheckoutRenewalBlockReasons.TooManyOverdueItems |
                CheckoutRenewalBlockReasons.TooManyLongOverdueItems |
                CheckoutRenewalBlockReasons.ItemNotCheckedOutToPatron |
                CheckoutRenewalBlockReasons.AutomaticRenewalOnly |
                CheckoutRenewalBlockReasons.PatronAccountSecured);

        private static readonly IReadOnlyList<BlockReasonDescription> PatronBlockReasonDescriptionsByFlag =
        [
            new((int)CheckoutPatronBlockReasons.TooManyOverdueItems, "Patron has too many overdue items."),
            new((int)CheckoutPatronBlockReasons.TooManyLongOverdueItems, "Patron has too many long overdue items."),
            new((int)CheckoutPatronBlockReasons.TooManyOutstandingClaims, "Patron has too many outstanding claims."),
            new((int)CheckoutPatronBlockReasons.TooManyTotalClaims, "Patron has too many total claims."),
            new((int)CheckoutPatronBlockReasons.OwesMoreThanSecondLevelFineAmount, "Patron owes more than second-level fine amount."),
            new((int)CheckoutPatronBlockReasons.CollectionAgencyBlock, "Patron has a collection agency block."),
            new((int)CheckoutPatronBlockReasons.AddressCheckBlock, "Patron has address check block."),
            new((int)CheckoutPatronBlockReasons.VerifyBorrowerBlock, "Patron has verify borrower block."),
            new((int)CheckoutPatronBlockReasons.TooManyLostItems, "Patron has too many lost items."),
            new((int)CheckoutPatronBlockReasons.PatronCodeBlocked, "Patron code is blocked."),
            new((int)CheckoutPatronBlockReasons.LibraryAssignedBlock, "Patron has a library assigned block."),
            new((int)CheckoutPatronBlockReasons.FreeTextBlock, "Patron has a free text block."),
            new((int)CheckoutPatronBlockReasons.ExceededMoneyOwedEstimatedFinesOnly, "Patron has exceeded the money owed block for estimated fines only."),
            new((int)CheckoutPatronBlockReasons.ExceededMoneyOwedCurrentAndEstimatedFines, "Patron has exceeded the money owed block for current fines plus estimated fines."),
            new((int)CheckoutPatronBlockReasons.CheckoutLimitedToTransactingBranchPatrons, "Check out is limited to patrons assigned to the transacting branch."),
            new((int)CheckoutPatronBlockReasons.PatronAccountSecured, "Patron account is secured."),
        ];

        private static readonly IReadOnlyList<BlockReasonDescription> ItemBlockReasonDescriptionsByFlag =
        [
            new((int)CheckoutItemBlockReasons.PatronReachedTotalItemsOutForPatronType, "Patron has reached the total number of items out for patron type."),
            new((int)CheckoutItemBlockReasons.PatronReachedTotalItemsPermittedForMaterialType, "Patron has reached the total number of items permitted for material type."),
            new((int)CheckoutItemBlockReasons.PatronReachedCourseReserveItemLimitForPatronType, "Patron has reached the total number of course reserve items for patron type."),
            new((int)CheckoutItemBlockReasons.StatusBindingBlocked, "Item status 'Binding' is blocked."),
            new((int)CheckoutItemBlockReasons.StatusInProgressBlocked, "Item status 'In-Progress' is blocked."),
            new((int)CheckoutItemBlockReasons.StatusInRepairBlocked, "Item status 'In-Repair' is blocked."),
            new((int)CheckoutItemBlockReasons.StatusLostBlocked, "Item status 'Lost' is blocked."),
            new((int)CheckoutItemBlockReasons.StatusMissingBlocked, "Item status 'Missing' is blocked."),
            new((int)CheckoutItemBlockReasons.StatusOnOrderBlocked, "Item status 'On-Order' is blocked."),
            new((int)CheckoutItemBlockReasons.StatusInTransitBlocked, "Item status 'In-Transit' is blocked."),
            new((int)CheckoutItemBlockReasons.StatusUnavailableBlocked, "Item status 'Unavailable' is blocked."),
            new((int)CheckoutItemBlockReasons.StatusWithdrawnBlocked, "Item status 'Withdrawn' is blocked."),
            new((int)CheckoutItemBlockReasons.StatusRoutedBlocked, "Item status 'Routed' is blocked."),
            new((int)CheckoutItemBlockReasons.ItemFreeTextBlock, "Item has a free text block."),
            new((int)CheckoutItemBlockReasons.ItemLibraryAssignedBlock, "Item has a library-assigned block."),
            new((int)CheckoutItemBlockReasons.ItemFromAnotherBranch, "Item is from another branch."),
            new((int)CheckoutItemBlockReasons.StatusClaimMissingPartsBlocked, "Item status 'Claim Missing Parts' is blocked."),
            new((int)CheckoutItemBlockReasons.ItemMaterialTypeBlocked, "Item material type is blocked."),
            new((int)CheckoutItemBlockReasons.ItemHeldForAnotherPatron, "Item is held for another patron."),
            new((int)CheckoutItemBlockReasons.ItemFillsRequestForAnotherPatron, "Item fills a request for another patron."),
            new((int)CheckoutItemBlockReasons.ItemAlreadyCheckedOutToAnotherPatron, "Item is already checked out to another patron."),
            new((int)CheckoutItemBlockReasons.ChargeForCheckout, "Item is blocked because there is a charge for checkout."),
            new((int)CheckoutItemBlockReasons.IntegratedElectronicItem, "Integrated electronic item."),
            new((int)CheckoutItemBlockReasons.PatronDoesNotMeetMinimumAgeRequirement, "Patron does not meet the minimum age requirement for the item."),
            new((int)CheckoutItemBlockReasons.StatusDamagedBlocked, "Item status 'Damaged' is blocked."),
        ];

        private static readonly IReadOnlyList<BlockReasonDescription> RenewalBlockReasonDescriptionsByFlag =
        [
            new((int)CheckoutRenewalBlockReasons.PatronCodeBlock, "Patron, or associated patron, has a patron code block."),
            new((int)CheckoutRenewalBlockReasons.LibraryAssignedBlock, "Patron, or associated patron, has a library-assigned block."),
            new((int)CheckoutRenewalBlockReasons.FreeTextBlock, "Patron, or associated patron, has a free text block."),
            new((int)CheckoutRenewalBlockReasons.OwesMoreThanSecondLevelFineAmount, "Patron, or associated patron, owes more than second level fine amount."),
            new((int)CheckoutRenewalBlockReasons.CollectionAgencyBlock, "Patron, or associated patron, has a collection agency block."),
            new((int)CheckoutRenewalBlockReasons.VerifyBorrowerBlock, "Patron, or associated patron, has verify borrower block."),
            new((int)CheckoutRenewalBlockReasons.TooManyLostItems, "Patron, or associated patron, has too many lost items."),
            new((int)CheckoutRenewalBlockReasons.TooManyOutstandingClaims, "Patron, or associated patron, has too many outstanding claims."),
            new((int)CheckoutRenewalBlockReasons.TooManyTotalClaims, "Patron, or associated patron, has too many total claims."),
            new((int)CheckoutRenewalBlockReasons.RegistrationExpiresBeforeDueDate, "Patron registration will expire before item due date."),
            new((int)CheckoutRenewalBlockReasons.ItemLibraryAssignedBlock, "Item has a library-assigned block."),
            new((int)CheckoutRenewalBlockReasons.ItemFreeTextBlock, "Item has a free text block."),
            new((int)CheckoutRenewalBlockReasons.ItemAlreadyOverdue, "Item is already overdue."),
            new((int)CheckoutRenewalBlockReasons.ItemAlreadyLongOverdue, "Item is already long overdue."),
            new((int)CheckoutRenewalBlockReasons.ItemRenewalLimitReached, "Item renewal limit reached."),
            new((int)CheckoutRenewalBlockReasons.ItemFillsRequestForAnotherPatron, "Item fills a request for another patron."),
            new((int)CheckoutRenewalBlockReasons.TooManyOverdueItems, "Patron, or associated patron, has too many overdue items."),
            new((int)CheckoutRenewalBlockReasons.TooManyLongOverdueItems, "Patron, or associated patron, has too many long overdue items."),
            new((int)CheckoutRenewalBlockReasons.ItemNotCheckedOutToPatron, "Item is not checked out to the patron."),
            new((int)CheckoutRenewalBlockReasons.AutomaticRenewalOnly, "Automatic renewal only."),
            new((int)CheckoutRenewalBlockReasons.PatronAccountSecured, "Patron account is secured."),
        ];

        public int ItemRecordID { get; set; }
        public bool IsRenewal { get; set; }
        public DateTime? DueDate { get; set; }
        public double ChargeAmount { get; set; }
        public int PatronBlockFlags { get; set; }
        public int ItemBlockFlags { get; set; }
        public int RenewalBlockFlags { get; set; }
        public int MaterialTypeID { get; set; }
        public int SelfCheckMediaTypeID { get; set; }
        public bool IsMagnetic { get; set; }
        public bool CanDesensitize { get; set; }
        public bool DoubleSided { get; set; }
        public bool Unlocker { get; set; }
        public int DDM_MediaFormatID { get; set; }
        public string? Title { get; set; }

        public override string ToString() => $"{ItemRecordID} - {Title ?? string.Empty} - {DueDate:O} - {MaterialTypeID} - {ItemBlockFlags} - {RenewalBlockFlags}";

        public CheckoutPatronBlockReasons PatronBlockReasons => (CheckoutPatronBlockReasons)PatronBlockFlags;
        public CheckoutItemBlockReasons ItemBlockReasons => (CheckoutItemBlockReasons)ItemBlockFlags;
        public CheckoutRenewalBlockReasons RenewalBlockReasons => (CheckoutRenewalBlockReasons)RenewalBlockFlags;

        public IReadOnlyList<string> PatronBlockReasonDescriptions => DecodeBlockReasonDescriptions(PatronBlockFlags, PatronBlockReasonDescriptionsByFlag);
        public IReadOnlyList<string> ItemBlockReasonDescriptions => DecodeBlockReasonDescriptions(ItemBlockFlags, ItemBlockReasonDescriptionsByFlag);
        public IReadOnlyList<string> RenewalBlockReasonDescriptions => DecodeBlockReasonDescriptions(RenewalBlockFlags, RenewalBlockReasonDescriptionsByFlag);

        public int UnknownPatronBlockFlags => PatronBlockFlags & ~KnownPatronBlockFlags;
        public int UnknownItemBlockFlags => ItemBlockFlags & ~KnownItemBlockFlags;
        public int UnknownRenewalBlockFlags => RenewalBlockFlags & ~KnownRenewalBlockFlags;

        private static IReadOnlyList<string> DecodeBlockReasonDescriptions(int flags, IReadOnlyList<BlockReasonDescription> descriptions)
        {
            if (flags == 0)
            {
                return Array.Empty<string>();
            }

            var matchingDescriptions = new List<string>();

            foreach (var description in descriptions)
            {
                if ((flags & description.Flag) == description.Flag)
                {
                    matchingDescriptions.Add(description.Description);
                }
            }

            return matchingDescriptions;
        }

        private sealed record BlockReasonDescription(int Flag, string Description);
    }

    [Flags]
    public enum CheckoutPatronBlockReasons
    {
        None = 0,
        TooManyOverdueItems = 1,
        TooManyLongOverdueItems = 2,
        TooManyOutstandingClaims = 4,
        TooManyTotalClaims = 8,
        OwesMoreThanSecondLevelFineAmount = 16,
        CollectionAgencyBlock = 32,
        AddressCheckBlock = 64,
        VerifyBorrowerBlock = 128,
        TooManyLostItems = 256,
        PatronCodeBlocked = 512,
        LibraryAssignedBlock = 1024,
        FreeTextBlock = 2048,
        ExceededMoneyOwedEstimatedFinesOnly = 4096,
        ExceededMoneyOwedCurrentAndEstimatedFines = 8192,
        CheckoutLimitedToTransactingBranchPatrons = 16384,
        PatronAccountSecured = 32768,
    }

    [Flags]
    public enum CheckoutItemBlockReasons
    {
        None = 0,
        PatronReachedTotalItemsOutForPatronType = 1,
        PatronReachedTotalItemsPermittedForMaterialType = 2,
        PatronReachedCourseReserveItemLimitForPatronType = 4,
        StatusBindingBlocked = 8,
        StatusInProgressBlocked = 16,
        StatusInRepairBlocked = 32,
        StatusLostBlocked = 64,
        StatusMissingBlocked = 128,
        StatusOnOrderBlocked = 256,
        StatusInTransitBlocked = 512,
        StatusUnavailableBlocked = 1024,
        StatusWithdrawnBlocked = 2048,
        StatusRoutedBlocked = 4096,
        ItemFreeTextBlock = 8192,
        ItemLibraryAssignedBlock = 16384,
        ItemFromAnotherBranch = 32768,
        StatusClaimMissingPartsBlocked = 65536,
        ItemMaterialTypeBlocked = 262144,
        ItemHeldForAnotherPatron = 524288,
        ItemFillsRequestForAnotherPatron = 1048576,
        ItemAlreadyCheckedOutToAnotherPatron = 2097152,
        ChargeForCheckout = 4194304,
        IntegratedElectronicItem = 8388608,
        PatronDoesNotMeetMinimumAgeRequirement = 16777216,
        StatusDamagedBlocked = 33554432,
    }

    [Flags]
    public enum CheckoutRenewalBlockReasons
    {
        None = 0,
        PatronCodeBlock = 1,
        LibraryAssignedBlock = 2,
        FreeTextBlock = 4,
        OwesMoreThanSecondLevelFineAmount = 8,
        CollectionAgencyBlock = 16,
        VerifyBorrowerBlock = 32,
        TooManyLostItems = 64,
        TooManyOutstandingClaims = 128,
        TooManyTotalClaims = 256,
        RegistrationExpiresBeforeDueDate = 512,
        ItemLibraryAssignedBlock = 1024,
        ItemFreeTextBlock = 2048,
        ItemAlreadyOverdue = 4096,
        ItemAlreadyLongOverdue = 8192,
        ItemRenewalLimitReached = 16384,
        ItemFillsRequestForAnotherPatron = 32768,
        TooManyOverdueItems = 65536,
        TooManyLongOverdueItems = 131072,
        ItemNotCheckedOutToPatron = 262144,
        AutomaticRenewalOnly = 524288,
        PatronAccountSecured = 1048576,
    }
}