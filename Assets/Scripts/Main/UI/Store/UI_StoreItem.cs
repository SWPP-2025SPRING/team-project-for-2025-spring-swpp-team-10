using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_StoreItem : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text priceText;
    public GameObject lockOverlay; // 잠김 상태 시 보여질 오버레이
    public GameObject equippedBadge; // "적용됨" 뱃지

    public void SetData(StoreItemData data)
    {
        iconImage.sprite = data.icon;
        titleText.text = data.title;
        descriptionText.text = data.description;
        priceText.text = $"{data.price}";

        lockOverlay.SetActive(data.isLocked);
        equippedBadge.SetActive(data.isEquipped);
    }
}