﻿namespace KTRegistration.API.Helpers;
public static class ResultExtensions
{
    public static ObjectResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot Convert Success Result to Problem");
        }

        var problem = Results.Problem(statusCode: result.Error.StatusCode);

        // using reflection we can access the properties and update it for this class
        var problemDetails = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;
        problemDetails!.Extensions = new Dictionary<string, object?>
        {
                                   { "errors", new[] { result.Error.Code, result.Error.Description } }

        };
        return new ObjectResult(problemDetails);
    }
}