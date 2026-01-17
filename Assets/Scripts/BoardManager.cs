using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField]
    private int height;
    [SerializeField]
    private int width;

    [SerializeField]
    private float tileSpacing = 0.85f;

    public Box[,] allBoxes;

    private Vector2 startPos;

    [Header("Game Rules")]
    public int thresholdA = 4;
    public int thresholdB = 7;
    public int thresholdC = 9;

    private List<Box> matches = new List<Box>();
    private Queue<Box> checkNext = new Queue<Box>();

    [Header("Colors")]
    [SerializeField] private Box boxPrefab;
    [SerializeField] private ItemStyle[] availableStyles;

    [Header("Object Pooling")]
    private Queue<Box> boxPool = new Queue<Box>();
    
    private readonly Vector2Int[] directions = new Vector2Int[]
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    private void Start()
    {
        GenerateGrid();
        UpdateBoardState();
    }

    private void GenerateGrid()
    {
        allBoxes = new Box[width, height];

        startPos = new Vector2(-(width - 1) * tileSpacing / 2f, -(height - 1) * tileSpacing / 2f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                ItemStyle randomStyle = availableStyles[Random.Range(0, availableStyles.Length)];

                Box newBox = GetBoxFromPool();
                newBox.Init(new Vector2Int(x, y), randomStyle, this);

                Vector2 worldPos = startPos + new Vector2(x * tileSpacing, y * tileSpacing);
                newBox.transform.position = worldPos;

                newBox.GetComponent<SpriteRenderer>().sortingOrder = y;

                allBoxes[x, y] = newBox;
            }
        }
    }

    public List<Box> FindMatches(Box startBox) // Breadth first search algoritmasýyla ayný colorID'li bitiþik box'larý arýyoruz
    {
        matches.Clear();
        checkNext.Clear();

        bool[,] visited = new bool[width, height];

        checkNext.Enqueue(startBox);
        visited[startBox.posIndex.x, startBox.posIndex.y] = true;
        matches.Add(startBox);

        while (checkNext.Count > 0)
        {
            Box current = checkNext.Dequeue();

            for (int i = 0; i < directions.Length; i++)
            {
                Vector2Int nextPos = current.posIndex + directions[i];

                if (IsValidPos(nextPos))
                {
                    if (!visited[nextPos.x, nextPos.y])
                    {
                        Box neighbor = allBoxes[nextPos.x, nextPos.y];

                        if (neighbor != null && (neighbor.itemStyle.colorID == startBox.itemStyle.colorID))
                        {
                            visited[nextPos.x, nextPos.y] = true;
                            matches.Add(neighbor);
                            checkNext.Enqueue(neighbor);
                        }
                    }
                }
            }
        }
        return matches;
    }

    private bool IsValidPos(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    public void UpdateBoardState()
    {
        bool[,] visited = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allBoxes[x, y] != null && !visited[x, y])
                {
                    List<Box> group = FindMatches(allBoxes[x, y]);
                    int groupSize = group.Count;

                    for (int i = 0; i < groupSize; i++)
                    {
                        Box member = group[i];
                        member.UpdateVisual(groupSize, thresholdA, thresholdB, thresholdC);
                        visited[member.posIndex.x, member.posIndex.y] = true;
                    }
                }
            }
        }
    }

    private Box GetBoxFromPool()
    {
        if (boxPool.Count > 0)
        {
            Box box = boxPool.Dequeue();
            box.gameObject.SetActive(true);
            return box;
        }

        Box newBox = Instantiate(boxPrefab);
        newBox.transform.parent = transform;
        return newBox;
    }

    private void ReturnToPool(Box box)
    {
        box.gameObject.SetActive(false);
        boxPool.Enqueue(box);
    }

    public void ExplodeGroup(List<Box> group)
    {
        StartCoroutine(ExplodeAndRefillRoutine(group));
    }

    private IEnumerator ExplodeAndRefillRoutine(List<Box> group)
    {
        for (int i = 0; i < group.Count; i++)
        {
            allBoxes[group[i].posIndex.x, group[i].posIndex.y] = null; // Tüm box'lar listesinde box'u null yaptýk

            // Efekt patlatýlabilir

            ReturnToPool(group[i]);
        }

        yield return new WaitForSeconds(0.1f);

        for (int x = 0; x < width; x++)
        {
            List<Box> movingBoxes = new List<Box>();
            for (int y = 0; y < height; y++)
            {
                if (allBoxes[x, y] != null)
                {
                    movingBoxes.Add(allBoxes[x, y]); // Mevcut box'larý ekliyoruz.
                }
            }

            int currentY = 0;

            for (int i = 0; i < movingBoxes.Count; i++) // Patlayan box'lar yerine mevcut box'lar düþüyor
            {
                if (movingBoxes[i].posIndex.y != currentY)
                {
                    movingBoxes[i].posIndex = new Vector2Int(x, currentY);
                    allBoxes[x, currentY] = movingBoxes[i];

                    Vector2 targetPos = startPos + new Vector2(x * tileSpacing, currentY * tileSpacing);
                    movingBoxes[i].MoveToPosition(targetPos);

                    // Görsel derinlik ayarý
                    if (movingBoxes[i].sr != null)
                        movingBoxes[i].sr.sortingOrder = currentY;
                }
                currentY++;
            }

            while (currentY < height) // Üstteki boþluða pool'dan box getiriyoruz
            {
                Box newBox = GetBoxFromPool();
                ItemStyle randomStyle = availableStyles[Random.Range(0, availableStyles.Length)];

                newBox.Init(new Vector2Int(x, currentY), randomStyle, this);
                allBoxes[x, currentY] = newBox;

                Vector2 finalPos = startPos + new Vector2(x * tileSpacing, currentY * tileSpacing);
                // Ekranýn üstünden baþlasýn
                Vector2 spawnPos = finalPos + new Vector2(0, height * tileSpacing);

                newBox.transform.position = spawnPos;
                newBox.MoveToPosition(finalPos);

                if (newBox.sr != null)
                    newBox.sr.sortingOrder = currentY;

                currentY++;
            }
        }

        yield return new WaitForSeconds(0.25f);
        UpdateBoardState();
    }
}