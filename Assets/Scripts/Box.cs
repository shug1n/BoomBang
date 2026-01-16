using UnityEngine;

public class Box : MonoBehaviour
{
    [HideInInspector]
    public Vector2 posIndex;
    [HideInInspector]
    public BoardManager board;
    [HideInInspector]
    public int typeID;

    public void Initialize(Vector2 pos, BoardManager boardOrigin, int randNum)
    {
        posIndex = pos;
        board = boardOrigin;

        typeID = randNum;
    }

}