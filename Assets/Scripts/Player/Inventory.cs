using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {

    public Vector2 iconDimensions;
    public float iconSpacing;
    public float itemCircleRadius;

    protected Dictionary<System.Type, List<Item>> items;
    protected Item equipped;
    protected System.Type[] slots;
    protected bool selecting;

    void Start () {
        items = new Dictionary<System.Type, List<Item>>();
        slots = new System.Type[3];
        equipped = null;
        selecting = false;
    }

    public void OnGUI() {
        if (!selecting)
            return;
        showSlottedItems();
        showAllItems();
    }

    public bool take(GameObject itemObject) {
        Item item = itemObject.GetComponent<Item>();
		if (item == null)
            return false;
        return take(item);
    }

    public bool take(Item item) {
		addItem(item);
        item.onTake(this);
        return true;
    }

    public void drop(Item item) {
        removeItem(item);
        item.onDrop(this);
    }

    public void equip(int slot) {
        Item newEquiped = getItem(slots[slot]);
        if (newEquiped.GetType() == equipped.GetType()) {
            unequip();
            return;
        }
        equip(newEquiped);
    }

    public void equip(Item item) {
        unequip();
        equipped = item;
        equipped.onEquip(this);
    }

    public void unequip() {
        if (equipped == null)
            return;
        equipped.onUnequip(this);
        equipped = null;
    }

    protected void showSlottedItems() {
        float offset = (Screen.width / 2f) - (iconDimensions.x * 1.5f + iconSpacing);
        for (int i = 0; i < slots.Length; ++i) {
            offset += iconDimensions.x + iconSpacing;
            if (slots[i] != null) {
                Item item = getItem(slots[i]);
                if (item.icon != null) {
                    Rect pos = new Rect(offset, 0, iconDimensions.x, iconDimensions.y);
                    GUI.DrawTexture(pos, item.icon, ScaleMode.ScaleToFit);
                }
            }
        }
    }

    protected void showAllItems() {
        Vector2 center = new Vector2(Screen.width, Screen.height) - iconDimensions /2;
        double angle = 0;
        double angleDiff = 2.0 *Mathf.PI / items.Count;
        foreach(System.Type itemType in items.Keys) {
            Vector2 offset = new Vector2(Mathf.Cos((float)angle), Mathf.Sin((float)angle)) *itemCircleRadius;
            Rect pos = new Rect(center.x +offset.x, center.y +offset.y, iconDimensions.x, iconDimensions.y);
            GUI.DrawTexture(pos, getItem(itemType).icon, ScaleMode.ScaleToFit);
            angle += angleDiff;
        }
    }

    protected Item getItem(System.Type type) {
        List<Item> list = items[type];
        if (list == null)
            return null;
        return list[0];
    }

    protected void addItem(Item item) {
        System.Type type = item.GetType();
        List<Item> list = getItemList(type);
        list.Add(item);
        items[type] = list;
    }

    protected Item removeItem(System.Type type) {
        List<Item> list = getItemList(type);
        if (list.Count > 1) {
            Item item = list[list.Count];
            list.RemoveAt(list.Count);
            items[type] = list;
            return item;
        }
        items[type] = null;
        return (list.Count == 1) ? list[0] : null;
    }

    protected bool removeItem(Item item) {
        System.Type type = item.GetType();
        List<Item> list = getItemList(type);
        bool removed = list.Remove(item);
        if (list.Count < 1)
            items[type] = null;
        else
            items[type] = list;
        return removed;
    }

    protected List<Item> getItemList(System.Type type) {
        List<Item> list = items[type];
        if (list == null)
            return new List<Item>();
        return list;
    }
}
