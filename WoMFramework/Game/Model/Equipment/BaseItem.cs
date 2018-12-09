using System;
using System.Collections.Generic;
using System.Linq;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Model.Actions;

namespace WoMFramework.Game.Model
{
    public enum RarityType
    {
        Normal, Magic, Rare, Legendary, Artifact
    }

    public class BaseItem : Learnable
    {
        public SlotType SlotType { get; }

        public RarityType RarityType { get; }

        public string Name { get; }

        public double Cost { get; }

        public double Weight { get; }

        public string Description { get; set; }

        public List<Requirement> Requirements { get; set; }

        public List<Modifier> Modifiers { get; set; }

        public List<CombatAction> CombatActions { get; set; }

        public Entity Owner { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cost"></param>
        /// <param name="weight"></param>
        /// <param name="description"></param>
        /// <param name="rarityType"></param>
        /// <param name="slotType"></param>
        public BaseItem(string name, double cost, double weight, string description, RarityType rarityType = RarityType.Normal, SlotType slotType = SlotType.None)
        {
            SlotType = slotType;
            RarityType = rarityType;

            Name = name;
            Cost = cost;
            Weight = weight;
            Description = description;

            Requirements = new List<Requirement>();
            Modifiers = new List<Modifier>();
            CombatActions = new List<CombatAction>();

            Owner = null;
        }

        public override string ToString()
        {
            return Name;
        }

        public void ChangeOwner(Entity owner)
        {
            Owner = owner;

            // change owner ship
            foreach (var combatAction in CombatActions)
            {
                combatAction.ChangeOwner(owner);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool CanLearn(Entity entity)
        {
            return Requirements.All(p => p.Valid(entity));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Learn(Entity entity)
        {
            if (!CanLearn(entity))
            {
                return false;
            }

            // add modifiers
            Modifiers.ForEach(p => p.AddMod(entity));

            // add actions
            CombatActions.ForEach(p => entity.CombatActions.Add(p));

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool CanUnLearn(Entity entity)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool UnLearn(Entity entity)
        {
            if (!CanUnLearn(entity))
            {
                return false;
            }

            // remove modifiers
            Modifiers.ForEach(p => p.RemoveMod(entity));

            // remove actions
            CombatActions.ForEach(p => entity.CombatActions.Remove(p));

            return true;
        }
    }

}
