namespace WoMFramework.Game.Model
{
    using Mogwai;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class Stats
    {
        public Attribute LuckAttr = AttributeBuilder.Create("Luck")
            .Salted(true).SetPosition(42).SetSize(2).SetCreation(32).SetMinRange(1).SetMaxRange(256)
            .SetDescription("A measure of a character's luck. Luck might influence anything, but mostly random items, encounters and outstanding successes/failures (such as critical hits).").Build();

        public int Luck => LuckAttr.GetValue();

        public Attribute AlignmentAttr = AttributeBuilder.Create("Alignment")
            .Salted(true).SetPosition(44).SetSize(1).SetCreation(16).SetMaxRange(16)
            .SetDescription("A creature's general moral and personal attitudes are represented by its alignment: lawful good, neutral good, chaotic good, lawful neutral, neutral, chaotic neutral, lawful evil, neutral evil, or chaotic evil.").Build();

        public int Alignment => AlignmentAttr.GetValue();

        public List<Attribute> All => new List<Attribute> { LuckAttr, AlignmentAttr };

        public Stats(HexValue hexValue)
        {
            All.ForEach(p => p.CreateValue(hexValue));
        }

        public string MapAlignment()
        {
            switch (Alignment)
            {
                case 0:
                    return "Lawful Good";
                case 1:
                    return "Lawful Good";
                case 2:
                    return "Lawful Good";
                case 3:
                    return "Lawful Good";
                case 4:
                    return "Lawful Good";
                case 5:
                    return "Neutral Good";
                case 6:
                    return "Neutral Good";
                case 7:
                    return "Chaotic Good";
                case 8:
                    return "Lawful Neutral";
                case 9:
                    return "Lawful Neutral";
                case 10:
                    return "True Neutral";
                case 11:
                    return "True Neutral";
                case 12:
                    return "Chaotic Neutral";
                case 13:
                    return "Lawful Evil";
                case 14:
                    return "Neutral Evil";
                case 15:
                    return "Chaotic Evil";
                default:
                    return "Lawful Good";
            }
        }
    }
}
