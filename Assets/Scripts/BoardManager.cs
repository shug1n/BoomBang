using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField]
    private int height = 4;
    [SerializeField]
    private int width = 4;

    [SerializeField]
    private float tileSpacing = 0.85f;

    public Box[,] allBoxes;
    private float scaleFactor;
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

    private AudioSource sfxSource;

    [Header("Object Pooling")]
    private Queue<Box> boxPool = new Queue<Box>();

    [SerializeField] private Vector2 referenceGridSize = new Vector2(4, 4);

    private readonly Vector2Int[] directions = new Vector2Int[]
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    private void Awake()
    {
        sfxSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        CreateBoard(4, 4); // Başlangıçta 4x4
    }

    public void CreateBoard(int newWidth, int newHeight)
    {
        width = newWidth;
        height = newHeight;
        CalculateAutoScale();

        if (allBoxes != null)
        {
            DestroyCurrentBoard();
        }

        GenerateGrid();
        UpdateBoardState();
    }

    private void DestroyCurrentBoard()
    {
        for (int x = 0; x < allBoxes.GetLength(0); x++)
        {
            for (int y = 0; y < allBoxes.GetLength(1); y++)
            {
                if (allBoxes[x, y] != null)
                {
                    ReturnToPool(allBoxes[x, y]);
                }
            }
        }
        allBoxes = null;
    }

    private void CalculateAutoScale()
    {
        float scaleX = referenceGridSize.x / width;
        float scaleY = referenceGridSize.y / height;

        scaleFactor = Mathf.Min(scaleX, scaleY);

        boxPrefab.transform.localScale = Vector3.one * scaleFactor;
        tileSpacing = 0.85f * scaleFactor;
    }

    private void GenerateGrid()
    {
        allBoxes = new Box[width, height];

        startPos = new Vector2(-(width - 1) * tileSpacing / 2f, -(height - 1) * tileSpacing / 2f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                Box newBox = GetBoxFromPool();
                newBox.transform.localScale = Vector3.one * scaleFactor;

                ItemStyle randomStyle = availableStyles[Random.Range(0, availableStyles.Length)];
                newBox.Init(new Vector2Int(x, y), randomStyle, this);

                Vector2 worldPos = startPos + new Vector2(x * tileSpacing, y * tileSpacing);
                newBox.transform.position = worldPos;

                newBox.GetComponent<SpriteRenderer>().sortingOrder = y;

                allBoxes[x, y] = newBox;
            }
        }
    }

    public bool HasAnyValidMoves()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allBoxes[x, y] == null) continue;

                for (int i = 0; i < directions.Length; i++)
                {
                    Vector2Int neighborPos = new Vector2Int(x, y) + directions[i];

                    if (IsValidPos(neighborPos) && allBoxes[neighborPos.x, neighborPos.y] != null)
                    {
                        if (allBoxes[x, y].itemStyle.colorID == allBoxes[neighborPos.x, neighborPos.y].itemStyle.colorID)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public void GuaranteedShuffle()
    {
        List<ItemStyle> usedStyles = new List<ItemStyle>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allBoxes[x, y] != null && !usedStyles.Contains(allBoxes[x, y].itemStyle))
                {
                    usedStyles.Add(allBoxes[x, y].itemStyle);
                }
            }
        }

        int totalBoxes = width * height;
        ItemStyle[] shuffledStyles = new ItemStyle[totalBoxes];

        int index = 0;
        for (int i = 0; i < usedStyles.Count && index + 1 < totalBoxes; i++)
        {
            shuffledStyles[index] = usedStyles[i];
            shuffledStyles[index + 1] = usedStyles[i];
            index += 2;
        }

        while (index < totalBoxes)
        {
            shuffledStyles[index] = usedStyles[Random.Range(0, usedStyles.Count)];
            index++;
        }

        index = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allBoxes[x, y] != null)
                {
                    allBoxes[x, y].itemStyle = shuffledStyles[index];
                    index++;
                }
            }
        }

        UpdateBoardState();
    }

    public List<Box> FindMatches(Box startBox)
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
        if (sfxSource != null)
        {
            float randPitch = Random.Range(.8f, 1.2f);
            sfxSource.pitch = randPitch;
            sfxSource.Play();
        }

        for (int i = 0; i < group.Count; i++)
        {
            allBoxes[group[i].posIndex.x, group[i].posIndex.y] = null;
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
                    movingBoxes.Add(allBoxes[x, y]);
                }
            }

            int currentY = 0;

            for (int i = 0; i < movingBoxes.Count; i++)
            {
                if (movingBoxes[i].posIndex.y != currentY)
                {
                    movingBoxes[i].posIndex = new Vector2Int(x, currentY);
                    allBoxes[x, currentY] = movingBoxes[i];

                    Vector2 targetPos = startPos + new Vector2(x * tileSpacing, currentY * tileSpacing);
                    movingBoxes[i].MoveToPosition(targetPos);

                    if (movingBoxes[i].sr != null)
                        movingBoxes[i].sr.sortingOrder = currentY;
                }
                currentY++;
            }

            while (currentY < height)
            {
                Box newBox = GetBoxFromPool();
                ItemStyle randomStyle = availableStyles[Random.Range(0, availableStyles.Length)];

                newBox.Init(new Vector2Int(x, currentY), randomStyle, this);
                allBoxes[x, currentY] = newBox;

                Vector2 finalPos = startPos + new Vector2(x * tileSpacing, currentY * tileSpacing);
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

        if (!HasAnyValidMoves())
        {
            Debug.Log("<color=yellow> Deadlock tespit edildi! Shuffle yapılıyor...</color>");
            yield return new WaitForSeconds(0.5f);
            GuaranteedShuffle();
        }
    }
}