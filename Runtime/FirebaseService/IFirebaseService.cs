using System.Collections.Generic;
using System.Threading.Tasks;

namespace FirebaseService
{
    public interface IFirebaseService 
    {
        Task<FirebaseResult<List<T>>> GetDataFromCollectionAsync<T>(string collectionName);
        Task<FirebaseResult<T>> GetDataByIdAsync<T>(string collectionName, string id);
        Task<FirebaseResult<bool>> SetDataAsync<T>(T data, string collectionName, string id);
        Task<FirebaseResult<bool>> CreateDataAsync<T>(T data, string collectionName);
        Task<FirebaseResult<bool>> UpdateDataAsync(string collectionName, string id, Dictionary<string, object> newData);
    }
}
