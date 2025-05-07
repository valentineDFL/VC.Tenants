using FluentResults;
using Microsoft.AspNetCore.Http;

namespace VC.Tenants.Utilities;

public static class ResultExtensions
{
    public static IResult ToMinimalApi<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return Results.Ok(result.Value);
        
        return Results.UnprocessableEntity(result.Errors);
    }
    
    public static IResult ToMinimalApi(this Result result)
    {
        if (result.IsSuccess)
            return Results.NoContent();
        
        return Results.UnprocessableEntity(result.Errors);
    }
}