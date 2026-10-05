using UnityEngine;

[CreateAssetMenu(fileName = "pickaxe", menuName = "Pickaxe")]
public class Pickaxe : Item
{
    [Header("Unique Stats")]
    public int durability = 50;
    private Vector2Int miningDirection;

    public override void use(GameObject target, CaveRenderer cave, Vector3 pos)
    {
        CardinalMovement movement = target.GetComponentInChildren<CardinalMovement>();
        miningDirection = movement.GetLastInput();

        Debug.Log($"Name: {itemName}\n Durability: {durability}\n Last Input: {miningDirection}");

        Vector3Int cell = cave.WorldToCell(pos);
        int x = Mathf.RoundToInt(pos.x);
        int y = Mathf.RoundToInt(pos.y);
        cave.MineWall(cell.x + miningDirection.x, cell.y + miningDirection.y);
    }
}
