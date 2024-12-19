using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bestScoreLobyTextMesh; // �ְ� ���� �ؽ�Ʈ (�κ�)

    void Start()
    {
        // �κ� ���� ��� ����Ʈ ���ھ� ǥ��
        if (bestScoreLobyTextMesh != null)
        {
            bestScoreLobyTextMesh.text = "Best Score: " + PlayerPrefs.GetInt("BestScore", 0).ToString("D5");

        }
    }
}
