
namespace BookStore.Application.Common.Models
{
    public class ErrorResponse
    {
        public bool success { get; set; }
        public string errorMessage { get; set; }
        public List<ValidationError> errors { get; set; }
        public object data { get; set; }
      
    }
}
