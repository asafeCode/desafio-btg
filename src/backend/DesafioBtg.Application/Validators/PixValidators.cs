using DesafioBtg.Domain.DTOs.Requests;
using FluentValidation;

namespace DesafioBtg.Application.Validators;

public sealed class ProcessPixRequestValidator : AbstractValidator<ProcessPixRequest>
{
    public ProcessPixRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Valor da transacao deve ser maior que zero.")
            .Must(v => decimal.Round(v, 2) == v).WithMessage("Valor da transacao deve ter no maximo 2 casas decimais.");
    }
}

public sealed class UpdatePixLimitRequestValidator : AbstractValidator<UpdatePixLimitRequest>
{
    public UpdatePixLimitRequestValidator()
    {
        RuleFor(x => x.PixLimit)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O limite do PIX nao pode ser negativo.")
            .LessThanOrEqualTo(1_000_000)
            .WithMessage("O limite maximo permitido e 1.000.000.");
    }
}
