using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace FirebaseService
{
    public interface IFirebaseService 
    {
        UniTask<FirebaseResult<List<T>>> GetDataFromCollectionAsync<T>(string collectionName);
        UniTask<FirebaseResult<T>> GetDataByIdAsync<T>(string collectionName, string id);
        UniTask<FirebaseResult<bool>> SetDataAsync<T>(T data, string collectionName, string id);
        UniTask<FirebaseResult<bool>> CreateDataAsync<T>(T data, string collectionName);
        UniTask<FirebaseResult<bool>> UpdateDataAsync(string collectionName, string id, Dictionary<string, object> newData);
    }
}
