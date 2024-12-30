using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
public class SteamAchievement : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;

    private int acquiredItemCount;

    private void Start()
    {
        acquiredItemCount = 0;
    }
    private void OnEnable()
    {
        gameManager.onStageEntered += CheckChallenge00;
        gameManager.onStageEntered += CheckChallenge01;
        gameManager.onStageEntered += CheckChallenge02;
        gameManager.onAccumulatedScoreReached += CheckChallenge03;
        gameManager.onAccumulatedScoreReached += CheckChallenge04;
        gameManager.onAccumulatedScoreReached += CheckChallenge05;
        gameManager.onAccumulatedScoreReached += CheckChallenge06;
        gameManager.onBestScoreAchieved += CheckChallenge07;
        gameManager.onBestScoreAchieved += CheckChallenge08;
        gameManager.onBestScoreAchieved += CheckChallenge09;
        BuffMover.onItemAcquired += CheckChallenge10;
        gameManager.onScoreReached += CheckChallenge11;
    }
    private void OnDisable()
    {
        gameManager.onStageEntered -= CheckChallenge00;
        gameManager.onStageEntered -= CheckChallenge01;
        gameManager.onStageEntered -= CheckChallenge02;
        gameManager.onAccumulatedScoreReached -= CheckChallenge03;
        gameManager.onAccumulatedScoreReached -= CheckChallenge04;
        gameManager.onAccumulatedScoreReached -= CheckChallenge05;
        gameManager.onAccumulatedScoreReached -= CheckChallenge06;
        gameManager.onBestScoreAchieved -= CheckChallenge07;
        gameManager.onBestScoreAchieved -= CheckChallenge08;
        gameManager.onBestScoreAchieved -= CheckChallenge09;
        BuffMover.onItemAcquired -= CheckChallenge10;
        gameManager.onScoreReached -= CheckChallenge11;
    }
    private void Achieve(string apiName)
    {
        if (SteamManager.Initialized)
        {
            Steamworks.SteamUserStats.GetAchievement(apiName, out bool isAchieved);

            if (!isAchieved)
            {
                SteamUserStats.SetAchievement(apiName);
                SteamUserStats.StoreStats();
            }
        }
    }

    private void CheckChallenge00(int currentStage)
    {
        if(currentStage == 1)
        {
            Debug.Log("CheckChallenge00");
            Achieve("CHALLENGE_00");
        }
    }

    private void CheckChallenge01(int currentStage)
    {
        if(currentStage == 2)
        {
            Debug.Log("CheckChallenge01");
            Achieve("CHALLENGE_01");
        }
    }
    private void CheckChallenge02(int currentStage)
    {
        if(currentStage == 3)
        {
            Debug.Log("CheckChallenge02");
            Achieve("CHALLENGE_02");
        }
    }
    private void CheckChallenge03(int accumulatedScore)
    {
        if(accumulatedScore >= 1000)
        {
            Debug.Log("CheckChallenge03");
            Achieve("CHALLENGE_03");
        }
    }
    private void CheckChallenge04(int accumulatedScore)
    {
        if(accumulatedScore >= 2000)
        {
            Debug.Log("CheckChallenge04");
            Achieve("CHALLENGE_04");
        }
    }
    private void CheckChallenge05(int accumulatedScore)
    {
        if(accumulatedScore >= 3000)
        {
            Debug.Log("CheckChallenge05");
            Achieve("CHALLENGE_05");
        }
    }
    private void CheckChallenge06(int accumulatedScore)
    {
        if (accumulatedScore >= 5000)
        {
            Debug.Log("CheckChallenge06");
            Achieve("CHALLENGE_06");
        }
    }
    private void CheckChallenge07(int bestScore)
    {
        if(bestScore >= 100)
        {
            Debug.Log("CheckChallenge07");
            Achieve("CHALLENGE_07");
        }
    }
    private void CheckChallenge08(int bestScore)
    {
        if (bestScore >= 300)
        {
            Debug.Log("CheckChallenge08");
            Achieve("CHALLENGE_08");
        }
    }
    private void CheckChallenge09(int bestScore)
    {
        if(bestScore >= 500)
        {
            Debug.Log("CheckChallenge09");
            Achieve("CHALLENGE_09");
        }
    }
    private void CheckChallenge10()
    {
        acquiredItemCount++;
        if(acquiredItemCount == 30)
        {
            Debug.Log("CheckChallenge10");
            Achieve("CHALLENGE_10");
        }
    }
    private void CheckChallenge11(int score)
    {
        if(acquiredItemCount == 0 &&
            score >= 100)
        {
            Debug.Log("CheckChallenge11" + ", " + score );
            Achieve("CHALLENGE_11");
        }
    }

}
