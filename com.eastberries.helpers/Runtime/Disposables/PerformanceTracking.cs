using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Helpers.Disposables
{
    /// <summary>
    /// Performance tracking class that measures the time taken for an operation.
    /// <para>
    /// This class uses a Stopwatch to measure the duration of an operation and logs the result when disposed.
    /// </para>
    /// <example>
    /// Below is an example of how to use the PerformanceTracking class to measure the time taken for a data processing operation:
    /// <code>
    /// using Helpers.Disposables;
    /// using UnityEngine;
    ///
    /// public class DataProcessor : MonoBehaviour
    /// {
    ///     void Start()
    ///     {
    ///         using (new PerformanceTracking("Data Processing"))
    ///         {
    ///             // Simulate a time-consuming operation
    ///             for (int i = 0; i &lt; 1000000; i++)
    ///             {
    ///                 float result = Mathf.Sin(i);
    ///             }
    ///         }
    ///     }
    /// }
    /// </code>
    /// This will output logs like:
    /// <code>
    /// Starting operation: Data Processing
    /// Operation Data Processing completed in 123 ms
    /// </code>
    /// </example>
    /// </summary>
    public class PerformanceTracking : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly string _operationInfo;
        private bool _isDisposed;

        public PerformanceTracking(string infoText)
        {
            _stopwatch = new Stopwatch();
            _operationInfo = infoText;
            _stopwatch.Start();
            _isDisposed = false;
            Debug.Log($"Starting operation: {_operationInfo}");
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _stopwatch.Stop();
            Debug.Log($"Operation {_operationInfo} completed in {_stopwatch.ElapsedMilliseconds} ms");
            _isDisposed = true;
        }
    }
}