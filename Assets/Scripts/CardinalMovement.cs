using UnityEngine;
using System.Collections;

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

    private Vector2Int inputDirection;
    public Vector2Int lastInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (worldGrid == null)
            return;

        currentCell = worldGrid.WorldToCell(rb.position);
        rb.position = GetCellCenter(currentCell);
    }

    private void Update()
    {
        inputDirection = GetInputDirection();

        if (inputDirection == Vector2Int.zero)
        {
            heldDirection = Vector2Int.zero;
            heldTimer = 0f;
            hasBoosted = false;

            return;
        }

        if (inputDirection != Vector2Int.zero &&
            inputDirection != lastInput)
        {
            lastInput = inputDirection;
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

        float requiredBoost =
            hasBoosted
                ? boostDelay
                : initialBoostDelay;

        if (heldTimer >= requiredBoost)
        {
            heldTimer = 0f;
            hasBoosted = true;

            TryMove(heldDirection);
        }
    }

    public void SetWorldGrid(Grid grid)
    {
        worldGrid = grid;

        if (worldGrid == null)
            return;

        if (rb == null)
            return;

        currentCell =
            worldGrid.WorldToCell(
                rb.position
            );

        Debug.Log(
            $"WORLD GRID SET | " +
            $"Grid: {worldGrid.name} | " +
            $"Grid World Pos: {worldGrid.transform.position} | " +
            $"Player Pos: {rb.position} | " +
            $"Calculated Cell: {currentCell}"
        );
    }

    private Vector2Int GetInputDirection()
    {
        if (Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.UpArrow))
        {
            return Vector2Int.up;
        }

        if (Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.LeftArrow))
        {
            return Vector2Int.left;
        }

        if (Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.DownArrow))
        {
            return Vector2Int.down;
        }

        if (Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.RightArrow))
        {
            return Vector2Int.right;
        }

        return Vector2Int.zero;
    }

    private void TryMove(Vector2Int direction)
    {
        if (isMoving)
            return;

        Vector3Int targetCell =
            currentCell +
            new Vector3Int(
                direction.x,
                direction.y,
                0
            );

        Vector3 targetWorld =
            GetCellCenter(targetCell);

        Debug.Log(
            $"MOVE | " +
            $"Player: {rb.position} | " +
            $"Current Cell: {currentCell} | " +
            $"Target Cell: {targetCell} | " +
            $"Target World: {targetWorld} | " +
            $"Grid: {worldGrid?.name} | " +
            $"Grid Pos: {worldGrid?.transform.position}"
        );

        StartCoroutine(
            MoveToCell(targetCell)
        );
    }

    private IEnumerator MoveToCell(
        Vector3Int targetCell)
    {
        isMoving = true;

        Vector3 startPos = rb.position;
        Vector3 targetPos =
            GetCellCenter(targetCell);

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsedTime / moveDuration
                );

            Vector2 nextPos =
                Vector2.Lerp(
                    startPos,
                    targetPos,
                    t
                );

            rb.MovePosition(nextPos);

            yield return
                new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPos);

        currentCell = targetCell;
        isMoving = false;
    }

    private Vector3 GetCellCenter(
        Vector3Int cell)
    {
        Vector3 center =
            worldGrid.GetCellCenterWorld(cell);

        return new Vector2(
            center.x,
            center.y
        );
    }

    public Vector2Int GetLastInput()
    {
        return lastInput;
    }
}