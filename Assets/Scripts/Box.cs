using UnityEngine;

public class Box : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int posIndex;
    [HideInInspector]
    public BoardManager board;

    public ItemStyle itemStyle;
    private SpriteRenderer sr;


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Init(Vector2Int pos, ItemStyle style, BoardManager _board)
    {
        posIndex = pos;
        itemStyle = style;
        board = _board;

        UpdateVisual(1, 100, 100, 100);
    }

    public void UpdateVisual(int groupSize, int limitA, int limitB, int limitC)
    {
        if (itemStyle != null && sr != null)
        {
            sr.sprite = itemStyle.GetSprite(groupSize, limitA, limitB, limitC);
        }
    }

}