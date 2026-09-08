
namespace FixHub.Application.Common.Models
{
    public class ErrorResponse
    {
        public bool success { get; set; }
        public string errorMessage { get; set; } = string.Empty;
        public List<ValidationError> errors { get; set; } = new();
        public object? data { get; set; }
      
    }
}
