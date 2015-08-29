using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Scripts/Player/Inventory")]
public class Inventory : MonoBehaviour {

    public Vector2 iconDimensions;
    public float iconSpacing;
    public float itemCircleRadius;

    protected Dictionary<System.Type, List<Item>> items;
    protected Item equipped;
    protected System.Type[] slots;
    protected int selecting;
    protected int highlighted;
    protected List<System.Type> unselectedItems;
    protected Vector2 mousePos;

    void Start () {
        items = new Dictionary<System.Type, List<Item>>();
        slots = new System.Type[3];
        equipped = null;
        selecting = -1;
        unselectedItems = new List<System.Type>();
    }

    public void OnGUI() {
        if (selecting < 0)
            return;
        showSlottedItems();
        showUnequippedItems(selecting);
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

    public void useEquipped() {
        if (selecting >= 0) {
            slots[selecting] = unselectedItems[highlighted];
        }
        if (equipped == null)
            return;
        equipped.invoke(this);
    }

    public void doSelect(int slot) {
        if (selecting != slot) {
            selecting = slot;
            mousePos = Vector2.zero;
        }
        if (selecting < 0)
            return;
        unselectedItems.Clear();
        unselectedItems.AddRange(items.Keys);
        foreach(System.Type type in slots)
            if (type != null)
                unselectedItems.Remove(type);
        highlighted = getCircleSelection(unselectedItems.Count, mousePos);
    }

    public void moveMouse(Vector2 amount) {
        mousePos += amount;
    }

    public bool isSelecting() {
        return selecting >= 0;
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

    protected void showUnequippedItems(int slot) {
        Vector2 center = (new Vector2(Screen.width, Screen.height) - iconDimensions) /2 + new Vector2(-mousePos.x, mousePos.y) *0.1f;
        double angle = 0.5 *Mathf.PI;
        double angleDiff = 2.0 *Mathf.PI / unselectedItems.Count;
        int i=0;
        GUI.Label(new Rect(10, 10, 100, 100), highlighted.ToString());
        foreach(System.Type itemType in unselectedItems) {
            Vector2 offset = new Vector2(Mathf.Cos((float)angle), Mathf.Sin((float)angle)) *itemCircleRadius;
            float size = i==highlighted? 2: 1;
            Rect pos = new Rect(center.x +offset.x, center.y +offset.y, iconDimensions.x *size, iconDimensions.y *size);
            GUI.DrawTexture(pos, getItem(itemType).icon, ScaleMode.ScaleToFit);
            angle += angleDiff;
            ++i;
        }
        if (slots[slot] != null) {
            Rect pos = new Rect(center.x, center.y, iconDimensions.x, iconDimensions.y);
            GUI.DrawTexture(pos, getItem(slots[slot]).icon, ScaleMode.ScaleToFit);
        }
    }

    protected int getCircleSelection(int itemCount, Vector2 mousePosition) {
        if (mousePosition.sqrMagnitude < (itemCircleRadius *itemCircleRadius) /4)
            return -1;
        double angle = Mathf.Atan2(mousePosition.x, mousePosition.y) +Mathf.PI;
        return ((int)(angle /(2.0*Mathf.PI /itemCount) +0.5)) %itemCount;
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
        List<Item> list;
        if (items.TryGetValue(type, out list))
            return list;
        return new List<Item>();
    }
}
