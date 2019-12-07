using System.Collections.Generic;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Model.Classes;

namespace WoMFramework.Game.Model.Monster
{
    public class NewMonsters
    {

        public static Monster BunnyRat => new MonsterBuilder()
        {
            Name = "Bunny Rat",
            ChallengeRating = 0.25,
            MonsterType = MonsterType.Animal,
            //MonsterSubType = MonsterSubType.None,
            Experience = 100,
            SizeType = SizeType.Tiny,
            Strength = 2,
            Dexterity = 15,
            Constitution = 11,
            Intelligence = 2,
            Wisdom = 13,
            Charisma = 2,
            Fortitude = 2,
            Reflex = 4,
            Will = 1,
            BaseSpeed = 15,
            NaturalArmor = 0,
            BaseAttackBonus = new int[] { 0 },
            HitPointDiceRollEvent = new int[] { 1, 8, 0, 0 },
            WeaponSlots = new List<WeaponSlot>() {
                new WeaponSlot()
                {
                    PrimaryWeapon = NaturalWeapon.Bite(SizeType.Tiny)
                }
            },
            TreasureType = TreasureType.None,
            EnvironmentTypes = new EnvironmentType[] { EnvironmentType.Any },
            Description = "Fecund and secretive, bunny rats are omnivorous rodents that particularly thrive in urban areas."
        }.Build();

        public static Monster CrystalGuardian => new MonsterBuilder()
        {
            Name = "Crystal Guardian",
            ChallengeRating = 1.00,
            MonsterType = MonsterType.Elemental,
            //MonsterSubType = MonsterSubType.Earth,
            Experience = 400,
            SizeType = SizeType.Large,
            Strength = 19,
            Dexterity = 12,
            Constitution = 17,
            Intelligence = 6,
            Wisdom = 13,
            Charisma = 14,
            Fortitude = 5,
            Reflex = 1,
            Will = 1,
            BaseSpeed = 30,
            NaturalArmor = 4,
            BaseAttackBonus = new int[] { 1 },
            HitPointDiceRollEvent = new int[] { 2, 8, 0, 6 },
            WeaponSlots = new List<WeaponSlot>() {
                new WeaponSlot()
                {
                    PrimaryWeapon = NaturalWeapon.Slam(SizeType.Large)
                }
            },
            TreasureType = TreasureType.Incidental,
            EnvironmentTypes = new EnvironmentType[] { EnvironmentType.Any },
            Description = "An animated cluster of translucent crystals shaped disturbingly like a gemstone Humanoid."
        }.Build();

        //    public static Monster Rat =>
        //        MonsterBuilder.Create("Rat", 0.25, MonsterType.Animal, 100)
        //         .SetSizeType(SizeType.Tiny)
        //         .SetAbilities(2, 15, 11, 2, 13, 2)
        //         .SetBaseSpeed(15)
        //         .SetHitPointDiceRollEvent(new[] { 1, 8 })
        //         .SetBaseAttackBonus(0)
        //         .SetWeaponSlot(NaturalWeapon.Bite(SizeType.Tiny))
        //         .SetDescription("Fecund and secretive, rats are omnivorous rodents that particularly thrive in urban areas.")
        //         .Build();

        //    public static Monster DireRat =>
        //        MonsterBuilder.Create("Dire Rat", 0.33, MonsterType.Animal, 135)
        //            .SetSizeType(SizeType.Small)
        //            .SetAbilities(10, 17, 13, 2, 13, 4)
        //            .SetBaseSpeed(40)
        //            .SetHitPointDiceRollEvent(new[] { 1, 8 , 0, 1 })
        //            .SetBaseAttackBonus(0)
        //            .SetWeaponSlot(NaturalWeapon.Bite(SizeType.Small))
        //            .SetDescription("This filthy rat is the size of a small dog. It has a coat of coarse fur, a long and scabby tail, and two glittering eyes.")
        //            .Build();

        //    public static Monster Wolf =>
        //        MonsterBuilder.Create("Wolf", 1, MonsterType.Animal, 400)
        //        .SetSizeType(SizeType.Medium)
        //        .SetAbilities(13, 15, 15, 2, 12, 6)
        //        .SetBaseSpeed(50)
        //        .SetNaturalArmor(2)
        //        .SetHitPointDiceRollEvent(new[] { 2, 8, 0, 4 })
        //        .SetBaseAttackBonus(1)
        //        .SetWeaponSlot(NaturalWeapon.Bite(SizeType.Medium))
        //        .SetDescription("Wandering alone or in packs, wolves sit at the top of the food chain. Ferociously " +
        //            "territorial and exceptionally wide-ranging in their hunting, wolf packs cover broad " +
        //            "areas. A wolf’s wide paws contain slight webbing between the toes that assists in " +
        //            "moving over snow, and its fur is a thick, water-resistant coat ranging in color from " +
        //            "gray to brown and even black in some species. Its paws contain scent glands that mark " +
        //            "the ground as it travels, assisting in navigation as well as broadcasting its whereabouts " +
        //            "to fellow pack members. Generally, a wolf stands from 2-1/2 to 3 feet tall at the shoulder " +
        //            "and weighs between 45 and 150 pounds, with females being slightly smaller.")
        //        .Build();
    }
}
