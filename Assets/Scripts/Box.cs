using System.Collections;
using UnityEngine;

public class Box : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int posIndex;
    [HideInInspector]
    public BoardManager board;

    public ItemStyle itemStyle;
    public SpriteRenderer sr;

    private bool isMoving = false;

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
        gameObject.SetActive(true);
    }

    public void MoveToPosition(Vector2 targetPos)
    {
        StartCoroutine(MoveRoutine(targetPos));
    }

    private IEnumerator MoveRoutine(Vector2 targetPos)
    {
        isMoving = true;
        float duration = 0.2f;
        Vector2 startPos = transform.position;
        float elapsed = 0;

        while (elapsed < duration)
        {
            transform.position = Vector2.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }

    public void UpdateVisual(int groupSize, int limitA, int limitB, int limitC)
    {
        if (itemStyle != null && sr != null)
        {
            sr.sprite = itemStyle.GetSprite(groupSize, limitA, limitB, limitC);
        }
    }

}