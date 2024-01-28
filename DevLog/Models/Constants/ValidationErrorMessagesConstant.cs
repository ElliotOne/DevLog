namespace DevLog.Models.Constants
{
    public static class ValidationErrorMessagesConstant
    {
        public const string RequiredMsg = "Please enter {0}.";
        public const string MaxLengthMsg = "{0} shouldn't be more than {1} characters.";
        public const string MinLengthMsg = "{0} shouldn't be less than {1} characters.";
        public const string RegexMsg = "The format of {0} is incorrect.";
        public const string RangeMsg = "The value of {0} should be between {1} and {2}.";
        public const string RemoteMsg = "This {0} is not valid. {0} Please try another value.";
        public const string CompareMsg = "Value of {0} and {1} are not the same.";
        public const string StringLengthMsg = "The length of {0} is incorrect.";
        public const string DuplicateMsg = "{0}  is duplicated. Please try another value.";
        public const string IsNotValid = "{0} is not valid. Please try another value.";
    }
}
