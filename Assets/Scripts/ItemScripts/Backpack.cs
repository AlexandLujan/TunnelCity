using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Backpack : MonoBehaviour
{
    [SerializeField] private int capacity;
    [SerializeField] private Image slotPrefab;      // drag the BackpackSlot prefab
    [SerializeField] private Transform slotParent;
    [SerializeField] private Item DebugItem;
    [SerializeField] private TMP_Text backpackText;   // drag the button used to remove items

    public List<Item> items = new List<Item>();
    private List<Image> slots = new List<Image>();
    private bool isVisible = false;

    private void Start()
    {
        backpackText.text = "Press a number button to remove an item";
        backpackText.enabled = isVisible;
        for (int i = 0; i < capacity; i++)
        {
            Image slot = Instantiate(slotPrefab, slotParent);
            slots.Add(slot);
        }
        refresh();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            GiveDebugItem();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            isVisible = !isVisible;
            refresh();
        }
        for (int i = 0; i < items.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && isVisible)
            {
                RemoveItem(i);
            }
        }

    }
    public bool AddItem(Item item)
    {
        if (items.Count >= capacity || item == null)
        {
            return false;
        }
        items.Add(item);
        refresh();
        return true;
    }
    private void RemoveItem(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            Item obj = items[index];
            items.RemoveAt(index);
            refresh();
        }
    }
    public void GiveDebugItem()
    {
        AddItem(DebugItem);
        refresh();
    }
    private void refresh()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < items.Count && items[i] != null && items[i].icon != null)
            {
                slots[i].sprite = items[i].icon;
                slots[i].enabled = isVisible;
            }
            else
            {
                slots[i].sprite = null;
                slots[i].enabled = false;
            }
        }
        backpackText.enabled = isVisible;
    }
}