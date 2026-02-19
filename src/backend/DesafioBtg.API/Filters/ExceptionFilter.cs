using System.Net;
using DesafioBtg.Domain.DTOs.Exceptions.Base;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DesafioBtg.API.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case ValidationException validationException:
                HandleValidationException(validationException, context);
                break;

            case AppException appException:
                HandleProjectException(appException, context);
                break;

            default:
                ThrowUnknownException(context);
                break;
        }
    }
    private static void HandleValidationException(
        ValidationException validationException,
        ExceptionContext context)
    {
        const int statusCode = (int)HttpStatusCode.BadRequest;

        var errors = validationException.Errors
            .Select(e => e.ErrorMessage)
            .ToList();

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = "Validation error",
            Detail = errors.FirstOrDefault(),
            Extensions =
            {
                ["errors"] = errors
            }
        };

        context.HttpContext.Response.StatusCode = statusCode;
        context.Result = new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }

    private static void HandleProjectException(AppException appException, ExceptionContext context)
    {
        var statusCode = (int)appException.GetStatusCode();
        var errors = appException.GetErrorMessages();

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = "Business error",
            Detail = errors.FirstOrDefault(),
            Extensions =
            {
                ["errors"] = errors,
            }
        };

        context.HttpContext.Response.StatusCode = statusCode;
        context.Result = new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }

    private static void ThrowUnknownException(ExceptionContext context)
    {
        const int statusCode = (int)HttpStatusCode.InternalServerError;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = "Unknown error",
            Detail = "Erro Desconhecido",
            Extensions =
            {
                ["errors"] = new List<string> { "Erro Desconhecido" },
            }
        };

        context.HttpContext.Response.StatusCode = statusCode;
        context.Result = new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }
}
