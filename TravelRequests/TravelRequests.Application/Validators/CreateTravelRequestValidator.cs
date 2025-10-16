using FluentValidation;
using TravelRequests.Application.DTOs;

namespace TravelRequests.Application.Validators
{
    public class CreateTravelRequestValidator : AbstractValidator<CreateTravelRequestDto>
    {
        public CreateTravelRequestValidator()
        {
            RuleFor(x => x.OriginCity).NotEmpty().WithMessage("La ciudad de origen es requerida.");
            RuleFor(x => x.DestinationCity).NotEmpty().WithMessage("La ciudad de destino es requerida.");
            RuleFor(x => x.Justification).NotEmpty().WithMessage("La justificación es requerida.");
            RuleFor(x => x.DepartureDate).LessThan(x => x.ReturnDate).WithMessage("La fecha de salida debe ser anterior a la de regreso.");
            RuleFor(x => x.ReturnDate).GreaterThan(x => x.DepartureDate).WithMessage("La fecha de regreso debe ser posterior a la de ida.");
            RuleFor(x => x).Custom((dto, ctx) =>
            {
                if (!string.IsNullOrWhiteSpace(dto.OriginCity) && !string.IsNullOrWhiteSpace(dto.DestinationCity) &&
                    string.Equals(dto.OriginCity.Trim(), dto.DestinationCity.Trim(), System.StringComparison.OrdinalIgnoreCase))
                {
                    ctx.AddFailure("OriginCity", "La ciudad de origen y destino no pueden ser la misma.");
                }
            });
        }
    }
}
