using Domain.Entities;
using System.Text.Json;

public class ValidationMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
        {
            context.Request.EnableBuffering();
            var bodyAsText = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0; 

            if (string.IsNullOrWhiteSpace(bodyAsText))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("The request body is empty.");
                return;
            }

            // Validate the request body (you can apply specific model validation here)
            var validationResult = ValidateRequest(bodyAsText);

            if (!validationResult.IsValid)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(validationResult.ErrorMessage);
                return;
            }
        }

        // Call the next middleware if validation passes
        await _next(context);
    }

    private ValidationResult ValidateRequest(string requestBody)
    {
        try
        {
            var customer = JsonSerializer.Deserialize<Customer>(requestBody);
            if (customer == null)
            {
                return new ValidationResult(false, "Invalid customer object.");
            }

            if (string.IsNullOrWhiteSpace(customer.FirstName) || string.IsNullOrWhiteSpace(customer.LastName))
            {
                return new ValidationResult(false, "Customer's first and last names are required.");
            }

            return new ValidationResult(true);
        }
        catch (Exception)
        {
            return new ValidationResult(false, "Invalid JSON format.");
        }
    }
}

public class ValidationResult
{
    public bool IsValid { get; }
    public string ErrorMessage { get; }

    public ValidationResult(bool isValid, string errorMessage = null)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }
}
