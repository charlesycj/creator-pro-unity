using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bestScoreLobyTextMesh; // 최고 점수 텍스트 (로비)

    void Start()
    {
        // 로비 씬일 경우 베스트 스코어 표시
        if (bestScoreLobyTextMesh != null)
        {
            bestScoreLobyTextMesh.text = "Best Score: " + PlayerPrefs.GetInt("BestScore", 0).ToString("D5");

        }
    }
}
