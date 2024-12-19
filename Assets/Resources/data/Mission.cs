using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mission
{
    [Header("Mission Settings")]
    public List<GameObject> missions = new List<GameObject>();
    [HideInInspector] public int maxObject;
    [HideInInspector] public float ratioCountBot;
    [HideInInspector] public float ratioCountGoodFood;
    [HideInInspector] public float ratioCountBadFood;
    [HideInInspector] public float ratioCountTrap;

    [Header("Score Settings")]
    public float MAX_BOT_SCORE = 0.4f;
    public float MAX_NOT_MATCHING_BOT_SCORE = 0.3f;
    public float MAX_GOOD_FOOD_SCORE = 0.5f;
    public float MAX_BAD_FOOD_PENALTY = 0.25f;
    public float MAX_TRAP_SCORE = 0.3f;
    public DifficultyLevel difficultyLevel;  // Độ khó của level
    
    [Header("Time Settings")]
    public float totalTimer;
    public float variation = 0.25f;

    public void SetDifficulty()
    {
        variation = 0.25f;
        switch (difficultyLevel)
        {
            case DifficultyLevel.Beginner:
                maxObject = 10;
                totalTimer = 30;
                ratioCountBot = 0.1f;       // 10% bots
                ratioCountGoodFood = 0.7f;  // 70% good food
                ratioCountBadFood = 0.1f;   // 10% bad food
                ratioCountTrap = 0.1f;     // 10% traps
                break;

            case DifficultyLevel.Novice:
                maxObject = 20;
                totalTimer = 50;
                ratioCountBot = 0.1f;       // 10% bots
                ratioCountGoodFood = 0.55f;  // 55% good food
                ratioCountBadFood = 0.2f;   // 20% bad food
                ratioCountTrap = 0.15f;      // 15% traps
                break;

            case DifficultyLevel.Intermediate:
                maxObject = 20;
                totalTimer = 45;
                ratioCountBot = 0.15f;       // 15% bots
                ratioCountGoodFood = 0.4f;  // 40% good food
                ratioCountBadFood = 0.25f;   // 25% bad food
                ratioCountTrap = 0.2f;      // 20% traps
                break;

            case DifficultyLevel.Advanced:
                maxObject = 30;
                totalTimer = 60;
                ratioCountBot = 0.15f;       // 15% bots
                ratioCountGoodFood = 0.3f;  // 30% good food
                ratioCountBadFood = 0.3f;   // 30% bad food
                ratioCountTrap = 0.25f;      // 25% traps
                break;

            case DifficultyLevel.Expert:
                maxObject = 50;
                totalTimer = 90;
                ratioCountBot = 0.4f;       // 40% bots
                ratioCountGoodFood = 0.2f;  // 20% good food
                ratioCountBadFood = 0.3f;   // 30% bad food
                ratioCountTrap = 0.3f;      // 30% traps
                break;
            case DifficultyLevel.Master:
                maxObject = 80;
                totalTimer = 130;
                ratioCountBot = 0.4f;       // 40% bots
                ratioCountGoodFood = 0.2f;  // 20% good food
                ratioCountBadFood = 0.3f;   // 30% bad food
                ratioCountTrap = 0.3f;      // 30% traps
                break;
        }
    }
}

// Enum for difficulty levels
public enum DifficultyLevel
{
    Beginner,        // Beginner level
    Novice,          // Novice level
    Intermediate,    // Intermediate level
    Advanced,        // Advanced level
    Expert,     // Professional level
    Master
}
