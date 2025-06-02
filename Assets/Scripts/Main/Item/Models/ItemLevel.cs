using System;
using UnityEngine;

public enum ItemLevelType
{
    Ratio, // 비율 증가
    Absolute // 고정 증가
}

public class ItemLevelMeta
{
    public ItemLevelType type;
    public float increment;
}

[Serializable]
public class ItemLevelData
{
    [Tooltip("해당 레벨 가격")]
    public int price;

    [Tooltip("해당 레벨의 이름(Optional)")]
    public string name;

    [Tooltip("해당 레벨 설명(Optional)")]
    public string description;

    [Tooltip("해당 레벨의 아이콘(Optional)")]
    public Sprite image;

    [Tooltip("해당 레벨에서 적용되는 수치값 (예: 소모율, 시간, 길이 등)")]
    public float value;

}