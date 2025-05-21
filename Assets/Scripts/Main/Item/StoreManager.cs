using UnityEngine;
using System.Collections.Generic;

public class StoreManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject storeItemPrefab;
    public Transform itemParent;

    [Header("Mock Data")]
    public List<StoreItemData> storeItems;

    void Start()
    {
        PopulateStore(storeItems);
    }

    public void PopulateStore(List<StoreItemData> items)
    {
        // 기존 자식 제거
        foreach (Transform child in itemParent)
        {
            Destroy(child.gameObject);
        }

        // 새로운 아이템 추가
        foreach (var data in items)
        {
            var go = Instantiate(storeItemPrefab, itemParent);
            var itemUI = go.GetComponent<StoreItemUI>();
            itemUI.SetData(data);
        }
    }
}