using ExchangeSimulatorBackend.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ExchangeSimulatorBackend.Dtos
{
    public class LoginUserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginUserDtoValidator : AbstractValidator<LoginUserDto>
    {
        public LoginUserDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty();

         
        }
    }
}
