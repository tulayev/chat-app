using ChatApp.Application.Helpers;

namespace ChatApp.Tests.Helpers
{
    public class ApiResponseTests
    {
        [Fact]
        public void Ok_SetsSuccessTrue_DataProvided_ErrorMessageNull()
        {
            var response = ApiResponse<string>.Ok("data");

            Assert.True(response.Success);
            Assert.Equal("data", response.Data);
            Assert.Null(response.ErrorMessage);
        }

        [Fact]
        public void Fail_SetsSuccessFalse_DataIsDefault_ErrorMessageSet()
        {
            var response = ApiResponse<string>.Fail("error");

            Assert.False(response.Success);
            Assert.Null(response.Data);
            Assert.Equal("error", response.ErrorMessage);
        }
    }
}
