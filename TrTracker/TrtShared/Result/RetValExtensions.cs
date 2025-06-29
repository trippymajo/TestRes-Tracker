using Microsoft.AspNetCore.Mvc;

using TrtShared.RetValType;

namespace TrtShared.RetValExtensions
{
    public static class RetValExtensions
    {
        /// <summary>
        /// Converts RetVal result in to IActionResult for the controllers
        /// </summary>
        /// <param name="controller">Current controller</param>
        /// <param name="result">Result of the workflow service</param>
        /// <returns>IActionResults for HTTP Response</returns>
        public static IActionResult ToActionResult(this ControllerBase controller, RetVal result)
        {
            if (result.Success)
                return controller.Ok();

            return result.ErrorType switch
            {
                ErrorType.BadRequest => controller.BadRequest(result.ErrorText),
                ErrorType.NotFound => controller.NotFound(result.ErrorText),
                ErrorType.Conflict => controller.Conflict(result.ErrorText),
                ErrorType.Forbidden => controller.Forbid(),
                ErrorType.ServerError => controller.StatusCode(500, result.ErrorText),
                ErrorType.Unexpected => controller.StatusCode(500, result.ErrorText),
                null => controller.StatusCode(500, result.ErrorText ?? "Unknown server error"),
                _ => controller.StatusCode(500, result.ErrorText ?? "Unknown server error")
            };
        }

        /// <summary>
        /// Converts RetVal<T> result in to IActionResult for the controllers
        /// </summary>
        /// <param name="controller">Current controller</param>
        /// <param name="result">Result of the workflow service</param>
        /// <returns>IActionResults for HTTP Response</returns>
        public static ActionResult ToActionResult<T>(this ControllerBase controller, RetVal<T> result)
        {
            if (result.Success)
                return controller.Ok(result.Value);

            return result.ErrorType switch
            {
                ErrorType.BadRequest => controller.BadRequest(result.ErrorText),
                ErrorType.NotFound => controller.NotFound(result.ErrorText),
                ErrorType.Conflict => controller.Conflict(result.ErrorText),
                ErrorType.Forbidden => controller.Forbid(),
                ErrorType.ServerError => controller.StatusCode(500, result.ErrorText),
                ErrorType.Unexpected => controller.StatusCode(500, result.ErrorText),
                null => controller.StatusCode(500, result.ErrorText ?? "Unknown server error"),
                _ => controller.StatusCode(500, result.ErrorText ?? "Unknown server error")
            };
        }
    }
}
