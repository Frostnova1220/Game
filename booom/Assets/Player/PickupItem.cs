using UnityEngine;

public class PickupItem : MonoBehaviour, IPickupable
{
    public enum ItemType { Grenade, Wormhole, Decoy }
    public ItemType itemType;

    public string GetPickupName()
    {
        switch (itemType)
        {
            case ItemType.Grenade: return "¡ÒµØ";
            case ItemType.Wormhole: return "≥Ê∂¥¥©‘Ω∆˜";
            case ItemType.Decoy: return "µ»…Ì»À≈º";
            default: return "???";
        }
    }

    public int GetMaxUses()
    {
        return 3;
    }

    public void OnPickup(GameObject player)
    {
        ItemManager manager = FindObjectOfType<ItemManager>();
        if (manager != null)
            manager.AddItem(itemType);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("¥•∑¢ºÏ≤‚£∫" + other.name);
        if (other.CompareTag("Player"))
        {
            OnPickup(other.gameObject);
            Destroy(gameObject);
        }
    }
}
