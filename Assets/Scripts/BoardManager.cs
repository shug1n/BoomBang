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

    [SerializeField]
    private Box[] boxes;

    public Box[,] allBoxes;

    private Vector2 startPos;

    [Header("Game Rules")]
    public int thresholdA = 4;
    public int thresholdB = 7;
    public int thresholdC = 9;

    [Header("Colors")]
    [SerializeField] private Box boxPrefab;
    [SerializeField] private ItemStyle[] availableStyles;

    private void Start()
    {
        GenerateGrid();
        UpdateBoardState();
    }

    private void GenerateGrid()
    {
        allBoxes = new Box[width, height];
        startPos = new Vector2(-(width - 1)*tileSpacing / 2f, -(height - 1)*tileSpacing / 2f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int randNum = Random.Range(0, availableStyles.Length);
                CreateBox(new Vector2Int(x, y), boxPrefab, availableStyles[randNum]);
            }
        }
    }

    private void CreateBox(Vector2Int pos, Box prefab, ItemStyle style)
    {
        Vector2 posSum = startPos + new Vector2(pos.x * tileSpacing, pos.y * tileSpacing);
        Box box = Instantiate(prefab, new Vector3(posSum.x, posSum.y, 0), Quaternion.identity);

        box.transform.parent = this.gameObject.transform;
        box.name = $"{pos.x}, {pos.y}";
        SpriteRenderer sr = box.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = pos.y;
        }

        box.Init(pos, style, this);
        allBoxes[pos.x, pos.y] = box;
    }
    private readonly Vector2Int[] directions = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    public List<Box> FindMatches(Box startBox) // Breadth First Search ile match arama fonksiyonu
    {
        List<Box> matches = new List<Box>();
        Queue<Box> checkNext = new Queue<Box>();
        bool[,] visited = new bool[width, height];

        checkNext.Enqueue(startBox);
        visited[startBox.posIndex.x, startBox.posIndex.y] = true;
        matches.Add(startBox);

        while (checkNext.Count > 0)
        {
            Box current = checkNext.Dequeue();

            for(int i = 0; i < directions.Length; i++)
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
                            checkNext.Enqueue(neighbor); // Bunun da komþularýna bakmak için sýraya alýyoruz
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
}
