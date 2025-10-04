namespace Core.Helpers
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }

        public ApiResponse(bool success, T? data, string? errorMessage)
        {
            Success = success;
            Data = data;
            ErrorMessage = errorMessage;
        }

        public static ApiResponse<T> Ok(T data)
        {
            return new(true, data, null);
        }

        public static ApiResponse<T> Fail(string error)
        {
            return new(false, default, error);
        }
    }
}
