using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AutenticacaoJWT.API.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument == null)
                    continue;

                var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
                if (_serviceProvider.GetService(validatorType) is not IValidator validator)
                    continue;

                var validationContext = new ValidationContext<object>(argument);
                var result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

                if (!result.IsValid)
                {
                    context.Result = new BadRequestObjectResult(new
                    {
                        message = "Dados inválidos.",
                        errors = result.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                    });
                    return;
                }
            }

            await next();
        }
    }
}
