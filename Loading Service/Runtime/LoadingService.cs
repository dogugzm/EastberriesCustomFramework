using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.LoadingService
{
    public class LoadingService
    {
        private int _activeUniTaskCount = 0;
        private bool _isLoading = false;

        private readonly List<UniTask> _uniTaskList = new();

        public event Action<bool> OnLoadingStatusChanged;

        public bool IsLoading => _isLoading;

        public void StartUniTask()
        {
            _activeUniTaskCount++;
            UpdateLoadingState();
        }

        public void CompleteUniTask()
        {
            _activeUniTaskCount--;
            UpdateLoadingState();
        }

        public async UniTask RunUniTask(Func<UniTask> uniTaskFunc)
        {
            StartUniTask();
            try
            {
                await uniTaskFunc();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in UniTask: {ex.Message}");
            }
            finally
            {
                CompleteUniTask();
            }
        }

        private void UpdateLoadingState()
        {
            bool shouldBeLoading = _activeUniTaskCount > 0;

            if (shouldBeLoading != _isLoading)
            {
                _isLoading = shouldBeLoading;
                OnLoadingStatusChanged?.Invoke(_isLoading);
                Debug.Log("Loading: " + (_isLoading ? "Start" : "Finish"));
            }
        }

        public async UniTask RunMultipleUniTasks(IEnumerable<Func<UniTask>> uniTasks)
        {
            foreach (var uniTask in uniTasks)
            {
                await RunUniTask(uniTask);
            }
        }
    }
}