using Microsoft.AspNetCore.Mvc;

namespace TrtShared.RetVal.Extensions
{
    public static class RetValExtensions
    {
        public static IActionResult ToActionResult(this ControllerBase controller, RetVal result)
        {
            if (result.Success)
                return controller.Ok();

            return result.ErrorType switch
            {
                ErrorType.BadRequest => controller.BadRequest(result.ErrorType),
                ErrorType.NotFound => controller.NotFound(result.ErrorType),
                ErrorType.Conflict => controller.Conflict(result.ErrorType),
                ErrorType.Forbidden => controller.Forbid(),
                ErrorType.ServerError => controller.StatusCode(500, result.ErrorText),
                ErrorType.Unexpected => controller.StatusCode(500, result.ErrorText),
                null => controller.StatusCode(500, result.ErrorText ?? "Unknown server error"),
                _ => controller.StatusCode(500, result.ErrorText ?? "Unknown server error")
            };
        }

        public static IActionResult ToActionResult<T>(this ControllerBase controller, Result<T> result)
        {
            if (result.Success)
                return controller.Ok(result.Value);

            return controller.ToActionResult((RetVal)result);
        }
    }
}
