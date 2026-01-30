using UnityEngine;
using TMPro;

public class BoardUIController : MonoBehaviour
{
    [SerializeField]
    private BoardManager boardManager;

    [SerializeField] 
    private TMP_Dropdown widthDropdown;
    
    [SerializeField]
    private TMP_Dropdown heightDropdown;

    [SerializeField]
    private TMP_Dropdown colorCountDropdown;

    [SerializeField] 
    private TMP_Dropdown inputThresholdA;

    [SerializeField]
    private TMP_Dropdown inputThresholdB;

    [SerializeField] 
    private TMP_Dropdown inputThresholdC;

    private void Start()
    {
        widthDropdown.value = 2;
        heightDropdown.value = 2;
        colorCountDropdown.value = 5;
        inputThresholdA.value = 3;
        inputThresholdB.value = 4;
        inputThresholdC.value = 5;
    }

    public void OnCreateButtonClicked()
    {
        int selectedWidth = widthDropdown.value + 2; // 0-8 → 2-10
        int selectedHeight = heightDropdown.value + 2;
        int selectedColorCount = colorCountDropdown.value + 1;
        int tA = inputThresholdA.value + 1;
        int tB = inputThresholdB.value + 2;
        int tC = inputThresholdC.value + 3;


        boardManager.CreateBoard(selectedWidth, selectedHeight, selectedColorCount, tA, tB, tC);
    }
}