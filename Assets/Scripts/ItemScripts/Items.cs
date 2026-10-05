using UnityEngine;

public abstract class Item : ScriptableObject
{
    [Header("Base Stats")]
    public string itemName;
    public Sprite icon;
    public bool isBroken;
    public abstract void use(GameObject target,CaveRenderer cave, Vector3 pos);
}



