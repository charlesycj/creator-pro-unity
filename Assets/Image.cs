using UnityEngine;
using UnityEngine.UI;

public class TitleImageAlpha : MonoBehaviour
{
    public Image titleImage; // 타이틀 이미지 (UI Image)
    public float alphaMin = 100f; // 알파 최소값
    public float alphaMax = 230f; // 알파 최대값
    public float speed = 2f; // 알파값 변화 속도

    private Color imageColor;

    void Start()
    {
        // 초기 이미지 색상 가져오기
        if (titleImage != null)
        {
            imageColor = titleImage.color;
        }
    }

    void Update()
    {
        if (titleImage != null)
        {
            // PingPong으로 알파값 계산
            float alpha = Mathf.PingPong(Time.time * speed, alphaMax - alphaMin) + alphaMin;

            // 알파값 적용
            imageColor.a = alpha / 255f; // 알파값을 0~1로 변환
            titleImage.color = imageColor;
        }
    }
}
