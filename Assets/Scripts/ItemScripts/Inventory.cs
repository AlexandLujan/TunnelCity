using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public Item item1;
    public Item item2;
    [SerializeField] private CaveRenderer cave;
    [SerializeField] private Image item1Slot;
    [SerializeField] private Image item2Slot;
    [SerializeField] private Item debugItem;

    private void Start()
    {
        refresh();
    }
    private void Update()
    {
        // Press number key '1' on your keyboard to use Item 1
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseItem(item1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            UseItem(item2);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            GiveItem(debugItem);
        }

    }

    public void UseItem(Item item)
    {
        if(item != null) { item.use(gameObject, cave, transform.position); }
    }

    public void refresh()
    {
        UpdateSlot(item1Slot, item1);
        UpdateSlot(item2Slot,item2);
    }
    public void UpdateSlot(Image slot, Item item)
    {
        if(slot == null) { return; }

        bool hasIcon = item != null && item.icon != null;
        slot.sprite = hasIcon ? item.icon : null;
        slot.enabled = hasIcon;
    }
    public void GiveItem(Item item)
    {
        if (item1 == null)
        {
            item1 = item;
        }
        else if (item2 == null)
        {
            item2 = item;
        }
        refresh();
    }
}
