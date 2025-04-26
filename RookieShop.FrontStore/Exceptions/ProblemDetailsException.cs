using Microsoft.AspNetCore.Mvc;

namespace RookieShop.FrontStore.Exceptions;

public class ProblemDetailsException : Exception
{
    public readonly ProblemDetails ProblemDetails;

    public ProblemDetailsException(ProblemDetails problemDetails) : base(problemDetails.Detail ?? problemDetails.Title ?? "An error has occurred.") 
    {
        ProblemDetails = problemDetails;
    }
}

public class UnexpectedException : Exception
{
    public UnexpectedException() : base("An unexpected error has occurred.") {}
}

internal static class ProblemDetailsExceptionExtensions
{
    internal static async Task<T> ReadFromJsonAsync<T>(this HttpResponseMessage response, CancellationToken cancellationToken) where T : class
    {
        await response.EnsureSuccess(cancellationToken);
        
        var result = await response.Content.ReadFromJsonAsync<T>(cancellationToken);
        
        return result!;
    }
    
    internal static async Task EnsureSuccess(this HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken);

            if (problemDetails != null)
            {
                throw new ProblemDetailsException(problemDetails);
            }
            
            throw new UnexpectedException();
        }
    }
}