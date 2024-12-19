using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetObjectInLevel : MonoBehaviour
{
    [Header("Prefab Bot")]
    public GameObject botPrefab; // Bot là một object duy nhất
    [SerializeField] private float ratioCountBot;
    [SerializeField] private float MAX_BOT_SCORE = 0.4f;    // Bot chiếm tối đa 40% tổng điểm
    [SerializeField] private float MAX_NOT_MATCHING_BOT_SCORE = -0.3f; // Các vật phẩm khác trừ tối đa 30% tổng điểm

    [Header("Prefab Good Food")]
    public List<GameObject> goodFoodPrefabs; // Danh sách prefab cho Good Food
    [SerializeField] private float ratioCountGoodFood;
    [SerializeField] private float MAX_GOOD_FOOD_SCORE = 0.5f;  // Good Food chiếm tối đa 50% tổng điểm

    [Header("Prefab Bad Food")]
    public List<GameObject> badFoodPrefabs; // Danh sách prefab cho Bad Food
    [SerializeField] private float ratioCountBadFood;
    [SerializeField] private float MAX_BAD_FOOD_PENALTY = -0.25f;  // Bad Food trừ tối đa 25% tổng điểm

    [Header("Prefab Trap")]
    public List<GameObject> trapPrefabs; // Danh sách prefab cho Trap
    [SerializeField] private float ratioCountTrap;
    [SerializeField] private float MAX_TRAP_SCORE = 0.3f;

    [Header("OBJECT COUNT:")]
    public int maxObjectInLevel;
    int remainingObjects;
    public float totalTimer;
    float remainingTime;
    float variation = 0.2f;


    public Transform spawnPoint;

    private int botSpawned = 0, goodFoodSpawned = 0, badFoodSpawned = 0, trapSpawned = 0;
    private int botCount, goodFoodCount, badFoodCount, trapCount;

    [HideInInspector] public int botScorePerObject;
    [HideInInspector] public int goodFoodScorePerObject;
    [HideInInspector] public int trapScorePerObject;
    [HideInInspector] public int badFoodPenaltyPerObject;
    [HideInInspector] public int notMatchingPenaltyPerObject;

    [SerializeField] FoodObjects foodObjects;

    bool botIsSpawning = false;

    private void Update()
    {
        
    }

    public void SetupLevel()
    {
        int Level = 0;
        trapPrefabs = GameManager.Instance.missions;
        maxObjectInLevel = GameManager.Instance.maxObject;
        Level = GameManager.Instance.animIndex;
        goodFoodPrefabs = foodObjects.goodFoodPrefabs;
        badFoodPrefabs = foodObjects.badFoodPrefabs;
        ratioCountBot = GameManager.Instance.MissionListSO.missionLevels[Level].ratioCountBot;
        ratioCountGoodFood = GameManager.Instance.MissionListSO.missionLevels[Level].ratioCountGoodFood;
        ratioCountBadFood = GameManager.Instance.MissionListSO.missionLevels[Level].ratioCountBadFood;
        ratioCountTrap = GameManager.Instance.MissionListSO.missionLevels[Level].ratioCountTrap;

        MAX_BOT_SCORE = GameManager.Instance.MissionListSO.missionLevels[Level].MAX_BOT_SCORE;
        MAX_NOT_MATCHING_BOT_SCORE = GameManager.Instance.MissionListSO.missionLevels[Level].MAX_NOT_MATCHING_BOT_SCORE;
        MAX_GOOD_FOOD_SCORE = GameManager.Instance.MissionListSO.missionLevels[Level].MAX_GOOD_FOOD_SCORE;
        MAX_BAD_FOOD_PENALTY = GameManager.Instance.MissionListSO.missionLevels[Level].MAX_BAD_FOOD_PENALTY;
        MAX_TRAP_SCORE = GameManager.Instance.MissionListSO.missionLevels[Level].MAX_TRAP_SCORE;

        totalTimer = GameManager.Instance.MissionListSO.missionLevels[Level].totalTimer;
        variation = GameManager.Instance.MissionListSO.missionLevels[Level].variation;
        InitializeSpawnCounts();
    }

    private void InitializeSpawnCounts()
    {
        // Tính số lượng object cho từng loại
        botCount = Mathf.RoundToInt(maxObjectInLevel * ratioCountBot);
        goodFoodCount = Mathf.RoundToInt(maxObjectInLevel * ratioCountGoodFood);
        badFoodCount = Mathf.RoundToInt(maxObjectInLevel * ratioCountBadFood);
        trapCount = Mathf.RoundToInt(maxObjectInLevel * ratioCountTrap);
    }

    public GameObject SpawnObjects()
    {
        GameObject objectSpawned;
        int i = 0;
        if (i < maxObjectInLevel)
        {
            if (botSpawned >= botCount && goodFoodSpawned >= goodFoodCount && badFoodSpawned >= badFoodCount && trapSpawned >= trapCount)
            {
                return null;
            }

            // Chọn ngẫu nhiên loại object để spawn 
            string objectType = GetRandomObjectType();

            switch (objectType)
            {
                case "Bot":
                    
                    if (botSpawned < botCount)
                    {
                        objectSpawned = botPrefab;
                        botIsSpawning = true;
                        botSpawned++;
                        i++;
                        return objectSpawned;
                    }
                    break;

                case "GoodFood":
                    if (goodFoodSpawned < goodFoodCount)
                    {
                        objectSpawned = SpawnObject(goodFoodPrefabs, goodFoodSpawned);

                        goodFoodSpawned++;
                        i++;
                        return objectSpawned;
                    }
                    break;

                case "BadFood":
                    if (badFoodSpawned < badFoodCount)
                    {
                        objectSpawned = SpawnObject(badFoodPrefabs, badFoodSpawned);
                        badFoodSpawned++;
                        i++;
                        return objectSpawned;
                    }
                    break;

                case "Trap":
                    if (trapSpawned < trapCount)
                    {
                        objectSpawned = SpawnObject(trapPrefabs, trapSpawned);
                        trapSpawned++;
                        i++;
                        return objectSpawned;
                    }
                    break;
                default:
                    break;
            }
        }
        return null;
    }

    private GameObject SpawnObject(List<GameObject> objects, int spawnedCount)
    {
        GameObject objectToSpawn = objects[Random.Range(0, objects.Count)];
        return objectToSpawn;
    }

    
    private string GetRandomObjectType()
    {
        List<string> availableTypes = new List<string>();

        if (botSpawned < botCount) availableTypes.Add("Bot");
        if (goodFoodSpawned < goodFoodCount) availableTypes.Add("GoodFood");
        if (badFoodSpawned < badFoodCount) availableTypes.Add("BadFood");
        if (trapSpawned < trapCount) availableTypes.Add("Trap");
        int randomType = Random.Range(0, availableTypes.Count);
        if (availableTypes[randomType] == "Bot" && botIsSpawning)
        {
            randomType = Random.Range(1, availableTypes.Count);
        }
        return availableTypes[randomType];
    }

    public void SpawnedBot()
    {
        botIsSpawning = false;
    }

    public void AssignScoresToObjects()
    {
        // Tính điểm cho mỗi object dựa trên số lượng
        botScorePerObject = botCount > 0 ? Mathf.FloorToInt(MAX_BOT_SCORE * 100 / botCount) : 0;
        goodFoodScorePerObject = goodFoodCount > 0 ? Mathf.FloorToInt(MAX_GOOD_FOOD_SCORE * 100 / goodFoodCount) : 0;
        trapScorePerObject = trapCount > 0 ? Mathf.FloorToInt(MAX_TRAP_SCORE * 100 / trapCount) : 0;
        badFoodPenaltyPerObject = badFoodCount > 0 ? Mathf.FloorToInt(MAX_BAD_FOOD_PENALTY * 100 / badFoodCount) : 0;
        notMatchingPenaltyPerObject = Mathf.FloorToInt(MAX_NOT_MATCHING_BOT_SCORE * 100 / botCount);
    }

    public float SetSpawnRate()
    {
        remainingObjects = maxObjectInLevel;
        remainingTime = totalTimer;

        // Tính khoảng thời gian spawn tiếp theo
        float minTime = (remainingTime / remainingObjects) * (1 - variation);
        float maxTime = (remainingTime / remainingObjects) * (1 + variation);
        float spawnInterval = Random.Range(minTime, maxTime);

        // Nếu chỉ còn 1 đối tượng, dùng toàn bộ thời gian còn lại
        if (remainingObjects == 1)
        {
            spawnInterval = remainingTime;
        }
        remainingTime -= spawnInterval;
        return spawnInterval;
    }
}
