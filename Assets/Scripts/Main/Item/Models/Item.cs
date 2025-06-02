using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class ItemMeta
{
    [Tooltip("아이템 고유 ID")]
    public int id;

    [Tooltip("아이템 기본 이름")]
    public string name;

    [Tooltip("아이템 기본 설명")]
    public string description;

    [Tooltip("아이템 아이콘")]
    public Sprite image;
}



/// <summary>
/// 개별 아이템에 대한 상세 데이터 클래스
/// </summary>
[Serializable]
public class Item
{
    public ItemMeta meta;

    [Tooltip("아이템의 레벨별 데이터")]
    public ItemLevelData[] levels;

    [Tooltip("아이템 기본 설명")]
    public string description;

    // 현재 적용 중인 레벨 (선택사항)
    public int currentLevel = 0;

    public int CurrentPrice => levels != null && currentLevel < levels.Length
        ? levels[currentLevel].price
        : 0;

    public static Item Create(
        int id,
        string name,
        string description,
        Sprite image = null,
        ItemLevelData[] levels = null
    )
    {
        return new Item
        {
            meta = new ItemMeta
            {
                id = id,
                name = name,
                description = description,
                image = image
            },
            levels = levels
        };
    }
}