using System.Collections.Generic;

namespace WoMFramework.Game.Model
{
    public class Body
    {
        internal Attribute EarAttr = AttributBuilder.Create("Ear")
                .Salted(true).Position(2).Size(2).Creation(16).MaxRange(16).Build();
        public int Ear => EarAttr.GetValue();

        internal Attribute MouthAttr = AttributBuilder.Create("Mouth")
                .Salted(true).Position(4).Size(2).Creation(16).MaxRange(16).Build();
        public int Mouth => MouthAttr.GetValue();

        internal Attribute SkinColorAttr = AttributBuilder.Create("SkinColor")
                .Salted(true).Position(6).Size(2).Creation(32).MaxRange(64).Build();
        public int SkinColor => SkinColorAttr.GetValue();

        internal Attribute EyeTypeAttr = AttributBuilder.Create("EyeType")
                .Salted(true).Position(8).Size(2).Creation(16).MaxRange(16).Build();
        public int EyeType => EyeTypeAttr.GetValue();

        internal Attribute EyeColorAttr = AttributBuilder.Create("EyeColor")
                .Salted(true).Position(10).Size(2).Creation(128).MaxRange(256).Build();
        public int EyeColor => EyeColorAttr.GetValue();

        public List<Attribute> All => new List<Attribute>() { EarAttr, MouthAttr, SkinColorAttr, EyeTypeAttr, EyeColorAttr };

        public Body(HexValue hexValue)
        {
            All.ForEach(p => p.CreateValue(hexValue));
        }

    }

}