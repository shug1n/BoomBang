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
                if (clickedBox.IsMoving)
                {
                    return;
                }

                List<Box> matchingGroup = boardManager.FindMatches(clickedBox);

                // Kural: En az 2'li grup olmalý
                if (matchingGroup.Count >= 2)
                {
                    Debug.Log($"<color=green>PATLAMA!</color> {matchingGroup.Count} blok gidiyor.");

                    boardManager.ExplodeGroup(matchingGroup); // Eþleþen gruplarý patlatýyoruz
                }
                else
                {
                    Debug.Log("<color=red>Tek baþýna patlayamaz!</color>");
                }
            }
        }
    }
}