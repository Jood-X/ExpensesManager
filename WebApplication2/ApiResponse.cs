using System.Net;
using System.Runtime.Serialization;

namespace ExpenseManager.Api
{
    public class ApiResponse<T>
    {
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string Error { get; set; }

        public ApiResponse(bool status, string message, T? data, string error= null) 
        {
            Status = status;
            Message = message;
            Data = data;
            Error = error;
        }
        public static ApiResponse<T> SuccessResponse(T data, string message = "Request Successful")
        {
            return new ApiResponse<T>(true, message, data);
        }
        public static ApiResponse<T> ErrorResponse(string message = "Request Failed", string error = null)
        {
            return new ApiResponse<T>(false, message, default, error);
        }
    }
}
