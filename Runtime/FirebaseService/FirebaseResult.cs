#nullable enable
namespace FirebaseService
{
    public class FirebaseResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }

        public static FirebaseResult<T> Success(T data) => new FirebaseResult<T> { IsSuccess = true, Data = data };
        public static FirebaseResult<T> Failure(string errorMessage) => new FirebaseResult<T> { IsSuccess = false, ErrorMessage = errorMessage };
    }

}