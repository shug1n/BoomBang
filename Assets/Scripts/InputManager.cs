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
                Debug.Log($"Týklanan Kutu: {clickedBox.name} - Renk ID: {clickedBox.typeID}");

                // BURADA BoardManager'daki patlatma fonksiyonunu çaðýracaðýz.
                // boardManager.ExplodeGroup(clickedBox); 
            }
        }
    }
}