namespace WoMFramework.Game.Model.Classes
{
    using Enums;
    using Learnable;
    using System.Collections.Generic;

    public class Ranger : Classes
    {

        public Ranger() : base(ClassType.Ranger, true)
        {
            HitPointDiceRollEvent = new[] { 1, 10 };
            WealthDiceRollEvent = new[] { 3, 6, 0, 1 };
            Description = "For those who relish the thrill of the hunt, there are only predators and prey. Be they scouts, trackers, or bounty hunters, rangers share much in common: unique mastery of specialized weapons, skill at stalking even the most elusive game, and the expertise to defeat a wide range of quarries. Knowledgeable, patient, and skilled hunters, these rangers hound man, beast, and monster alike, gaining insight into the way of the predator, skill in varied environments, and ever more lethal martial prowess. While some track man-eating creatures to protect the frontier, others pursue more cunning game—even fugitives among their own people.";
            Role = "Rangers are deft skirmishers, either in melee or at range, capable of skillfully dancing in and out of battle. Their abilities allow them to deal significant harm to specific types of foes, but their skills are valuable against all manner of enemies.";
            //Alignment: Any
            Learnables.AddRange(ClassSpells());
        }

        public override int CasterMod(Entity entity) => entity.WisdomMod;

        public override List<Spell> ClassSpells()
        {
            return new List<Spell>();
        }

        public override void ClassLevelUp()
        {
            base.ClassLevelUp();

            FortitudeBaseSave = (int)(2 + (double)ClassLevel / 2);
            ReflexBaseSave = (int)(2 + (double)ClassLevel / 2);
            WillBaseSave = (int)(0 + (double)ClassLevel / 3);

            ClassAttackBonus = ClassLevel;
        }
    }
}
