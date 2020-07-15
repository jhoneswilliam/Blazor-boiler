using FluentValidation;

namespace blazor.Models.Security.Validators
{
    public class CreateLoginRequestValidator : AbstractValidator<CreateLoginRequest>
    {
        public CreateLoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email must be not empty");
         
            RuleFor(x => x.Password)
               .NotEmpty()
               .WithMessage("Password must be not empty");
        }
    }
}
