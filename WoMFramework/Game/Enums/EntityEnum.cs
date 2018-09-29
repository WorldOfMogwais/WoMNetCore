namespace WoMFramework.Game.Enums
{
    public enum GenderType
    {
        Male,
        Female
    }

    public enum SizeType
    {
        Colossal = -8,
        Gargantuan = -4,
        Huge = -2,
        Large = -1,
        Medium = 0,
        Small = 1,
        Tiny = 2,
        Diminutive = 4,
        Fine = 8
    }

    public enum ClassType
    {
        None,
        Barbarian,
        Bard,
        Cleric,
        Druid,
        Fighter,
        Monk,
        Paladin,
        Ranger,
        Rogue,
        Sorcerer,
        Wizard
    }

    public enum AlignmentType
    {
        LawfulGood,
        NeutralGood,
        ChaoticGood,
        LawfulNeutral,
        TrueNeutral,
        ChaoticNeutral,
        LawfulEvil,
        NeutralEvil,
        ChaoticEvil
    }

    public enum SkillType
    {
        Acrobatics,
        Appraise,
        Bluff,
        Balance,
        Craft,
        Climb,
        Diplomacy,
        DisableDevice,
        Disguise,
        EscapeArtist,
        Fly,
        HandleAnimal,
        Intimidate,
        Heal,
        Knowledge,
        Linguistics,
        Perception,
        Perform,
        Profession,
        SenseMotive,
        Ride,
        Spellcraft,
        Stealth,
        Survival,
        Swim,
        UseMagicDevice,
        SleightOfHand
    }

    public enum SkillSubType
    {
        Arcana,
        Dungeoneering,
        Engineering,
        Geography,
        History,
        Local,
        Nature,
        Nobility,
        Planes,
        Religion,
        Traps,
        Dance,
        Alchemy,
        Act,
        Farmer,
        Stonemasonry,
        Blacksmithing,
        Comedy,
        Oratory,
        Sailor,
        Sing,
        Weaponsmithing,
        String,
        Sculpture,
        Wind,
        Jewelry,
        Any,
        AnyOne,
        AnyTwo,
        AnyThree,
        Miner,
        Cloth,
        Metalworking,
        Scribe,
        Percussion,
        Soothsayer,
        Carpentry,
        Bows,
        Armor,
        Cook,
        CrystalCarving,
        Fisherman,
        Gemcutting,
        Herbalism,
        Leather,
        Librarian,
        Limericks,
        Miller,
        Merchant,
        Poisonmaking,
        Scientist,
        Soldier,
        MusicalInstruments,
        Storytelling,
        Song,
        Wood,
        Peddler,
        Weaving,
        Rope
    }

    public enum EvolutionPattern
    {
        None
    }

    public enum HealthState
    {
        Healthy = 1,
        Injured = 0,
        Disabled = -1,
        Dying = -2,
        Dead = -3
    }

    public enum LanguageType
    {
        Common,
        Draconic,
        Dwarven,
        Elven,
        Giant,
        Gnome,
        Halfling,
        None,
        Abyssal,
        Aquan,
        Sylvan,
        Infernal,
        Terran,
        Auran,
        Aklo,
        Ignan,
        Goblin,
        Other
    }
}
