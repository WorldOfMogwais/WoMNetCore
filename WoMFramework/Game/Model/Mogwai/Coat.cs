using System.Collections.Generic;

namespace WoMFramework.Game.Model
{
    public class Coat
    {
        public Attribute CoatTypeAttr = AttributBuilder.Create("CoatType")
                .Salted(true).Position(12).Size(2).Creation(4).MaxRange(8).Build();
        public int CoatType => CoatTypeAttr.GetValue();

        public Attribute CoatGeneticAttr = AttributBuilder.Create("CoatGenetic")
                .Salted(true).Position(14).Size(2).Creation(32).MaxRange(64).Build();
        public int CoatGenetic => CoatGeneticAttr.GetValue();

        public Attribute CoatColor1Attr = AttributBuilder.Create("CoatColor1")
                .Salted(true).Position(16).Size(2).Creation(256).MaxRange(256).Build();
        public int CoatColor1 => CoatColor1Attr.GetValue();

        public Attribute CoatColor2Attr = AttributBuilder.Create("CoatColor2")
                .Salted(true).Position(18).Size(2).Creation(256).MaxRange(256).Build();
        public int CoatColor2 => CoatColor2Attr.GetValue();

        public List<Attribute> All => new List<Attribute>() { CoatTypeAttr, CoatGeneticAttr, CoatColor1Attr, CoatColor2Attr };

        public Coat(HexValue hexValue)
        {
            All.ForEach(p => p.CreateValue(hexValue));
        }
    }
}