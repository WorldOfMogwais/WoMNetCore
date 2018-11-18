using GoRogue;
using WoMFramework.Game.Generator.Dungeon;
using WoMFramework.Game.Model.Actions;

namespace WoMFramework.Game.Generator
{
    public interface IAdventureEntity
    {
        Adventure Adventure { get; set; }

        Map Map { get; set; }

        Coord Coordinate { get; set; }

        bool IsStatic { get; }

        bool IsPassable { get; }

        int AdventureEntityId { get; set; }

        int Size { get; }

        bool TakeAction(EntityAction entityAction);
    }
}