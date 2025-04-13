using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoostUI : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private Color boostTextColor;

    private Image[] image;  
    private TextMeshProUGUI txt;
    private float startEnergy;
    float boostTime;

    void Start()
    {
        image = GetComponentsInChildren<Image>();
        txt = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        ImageControl();
        TextControl();  
    }

    void ImageControl()
    {
        float energy = player.currentBoostEnergy;

        Color color = image[0].color;
        image[1].fillAmount = energy;

        if (player.isBoost) {
            if (boostTime == 0) { // 부스트 시작
                startEnergy = energy;
            }
            SetImageAlpha(energy / startEnergy);
            
            if (boostTime < 0.2f) { // 부스터 시작하고 0.2초 동안 이미지의 크기 조절
                SetImageSize(Mathf.Lerp(120, 100, boostTime / 0.2f));
                boostTime += Time.deltaTime;
            }
            else {
                SetImageSize(100);
            }
        }
        else {
            boostTime = 0;
            SetImageSize(100);

            if (energy >= player.burstEnergyUsage) { // 부스터를 쓸 수 있는 조건
                SetImageAlpha(Mathf.Min(1, color.a + 3 * Time.deltaTime));
            }
            else {
                SetImageAlpha(0);
            }
        }
    }

    void SetImageSize(float size)
    {
        image[0].rectTransform.sizeDelta = image[1].rectTransform.sizeDelta = Vector3.one * size;
    }
    void SetImageAlpha(float a)
    {
        image[0].color = new Color(image[0].color.r, image[0].color.g, image[0].color.b, a);
    }

    void TextControl()
    {
        if (player.isBoost) {
            txt.rectTransform.anchoredPosition = new Vector3(-3, -55, 0);
            txt.color = boostTextColor;
        }
        else {
            txt.rectTransform.anchoredPosition = new Vector3(0, -54, 0);
            txt.color = Color.black;
        }
    }
}
