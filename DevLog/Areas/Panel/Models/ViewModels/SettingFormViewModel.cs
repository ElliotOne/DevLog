using DevLog.Core.Domain;
using DevLog.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace DevLog.Areas.Panel.Models.ViewModels
{
    public class SettingFormViewModel : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Phone 1")]
        [MaxLength(20, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string? Phone1 { get; set; }

        [Display(Name = "Phone 2")]
        [MaxLength(20, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string? Phone2 { get; set; }

        [Display(Name = "Email 1")]
        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string? Email1 { get; set; }

        [Display(Name = "Email 2")]
        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string? Email2 { get; set; }

        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string? Instagram { get; set; }

        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string? Telegram { get; set; }

        [Display(Name = "Google+")]
        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string? GooglePlus { get; set; }

        [Display(Name = "Facebook")]
        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string? FaceBook { get; set; }

        [Display(Name = "LinkedIn")]
        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string? LinkedIn { get; set; }

        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string? YouTube { get; set; }

        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string? GitHub { get; set; }

        [Display(Name = "Wallet 1 Name")]
        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string? WalletName1 { get; set; }

        [Display(Name = "Wallet 1 Address")]
        [MaxLength(1000, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string? WalletAddress1 { get; set; }

        [Display(Name = "Wallet 2 Name")]
        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string? WalletName2 { get; set; }

        [Display(Name = "Wallet 2 Address")]
        [MaxLength(1000, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string? WalletAddress2 { get; set; }

        [Display(Name = "Wallet 3 Name")]
        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string? WalletName3 { get; set; }

        [Display(Name = "Wallet 3 Address")]
        [MaxLength(1000, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string? WalletAddress3 { get; set; }

        [Display(Name = "Wallet 4 Name")]
        [MaxLength(256, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string? WalletName4 { get; set; }

        [Display(Name = "Wallet 4 Address")]
        [MaxLength(1000, ErrorMessage = ValidationErrorMessagesConstant.MaxLengthMsg)]
        public string? WalletAddress4 { get; set; }
    }
}
