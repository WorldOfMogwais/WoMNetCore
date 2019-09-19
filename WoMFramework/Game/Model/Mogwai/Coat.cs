namespace WoMFramework.Game.Model.Mogwai
{
    using System.Collections.Generic;

    public class Coat
    {
        public Attribute CoatTypeAttr = AttributeBuilder.Create("CoatType")
                .Salted(true).SetPosition(12).SetSize(2).SetCreation(4).SetMaxRange(8).Build();

        public int CoatType => CoatTypeAttr.GetValue();

        public Attribute CoatGeneticAttr = AttributeBuilder.Create("CoatGenetic")
                .Salted(true).SetPosition(14).SetSize(2).SetCreation(32).SetMaxRange(64).Build();

        public int CoatGenetic => CoatGeneticAttr.GetValue();

        public Attribute CoatColor1Attr = AttributeBuilder.Create("CoatColor1")
                .Salted(true).SetPosition(16).SetSize(2).SetCreation(256).SetMaxRange(256).Build();

        public int CoatColor1 => CoatColor1Attr.GetValue();

        public Attribute CoatColor2Attr = AttributeBuilder.Create("CoatColor2")
                .Salted(true).SetPosition(18).SetSize(2).SetCreation(256).SetMaxRange(256).Build();

        public int CoatColor2 => CoatColor2Attr.GetValue();

        public List<Attribute> All => new List<Attribute> { CoatTypeAttr, CoatGeneticAttr, CoatColor1Attr, CoatColor2Attr };

        public Coat(HexValue hexValue)
        {
            All.ForEach(p => p.CreateValue(hexValue));
        }
    }
}
