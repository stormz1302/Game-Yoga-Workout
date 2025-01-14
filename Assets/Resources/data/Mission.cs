﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mission
{
    [Header("Mission Settings")]
    public List<GameObject> missionsHome = new List<GameObject>();
    public List<GameObject> missionsIce = new List<GameObject>();
    public List<GameObject> missionsHLW = new List<GameObject>();
    public List<GameObject> missionsJapan = new List<GameObject>();
    [HideInInspector] public int maxObject;
    [HideInInspector] public float ratioCountBot;
    [HideInInspector] public float ratioCountGoodFood;
    [HideInInspector] public float ratioCountBadFood;
    [HideInInspector] public float ratioCountTrap;

    [Header("Score Settings")]
    public float MAX_BOT_SCORE = 0.4f;
    public float MAX_NOT_MATCHING_BOT_SCORE = 0.3f;
    public float MAX_GOOD_FOOD_SCORE = 0.3f;
    public float MAX_BAD_FOOD_PENALTY = 0.25f;
    public float MAX_TRAP_SCORE = 0.8f;
    //public DifficultyLevel difficultyLevel;  // Độ khó của level
    
    [Header("Time Settings")]
    public float totalTimer;
    public float variation = 0.25f;

    public void SetDifficulty(DifficultyLevel difficultyLevel)
    {
        variation = 0.25f;
        switch (difficultyLevel)
        {
            case DifficultyLevel.Beginner:
                maxObject = 10;
                totalTimer = 20;
                ratioCountBot = 0.3f;       // 15% bots
                ratioCountGoodFood = 0f;  // 25% good food
                ratioCountBadFood = 0f;   // 20% bad food
                ratioCountTrap = 0.7f;     // 40% traps
                break;

            case DifficultyLevel.Novice:
                maxObject = 20;
                totalTimer = 30;
                ratioCountBot = 0.3f;       // 15% bots
                ratioCountGoodFood = 0f;  // 25% good food
                ratioCountBadFood = 0f;   // 20% bad food
                ratioCountTrap = 0.7f;      // 40% traps
                break;

            case DifficultyLevel.Intermediate:
                maxObject = 25;
                totalTimer = 38;
                ratioCountBot = 0.3f;       // 20% bots
                ratioCountGoodFood = 0f;  // 20% good food
                ratioCountBadFood = 0f;   // 20% bad food
                ratioCountTrap = 0.7f;      // 40% traps
                break;

            case DifficultyLevel.Advanced:
                maxObject = 30;
                totalTimer = 45;
                ratioCountBot = 0.3f;       // 15% bots
                ratioCountGoodFood = 0f;  // 30% good food
                ratioCountBadFood = 0f;   // 30% bad food
                ratioCountTrap = 0.7f;      // 50% traps
                break;

            case DifficultyLevel.Expert:
                maxObject = 35;
                totalTimer = 53;
                ratioCountBot = 0.2f;       // 20% bots
                ratioCountGoodFood = 0f;  // 25% good food
                ratioCountBadFood = 0f;   // 25% bad food
                ratioCountTrap = 0.8f;      // 30% traps
                break;
            case DifficultyLevel.Master:
                maxObject = 40;
                totalTimer = 60;
                ratioCountBot = 0.2f;       // 30% bots
                ratioCountGoodFood = 0f;  // 20% good food
                ratioCountBadFood = 0f;   // 20% bad food
                ratioCountTrap = 0.8f;      // 30% traps
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
