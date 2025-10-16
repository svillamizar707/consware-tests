using FluentValidation;
using TravelRequests.Application.DTOs;

namespace TravelRequests.Application.Validators
{
    public class UserRegisterValidator : AbstractValidator<UserRegisterDto>
    {
        public UserRegisterValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            RuleFor(x => x.Role).NotEmpty().Must(r => r == "Solicitante" || r == "Aprobador").WithMessage("Role debe ser 'Solicitante' o 'Aprobador'.");
        }
    }
}
