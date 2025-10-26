namespace Client.API.Authorization
{
    public static class ScreenCodes
    {
        public const string INVOICE = "INVOICE";
        public const string PRODUCT = "PRODUCT";
        public const string SUBCONTRACTOR = "SUBCONTRACTOR";
        public const string PAYMENT = "PAYMENT";
        public const string ADDITIONAL_ENTITY = "ADDITIONALENTITY";
        public const string PAID_REPORT = "PAIDREPORT";
        public const string UNPAID_REPORT = "UNPAIDREPORT";
        public const string PRODUCT_WISE_REPORT = "PRODUCTWISEREPORT";
        public const string SUBCONTRACTOR_WISE_REPORT = "SUBCONTRACTORWISEREPORT";
        public const string COMBINED_REPORT = "COMBINEDREPORT";
        public const string COMPANY = "COMPANY";
        public const string USER = "USER";
        public const string ROLE = "ROLE";
        public const string BANK = "BANK";
    }

    public static class Permissions
    {
        public const string VIEW = "View";
        public const string CREATE = "Create";
        public const string EDIT = "Edit";
        public const string DELETE = "Delete";
    }
}
