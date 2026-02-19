using DesafioBtg.Domain.DTOs.Requests;
using FluentValidation;

namespace DesafioBtg.Application.Validators;

public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.NationalId)
            .NotEmpty().WithMessage("CPF e obrigatorio.")
            .Matches(@"^\d{11}$").WithMessage("CPF deve conter 11 digitos numericos.");

        RuleFor(x => x.AgencyNumber)
            .NotEmpty().WithMessage("Numero da agencia e obrigatorio.")
            .Matches(@"^\d{4}$").WithMessage("Agencia deve conter exatamente 4 digitos.");

        RuleFor(x => x.AccountNumber)
            .NotEmpty().WithMessage("Numero da conta e obrigatorio.")
            .Matches(@"^\d{5,10}(-\d)?$").WithMessage("Conta invalida (use 5-10 digitos, opcionalmente com digito verificador).");

        RuleFor(x => x.PixLimit)
            .GreaterThanOrEqualTo(0).WithMessage("Limite PIX nao pode ser negativo.");
    }
}
