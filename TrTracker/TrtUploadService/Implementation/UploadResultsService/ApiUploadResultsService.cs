using TrtShared.DTO;
using TrtUploadService.App.UploadResultsService;

namespace TrtUploadService.Implementation.UploadResultsService
{
    public class ApiUploadResultsService : IUploadResultsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiUploadResultsService> _logger;

        public ApiUploadResultsService(HttpClient httpClient, ILogger<ApiUploadResultsService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> PushResultsToDbAsync(TestRunDTO dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/UploadResults", dto);

                if (response.IsSuccessStatusCode)
                    _logger.LogInformation("Successfully pushed TestRunDTO to API.");
                else
                {
                    _logger.LogError("Failed to push TestRunDTO. StatusCode: {Status}", response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while posting DTO results to API.");
                return false;
            }

            return true;
        }
    }
}
