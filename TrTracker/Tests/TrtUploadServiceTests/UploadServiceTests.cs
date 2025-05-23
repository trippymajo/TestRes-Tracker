using Microsoft.Extensions.Logging.Abstractions;

using RichardSzalay.MockHttp;

// Tested namespaces
using TrtUploadService.UploadResultsService;
using TrtShared.DTO;

namespace TrtUploadServiceTests
{
    public class UploadServiceTests
    {
        #region ApiUploadResultsService
        [Fact]
        public async Task PushResultsToDb_ValidFile_SuccessStatusCode()
        {
            // Fake http client
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/api/UploadResults")
                    .Respond(System.Net.HttpStatusCode.OK);
            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://localhost");

            // Init
            var fakeLogger = NullLogger<ApiUploadResultsService>.Instance;
            var uploadService = new ApiUploadResultsService(client, fakeLogger);

            // Parsed DTO workaround
            var listResultDtos = new List<ResultDTO>();
            for (int i = 0; i < 10; i++)
            {
                var resultDto = new ResultDTO
                {
                    TestName = string.Format("TestNo{0}", i),
                    Outcome = (i % 2 == 0).ToString(),
                    ErrMsg = string.Format("Error message sample{0}", i)
                };

                listResultDtos.Add(resultDto);
            }

            var testRunDTO = new TestRunDTO()
            {
                Branch = "666",
                Version = "6.6.6",
                Date = DateTime.Now,
                Results = listResultDtos
            };

            // Act
            var retVal = await uploadService.PushResultsToDb(testRunDTO);

            // Assert
            Assert.True(retVal);
        }
        #endregion // ApiUploadResultsService
    }
}
