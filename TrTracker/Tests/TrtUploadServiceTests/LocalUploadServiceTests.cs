using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;
using TrtShared.DTO;

// Tested namespaces
using TrtUploadService.UploadService;
using TrtUploadService.UploadResultsService;

namespace UploadService
{
    public class LocalUploadServiceTests
    {
        #region LocalUploadDocService
        [Fact]
        public async Task SaveFileAsync_ValidFile_FileIsCreated()
        {
            // Init service
            var fakeLogger = NullLogger<LocalUploadDocService>.Instance;
            var uploadService = new LocalUploadDocService(fakeLogger);

            // File workaround
            var tempFileName = Path.Combine(AppContext.BaseDirectory, "samples", "mstest.trx");
            using var fileStream = File.OpenRead(tempFileName);
            var formFile = new FormFile(fileStream, 0, fileStream.Length, "mstest", tempFileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/trx"
            };

            // Act
            var result = await uploadService.SaveFileAsync(formFile);

            // Assert
            Assert.NotNull(result);
            var savedFullPath = Path.Combine(Path.GetTempPath(), "TrtUploads", result);
            Assert.True(File.Exists(savedFullPath));

            // Clean up
            File.Delete(savedFullPath);
        }
        #endregion // LocalUploadDocService


        [Fact]
        public async Task PushResultsToDb_ValidFile_SuccessStatusCode()
        {
            // Fake http client
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/api/UploadResults")
                    .Respond(System.Net.HttpStatusCode.OK);
            var client = mockHttp.ToHttpClient();

            // Init
            var fakeLogger = NullLogger<ApiUploadResultsService>.Instance;
            var uploadService = new ApiUploadResultsService(client,fakeLogger);

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
    }
}
