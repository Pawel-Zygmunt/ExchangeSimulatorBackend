using ExchangeSimulatorBackend.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace ExchangeSimulatorBackend.Dtos
{
    public class RegisterUserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator(UserManager<AppUser> userManager)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .MinimumLength(6);

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password)
                .WithMessage("Hasła muszą być takie same");

            RuleFor(x => x.Email)
                .Custom((value, context) =>
                {
                    var emailInUse = userManager.Users.FirstOrDefault((AppUser u) => u.Email == value);

                    if (emailInUse != null)
                    {
                        context.AddFailure("email", "Wybrany email jest zajęty");
                    }
                });
        }
    }
}
