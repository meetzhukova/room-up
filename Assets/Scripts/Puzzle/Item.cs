public class Item
{
    private ItemType type;

    public Item(ItemType type)
    {
        this.type = type;
    }

    public ItemType GetItemType()
    {
        return type;
    }

    public bool IsSameType(Item other)
    {
        if (other == null)
        {
            return false;
        }

        return type == other.GetItemType();
    }
}