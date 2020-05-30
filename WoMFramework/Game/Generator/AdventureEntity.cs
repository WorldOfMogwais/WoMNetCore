namespace WoMFramework.Game.Generator
{
    using Dungeon;
    using Enums;
    using GoRogue;
    using Model;
    using Model.Actions;

    public abstract class AdventureEntity : SpellEnabled
    {
        protected AdventureEntity(bool isStatic, bool isPassable, int size, bool isLootable)
        {
            IsStatic = isStatic;
            IsPassable = isPassable;
            Size = size;
            IsLootable = isLootable;
        }

        public string Name { get; set; }

        public Adventure Adventure { get; set; }

        public Map Map { get; set; }

        public Coord Coordinate { get; set; }

        public bool IsStatic { get; }

        public bool IsPassable { get; set; }

        public int AdventureEntityId { get; set; }

        public int Size { get; }

        public bool HasLoot => IsLootable && LootState != LootState.None && LootState != LootState.Looted ? true : false;

        public bool IsLootable { get; }

        public LootState LootState { get; set; }

        public Treasure Treasure { get; set; }

        public virtual bool TakeAction(EntityAction entityAction)
        {
            return false;
        }
    }
}
