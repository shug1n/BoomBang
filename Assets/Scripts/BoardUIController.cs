using UnityEngine;
using TMPro;

public class BoardUIController : MonoBehaviour
{
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private TMP_Dropdown widthDropdown;
    [SerializeField] private TMP_Dropdown heightDropdown;

    private void Start()
    {
        widthDropdown.value = 2;
        heightDropdown.value = 2;
    }

    public void OnCreateButtonClicked()
    {
        int selectedWidth = widthDropdown.value + 2; // 0-8 → 2-10
        int selectedHeight = heightDropdown.value + 2;

        boardManager.CreateBoard(selectedWidth, selectedHeight);
    }
}