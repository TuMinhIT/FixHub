
namespace BookStore.Application.Common.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }

        public object? Errors { get; init; }

        public ApiResponse()
        {
            
        }

        public ApiResponse(T data, string message = "Success")
        {
            Success = true;
            Message = message;
            Data = data;
        }
    }
}
