using System.Collections.Generic;
using UnityEngine;

namespace MockDataSystem
{
    public class MockDataTest : MonoBehaviour
    {
        public struct PlayerData
        {
            [CustomAttributes.IntValue(100)] public int health;

            [CustomAttributes.FloatRandom(5f, 10f)]
            public float speed;

            [CustomAttributes.StringValue("Player1")]
            public string username;
        }

        public class PlayerDataClass
        {
            [CustomAttributes.IntValue(100)] public int health;

            [CustomAttributes.FloatRandom(5f, 10f)]
            public float speed;

            [CustomAttributes.StringValue("Player1")]
            public string username;
        }

        public struct SampleData
        {
            [CustomAttributes.NestedMock] public PlayerDataClass playerClass;

            [CustomAttributes.NestedMock] public PlayerData player;
            [CustomAttributes.IntValue(5)] public int orderNumber;

            [CustomAttributes.IntRandom(3, 6)] public int randomNumber;

            [CustomAttributes.StringValue("aaa")] public string name;

            [CustomAttributes.FloatValue(3.14f)] public float pi;

            [CustomAttributes.FloatRandom(0f, 1f)] public float randomFloat;

            [CustomAttributes.BoolValue(true)] public bool isActive;

            [CustomAttributes.BoolRandom] public bool randomFlag;

            [CustomAttributes.ListValue(1, 2, 3)] public List<int> fixedNumbers;

            [CustomAttributes.ListRandom(2, 5, typeof(CustomAttributes.IntRandomAttribute), 10, 20)]
            public List<int> randomNumbers;

            [CustomAttributes.ListRandom(1, 3, typeof(CustomAttributes.FloatRandomAttribute), 3, 8)]
            public List<float> randomStrings;
        }

        private MockService _mockService;

        void Awake()
        {
            _mockService = new MockService();
        }

        void Start()
        {
            Generate();
        }

        [ContextMenu("Generate")]
        private void Generate()
        {
            _mockService ??= new MockService();

            SampleData mockData = _mockService.CreateMock<SampleData>();
            Debug.Log($"orderNumber: {mockData.orderNumber}");
            Debug.Log($"randomNumber: {mockData.randomNumber}");
            Debug.Log($"name: {mockData.name}");
            Debug.Log($"pi: {mockData.pi}");
            Debug.Log($"randomFloat: {mockData.randomFloat}");
            Debug.Log($"isActive: {mockData.isActive}");
            Debug.Log($"randomFlag: {mockData.randomFlag}");

            Debug.Log($"fixedNumbers: {string.Join(", ", mockData.fixedNumbers)}");
            Debug.Log($"randomNumbers: {string.Join(", ", mockData.randomNumbers)}");
            Debug.Log($"randomStrings: {string.Join(", ", mockData.randomStrings)}");
            Debug.Log(
                $"player: {mockData.player.username}, Health: {mockData.player.health}, Speed: {mockData.player.speed}");
        }
    }
}