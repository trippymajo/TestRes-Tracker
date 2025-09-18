using TrtShared.RetValType;
using TrtShared.Envelope;

namespace TrtApiService.App.UploadParsedService
{
    public interface IUploadParsedService
    {
        /// <summary>
        /// Workflow for uploading full test run info after parsng test results file into DB
        /// </summary>
        /// <param name="dto">Data transfered from UploadService after parsing</param>
        /// <returns>If uploading results was successfull</returns>
        Task<RetVal> UploadParsedAsync(UniEnvelope envelope);
    }
}
