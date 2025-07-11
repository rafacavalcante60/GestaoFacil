namespace GestaoFacil.Server.Services
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; } = 0;
        public T? Data { get; set; }

        public static ServiceResult<T> Ok(T data) => new() { Success = true, Data = data };
        public static ServiceResult<T> Ok(string message) => new() { Success = true, Message = message };
        public static ServiceResult<T> Fail(string message, int statusCode = 0) =>
            new() { Success = false, Message = message, StatusCode = statusCode };
    }


}
