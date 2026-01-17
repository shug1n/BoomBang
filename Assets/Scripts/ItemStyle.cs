using UnityEngine;

[CreateAssetMenu(fileName = "NewItemStyle", menuName = "Blast/ItemStyle")]
public class ItemStyle : ScriptableObject
{
    public int colorID;

    [Header("Sprite Seti")]
    public Sprite defaultIcon;
    public Sprite iconA;
    public Sprite iconB;
    public Sprite iconC;

    public Sprite GetSprite(int groupSize, int thresholdA, int thresholdB, int thresholdC)
    {
        if (groupSize > thresholdC) return iconC;
        if (groupSize > thresholdB) return iconB;
        if (groupSize > thresholdA) return iconA;

        return defaultIcon;
    }
}