using Claims.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Api.Common
{
    /// <summary>
    /// Translates application-layer results into HTTP responses.
    /// </summary>
    public static class ResultExtensions
    {
        private static readonly HashSet<string> NotFoundErrorCodes = new HashSet<string>
        {
            ResultErrorCodes.ClaimNotFound,
            ResultErrorCodes.CoverNotFound,
            ResultErrorCodes.CoverForClaimNotFound
        };

        /// <summary>
        /// Failures caused by the current state of the data rather than by the request itself.
        /// </summary>
        private static readonly HashSet<string> ConflictErrorCodes = new HashSet<string>
        {
            ResultErrorCodes.CoverHasClaims
        };

        /// <summary>
        /// Maps both outcomes: success to 200 OK carrying the value,
        /// failure to 404, 409 or 400
        /// </summary>
        public static ActionResult<T> ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(result.Value);
            }

            return ToErrorResult(result.Error);
        }

        /// <summary>
        /// Maps outcomes: success to 204 No Content,
        /// failure to 404, 409 or 400
        /// </summary>
        public static IActionResult ToNoContentResult(this Result result)
        {
            if (result.IsSuccess)
            {
                return new NoContentResult();
            }

            return ToErrorResult(result.Error);
        }

        /// <summary>
        /// Missing entities map to 404 Not Found, conflicts with existing data to 409 Conflict,
        /// every other expected failure to 400 Bad Request.
        /// </summary>
        private static ActionResult ToErrorResult(string error)
        {
            string code = string.IsNullOrEmpty(error) ? ResultErrorCodes.ValidationFailed : error;
            ErrorResponse body = new ErrorResponse(code);

            if (NotFoundErrorCodes.Contains(code))
            {
                return new NotFoundObjectResult(body);
            }

            if (ConflictErrorCodes.Contains(code))
            {
                return new ConflictObjectResult(body);
            }

            return new BadRequestObjectResult(body);
        }
    }
}
