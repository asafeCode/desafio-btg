using DesafioBtg.Domain.DTOs.Requests;
using FluentValidation;

namespace DesafioBtg.Application.Validators;

public sealed class AccountRouteRequestValidator : AbstractValidator<AccountRouteRequest>
{
    public AccountRouteRequestValidator()
    {
        RuleFor(x => x.AgencyNumber)
            .NotEmpty().WithMessage("Numero da agencia e obrigatorio.")
            .Matches(@"^\d{4}$").WithMessage("Agencia deve conter exatamente 4 digitos.");

        RuleFor(x => x.AccountNumber)
            .NotEmpty().WithMessage("Numero da conta e obrigatorio.")
            .Matches(@"^\d{5,10}(-\d)?$").WithMessage("Conta invalida (use 5-10 digitos, opcionalmente com digito verificador).");
    }
}
