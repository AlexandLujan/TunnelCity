using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MiningSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private CaveRenderer caveRenderer;
    [SerializeField]
    private Camera worldCamera;

    private void Awake()
    {
        if (worldCamera == null) worldCamera = Camera.main;
    }

    private void Update()
    {
        HandleMiningInput();
    }

    private void HandleMiningInput()
    {
        if (caveRenderer == null || worldCamera == null) return;
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = worldCamera.ScreenToWorldPoint(mouseScreenPosition);
        Vector3Int cellPosition = caveRenderer.WorldToCell(mouseWorldPosition);

        caveRenderer.MineWall(cellPosition);

    }
}

