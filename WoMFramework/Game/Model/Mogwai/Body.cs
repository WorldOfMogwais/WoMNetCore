namespace WoMFramework.Game.Model.Mogwai
{
    using System.Collections.Generic;

    public class Body
    {
        internal Attribute EarAttr = AttributeBuilder.Create("Ear")
                .Salted(true).SetPosition(2).SetSize(2).SetCreation(16).SetMaxRange(16).Build();

        public int Ear => EarAttr.GetValue();

        internal Attribute MouthAttr = AttributeBuilder.Create("Mouth")
                .Salted(true).SetPosition(4).SetSize(2).SetCreation(16).SetMaxRange(16).Build();

        public int Mouth => MouthAttr.GetValue();

        internal Attribute SkinColorAttr = AttributeBuilder.Create("SkinColor")
                .Salted(true).SetPosition(6).SetSize(2).SetCreation(32).SetMaxRange(64).Build();

        public int SkinColor => SkinColorAttr.GetValue();

        internal Attribute EyeTypeAttr = AttributeBuilder.Create("EyeType")
                .Salted(true).SetPosition(8).SetSize(2).SetCreation(16).SetMaxRange(16).Build();

        public int EyeType => EyeTypeAttr.GetValue();

        internal Attribute EyeColorAttr = AttributeBuilder.Create("EyeColor")
                .Salted(true).SetPosition(10).SetSize(2).SetCreation(128).SetMaxRange(256).Build();

        public int EyeColor => EyeColorAttr.GetValue();

        public List<Attribute> All => new List<Attribute> { EarAttr, MouthAttr, SkinColorAttr, EyeTypeAttr, EyeColorAttr };

        public Body(HexValue hexValue)
        {
            All.ForEach(p => p.CreateValue(hexValue));
        }
    }
}
