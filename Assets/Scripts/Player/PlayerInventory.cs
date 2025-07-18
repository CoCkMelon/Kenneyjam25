using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int maxSlots = 20;
    [SerializeField] private int currency = 0;
    
    // Inventory storage
    private Dictionary<string, int> items = new Dictionary<string, int>();
    
    // Events
    public System.Action<string, int> OnItemAdded;
    public System.Action<string, int> OnItemRemoved;
    public System.Action<int> OnCurrencyChanged;
    
    void Start()
    {
        InitializeInventory();
    }
    
    void InitializeInventory()
    {
        // Initialize inventory with default values
        Debug.Log("Inventory initialized");
    }
    
    public void AddItem(string itemName, int quantity = 1)
    {
        if (items.ContainsKey(itemName))
        {
            items[itemName] += quantity;
        }
        else
        {
            items[itemName] = quantity;
        }
        
        OnItemAdded?.Invoke(itemName, quantity);
        Debug.Log($"Added {quantity} {itemName} to inventory");
    }
    
    public bool RemoveItem(string itemName, int quantity = 1)
    {
        if (!items.ContainsKey(itemName) || items[itemName] < quantity)
        {
            return false;
        }
        
        items[itemName] -= quantity;
        if (items[itemName] <= 0)
        {
            items.Remove(itemName);
        }
        
        OnItemRemoved?.Invoke(itemName, quantity);
        Debug.Log($"Removed {quantity} {itemName} from inventory");
        return true;
    }
    
    public int GetItemCount(string itemName)
    {
        return items.ContainsKey(itemName) ? items[itemName] : 0;
    }
    
    public bool HasItem(string itemName, int quantity = 1)
    {
        return GetItemCount(itemName) >= quantity;
    }
    
    public void AddCurrency(int amount)
    {
        currency += amount;
        OnCurrencyChanged?.Invoke(currency);
        Debug.Log($"Added {amount} currency. Total: {currency}");
    }
    
    public bool SpendCurrency(int amount)
    {
        if (currency >= amount)
        {
            currency -= amount;
            OnCurrencyChanged?.Invoke(currency);
            Debug.Log($"Spent {amount} currency. Remaining: {currency}");
            return true;
        }
        return false;
    }
    
    public int GetCurrency()
    {
        return currency;
    }
    
    // Get all items in inventory
    public Dictionary<string, int> GetAllItems()
    {
        return new Dictionary<string, int>(items);
    }
    
    // Method to set currency (for save/load system)
    public void SetCurrency(int amount)
    {
        currency = amount;
        OnCurrencyChanged?.Invoke(currency);
    }
    
    // Method to get inventory items as List<string> (for save/load system)
    public List<string> GetInventoryItems()
    {
        List<string> itemsList = new List<string>();
        foreach (var item in items)
        {
            // Add each item multiple times based on quantity
            for (int i = 0; i < item.Value; i++)
            {
                itemsList.Add(item.Key);
            }
        }
        return itemsList;
    }
    
    // Method to set inventory items from List<string> (for save/load system)
    public void SetInventoryItems(List<string> itemsList)
    {
        items.Clear();
        
        foreach (string itemName in itemsList)
        {
            if (items.ContainsKey(itemName))
            {
                items[itemName]++;
            }
            else
            {
                items[itemName] = 1;
            }
        }
        
        // Notify about all items being added
        foreach (var item in items)
        {
            OnItemAdded?.Invoke(item.Key, item.Value);
        }
    }
    
    // Method to load inventory from save data (items only)
    public void LoadInventory(List<string> itemsList)
    {
        // Clear current inventory
        items.Clear();
        
        // Load items
        foreach (string itemName in itemsList)
        {
            if (items.ContainsKey(itemName))
            {
                items[itemName]++;
            }
            else
            {
                items[itemName] = 1;
            }
        }
        
        // Trigger events for items
        foreach (var item in items)
        {
            OnItemAdded?.Invoke(item.Key, item.Value);
        }
        
        Debug.Log($"Loaded inventory with {items.Count} unique items");
    }
    
    // Method to load inventory from save data (items and currency)
    public void LoadInventory(List<string> itemsList, int currencyAmount)
    {
        // Load items first
        LoadInventory(itemsList);
        
        // Set currency
        currency = currencyAmount;
        
        // Trigger currency event
        OnCurrencyChanged?.Invoke(currency);
        
        Debug.Log($"Loaded inventory with {items.Count} unique items and {currency} currency");
    }
}
