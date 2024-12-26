using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
public class SteamAchievement : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;

    private void OnEnable()
    {
        gameManager.onStageEntered += CheckChallenge00;
        gameManager.onStageEntered += CheckChallenge01;
        gameManager.onStageEntered += CheckChallenge02;
    }
    private void OnDisable()
    {
        gameManager.onStageEntered -= CheckChallenge00;
        gameManager.onStageEntered -= CheckChallenge01;
        gameManager.onStageEntered -= CheckChallenge02;
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
        Debug.Log("Challenge00");
    }

    private void CheckChallenge01(int currentStage)
    {
        Debug.Log("Challenge01");
    }
    private void CheckChallenge02(int currentStage)
    {
        Debug.Log("Challenge02");
    }
    private void CheckChallenge03(int accumulatedScore)
    {
        Debug.Log("Challenge03");
    }
    private void CheckChallenge04(int accumulatedScore)
    {
        Debug.Log("Challenge04");
    }
    private void CheckChallenge05(int accumulatedScore)
    {
        Debug.Log("Challenge05");
    }
    private void CheckChallenge06(int accumulatedScore)
    {
        Debug.Log("Challenge06");
    }
    private void CheckChallenge07(int bestScore)
    {
        Debug.Log("Challenge07");
    }

}
