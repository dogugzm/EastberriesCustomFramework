#nullable enable
namespace FirebaseService
{
    public class FirebaseResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }

        public static FirebaseResult<T> Success(T data) => new() { IsSuccess = true, Data = data };

        public static FirebaseResult<T> Failure(string errorMessage) =>
            new() { IsSuccess = false, ErrorMessage = errorMessage };
    }
}