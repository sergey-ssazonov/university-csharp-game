namespace HardwareKiller.Models
{
    public enum ItemType { File, Folder, Custom }
    public class Item : Entity
    {
        public ItemType Type { get; }
        public Item(int x, int y, ItemType type) : base(x, y)
            => Type = type;
    }
}