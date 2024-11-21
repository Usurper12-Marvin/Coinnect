using System.ComponentModel.DataAnnotations;

namespace Coinnect.Models.ViewModels
{
    public class UserViewModels
    {
        public class LoginModel
        {
            [Required]
            public string Username { get; set; }

            [Required]
            [UIHint("password")]
            public string Password { get; set; }
            public string ReturnUrl { get; set; } = "/";
        }
        public class RegisterModel
        {

            [Required(ErrorMessage = "The User name field is required.")]
            [StringLength(250)]
            public string Username { get; set; }

            [Required(ErrorMessage = "The E-mail field is required.")]
            [DataType(DataType.EmailAddress)]
            [UserEmailValidation]
            public string Email { get; set; }

            [Required(ErrorMessage = "The Password field is required.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required(ErrorMessage = "Confirm your new password")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "The contact number is required.")]
            [StringLength(10)]
            public string ContactNumber { get; set; }

            [Required(ErrorMessage = "The first name is required.")]
            public string Firstname { get; set; }

            [Required(ErrorMessage = "The last name is required.")]
            public string Lastname { get; set; }

            [Required(ErrorMessage = "The student or staff number is required.")]
            [StringLength(13)]
            [UserIdentifierValidation]
            public string Identifier { get; set; }

            [Required]
            [UserTypeValidation]
            public string UserType { get; set; }

            [Required(ErrorMessage = "The date is invalid.")]
            public DateOnly DOB { get; set; }

            public string UserRole { get; set; }
        }

        public class AssignRole
        {
            public string UserId { get; set; }
            public string Email { get; set; }
            public IEnumerable<string> Roles { get; set; }
        }
        public class CreateAccount
        {
            [Required]
            public string AccountType { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Initial deposit must be a positive amount")]
            public decimal InitialDeposit { get; set; }
        }

        public class Transfer
        {
            public string TransectionType { get; set; }
            [Required]
            public string toAccount { get; set; }
            public string fromAccount { get; set; }
            [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive amount")]
            public decimal Amount { get; set; }

        }

        public class ChangePassword
        {
            [Required(ErrorMessage = "Current password is required")]
            [DataType(DataType.Password)]
            public string CurrentPassword { get; set; }

            [Required(ErrorMessage = "New password is required")]
            [DataType(DataType.Password)]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 5)]
            public string NewPassword { get; set; }

            [Required(ErrorMessage = "Confirm your new password")]
            [DataType(DataType.Password)]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }
        public class AdviceViewModel
        {
            public string ClientId { get; set; }
            [Required]
            public string AdviceText { get; set; }
        }
        public class RatingViewModel
        {
            public int Rating { get; set; }
            [Required]
            public string Review { get; set; }
        }
        public class ContactUsViewModel
        {
            public string ContactUsText { get; set; }
            public DateTime DateSent { get; set; }
            public string ClientName { get; set; }
            public string ClientEmail { get; set; }
        }

        public class UserIdentifierValidationAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (RegisterModel)validationContext.ObjectInstance;
                var identifier = value as string;

                if (model.UserType == "Staff" && identifier != null)
                {
                    if (identifier.Length != 7)
                    {
                        return new ValidationResult("The staff number must be exactly 7 digits.");
                    }
                }
                else if(model.UserType == "Student" && identifier != null)
                {
                    if (identifier.Length != 10)
                    {
                        return new ValidationResult("The student number must be exactly 10 digits.");
                    }
                }
                else if(model.UserType == "Visitor"&& identifier != null)
                {
                    if (identifier.Length != 13)
                    {
                        return new ValidationResult("The ID must be exactly 13 digits.");
                    }
                }

                return ValidationResult.Success;
            }
        }
        public class UserEmailValidationAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (RegisterModel)validationContext.ObjectInstance;
                var email = value as string;

                if (model.UserType == "Student")
                {
                    string resultEmail = model.Identifier + "@ufs4life.ac.za";
                    if (email != resultEmail)
                    {
                        return new ValidationResult("Students email must be student number and @ufs4life.ac.za .");
                    }
                }
                else if (model.UserType == "Staff" && !email.EndsWith("@ufs.ac.za"))
                {
                    return new ValidationResult("Staff must use a @ufs.ac.za email address.");
                }
                return ValidationResult.Success;
            }
        }
        public class UserTypeValidationAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var model = (RegisterModel)validationContext.ObjectInstance;
                var type = value as string;

                if (type != "Student" && type != "Staff" && type != "Visitor")
                {
                    return new ValidationResult("Select user type.");
                }
                else
                {
                    return ValidationResult.Success;
                }
                
            }
        }



    }
}
