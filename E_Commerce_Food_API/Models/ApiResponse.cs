using System.Net;

namespace E_Commerce_Food_API.Models
{
    public class ApiResponse
    {
        public ApiResponse() 
        {
            Errors = new List<string>();
        }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public List<string> Errors { get; set; }
        public object? Result { get; set; }
    }
}
