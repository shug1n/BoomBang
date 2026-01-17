using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private GameInput gameInput;
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private BoardManager boardManager;

    private void Awake()
    {
        gameInput = new GameInput();
    }

    private void OnEnable()
    {
        gameInput.Enable();
        gameInput.Gameplay.Touch.performed += OnTouch;
    }

    private void OnDisable()
    {
        gameInput.Disable();
        gameInput.Gameplay.Touch.performed -= OnTouch;
    }

    private void OnTouch(InputAction.CallbackContext context)
    {
        Vector2 screenPos = gameInput.Gameplay.Position.ReadValue<Vector2>();
        Vector2 worldPos = mainCamera.ScreenToWorldPoint(screenPos);

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        if (hit.collider != null)
        {
            Box clickedBox = hit.collider.GetComponent<Box>();
            if (clickedBox != null)
            {
                Debug.Log($"Týklanan Kutu: {clickedBox.name} - Renk ID: {clickedBox.itemStyle.colorID}");

                if (clickedBox != null)
                {
                    List<Box> matchingGroup = boardManager.FindMatches(clickedBox);

                    if (matchingGroup.Count >= 2)
                    {
                        Debug.Log($"<color=green>PATLAMA!</color> Toplam {matchingGroup.Count} adet {clickedBox.itemStyle.colorID} ID'li blok bulundu.");

                        for (int i = 0; i < matchingGroup.Count; i++)
                        {
                            // Debug için patlayanlarýn ismini yaz
                            matchingGroup[i].gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        Debug.Log("<color=red>Yetersiz Sayý!</color> Tek baþýna patlayamaz.");
                    }
                }
            }
        }
    }
}