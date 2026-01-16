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

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        allBoxes = new Box[width, height];
        startPos = new Vector2(-(width - 1)*tileSpacing / 2f, -(height - 1)*tileSpacing / 2f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int randNum = Random.Range(0, boxes.Length);
                CreateBox(new Vector2Int(x,y), boxes[randNum], randNum);
            }
        }
    }

    private void CreateBox(Vector2Int pos, Box bossPrefab, int randNum)
    {
        Vector2 posSum = startPos + new Vector2(pos.x * tileSpacing, pos.y * tileSpacing);
        Box box = Instantiate(bossPrefab, new Vector3(posSum.x, posSum.y, 0), Quaternion.identity);

        box.transform.parent = this.gameObject.transform;
        box.name = $"{pos.x}, {pos.y}";
        SpriteRenderer sr = box.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = pos.y;
        }

        box.Initialize(pos, this, randNum);
        allBoxes[pos.x, pos.y] = box;
    }

}
