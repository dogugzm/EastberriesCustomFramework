using UnityEngine;
using VContainer;

namespace LoadingService.LoadingService
{
    public class LoadingUI : MonoBehaviour
    {
        public GameObject loadingSpinner; // Assign a loading spinner in the Inspector

        [Inject] private Assets.Scripts.LoadingService.LoadingService _loadingService;

        private void OnEnable()
        {
            _loadingService.OnLoadingStatusChanged += HandleLoadingStatusChanged;
        }

        private void OnDisable()
        {
            _loadingService.OnLoadingStatusChanged -= HandleLoadingStatusChanged;
        }

        private void HandleLoadingStatusChanged(bool isLoading)
        {
            loadingSpinner.SetActive(isLoading); // Show or hide the spinner based on the loading state
        }
    }
}