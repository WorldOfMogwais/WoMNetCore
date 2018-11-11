using System.Collections.Generic;
using System.Linq;
using System.Text;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Model.Actions;

namespace WoMFramework.Game.Model
{

    public enum FeatType
    {
        General,
        Combat,
        ItemCreation,
        Metamagic,
        Monster,
        Grit,
        Achievement,
        Story,
        Mythic
    }



    /// <summary>
    /// Feat class
    /// </summary>
    public class Feat : Learnable
    {
        public int Id { get; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Benefit { get; set; }

        public FeatType FeatType { get; set; }

        public List<Requirement> Requirements { get; set; }

        public List<Modifier> Modifiers { get; set; }

        public List<CombatAction> CombatActions { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public Feat(int id, string name)
        {
            Id = id;
            Name = name;
            Requirements = new List<Requirement>();
            Modifiers = new List<Modifier>();
            IsActive = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"// {Name}[{Id}]\n" +
                   $"// {Benefit}\n" +
                   //$"{string.Join(',', Requirements)}\n" +
                   $"";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool CanLearn(Entity entity)
        {
            // can't learn skill 2x times
            if (entity.Feats.Any(p => p.Id == Id))
            {
                return false;
            }

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

            // add feat
            entity.Feats.Add(this);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool CanUnLearn(Entity entity)
        {
            // can't learn skill 2x times
            if (!entity.Feats.Exists(p => p.Id == Id))
            {
                return false;
            }

            // can't unlearn skills that have requirments for learned follow up skills
            if (entity.Feats.Exists(p => p.Requirements.Any(q => q.RequirementType == RequirementType.Skill && q.Value == Id)))
            {
                return false;
            }

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

            // remove skill
            entity.Feats.Remove(this);

            return true;
        }
    }

    public interface Learnable
    {
        bool CanLearn(Entity entity);

        bool Learn(Entity entity);

        bool CanUnLearn(Entity entity);

        bool UnLearn(Entity entity);
    }
}
