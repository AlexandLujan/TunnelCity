using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class CardinalMovement : MonoBehaviour
{
    [Header("Grid Movement")]
    [SerializeField]
    private Grid worldGrid;

    [SerializeField]
    private float moveDuration = 0.15f;

    [Header("Held Input")]
    [SerializeField]
    private float initialBoostDelay = 0.20f;

    [SerializeField]
    private float boostDelay = 0.08f;

    private Rigidbody2D rb;

    private Vector3Int currentCell;
    private bool isMoving;
    private Vector2Int heldDirection;
    private float heldTimer;
    private bool hasBoosted;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (worldGrid == null) return;

        currentCell = worldGrid.WorldToCell(rb.position);
        rb.position = GetCellCenter(currentCell);
    }

    private void Update()
    {
        Vector2Int inputDirection = GetInputDirection();

        if (inputDirection == Vector2Int.zero)
        {
            heldDirection = Vector2Int.zero;
            heldTimer = 0f;
            hasBoosted = false;

            return;
        }

        if (inputDirection != heldDirection)
        {
            heldDirection = inputDirection;

            heldTimer = 0f;
            hasBoosted = false;

            TryMove(heldDirection);
            return;
        }
        heldTimer += Time.deltaTime;

        float requiredBoost = hasBoosted ? boostDelay : initialBoostDelay;

        if (heldTimer >= requiredBoost)
        {
            heldTimer = 0f;
            hasBoosted = true;

            TryMove(heldDirection);
        }
    }

    private Vector2Int GetInputDirection()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null) return Vector2Int.zero;

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) return Vector2Int.up;
        if (keyboard.aKey.isPressed || keyboard.downArrowKey.isPressed) return Vector2Int.left;
        if (keyboard.sKey.isPressed || keyboard.leftArrowKey.isPressed) return Vector2Int.down;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) return Vector2Int.right;

        return Vector2Int.zero;
    }

    private void TryMove(Vector2Int direction)
    {
        if (isMoving) return;

        Vector3Int targetCell = currentCell + new Vector3Int(direction.x, direction.y, 0);

        Debug.Log($"Moving from {currentCell} to {targetCell}");

        StartCoroutine(MoveToCell(targetCell));
    }

    private IEnumerator MoveToCell(Vector3Int targetCell)
    {
        isMoving = true;

        Vector3 startPos = rb.position;
        Vector3 targetPos = GetCellCenter(targetCell);

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / moveDuration);

            Vector2 nextPos = Vector2.Lerp(startPos, targetPos, t);

            rb.MovePosition(nextPos);
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPos);
        currentCell = targetCell;
        isMoving = false;
    }

    private Vector3 GetCellCenter(Vector3Int cell)
    {
        Vector3 center = worldGrid.GetCellCenterWorld(cell);
        return new Vector2(center.x, center.y);
    }
}
