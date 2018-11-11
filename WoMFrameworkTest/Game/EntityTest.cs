﻿using System.Collections.Generic;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model;
using WoMFramework.Game.Model.Mogwai;
using WoMFramework.Tool;
using Xunit;

namespace WoMFrameworkTest.Game
{
    public class EntityTest
    {

        [Fact]
        public void BasicEntity()
        {
            string mogAddress = "MJHYMxu2kyR1ci4pYwktbeCM7yjZyVxt2i";
            string pubMogAddressHex = HexHashUtil.ByteArrayToString(Base58Encoding.Decode(mogAddress));

            var creation = new Shift(0, 1530914381, pubMogAddressHex,
                2000, "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
                2, "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
                1.00m,
                0.0001m);
            var shifts = new Dictionary<double, Shift>
            {
                {2000, creation}
            };

            var mogwai = new Mogwai(mogAddress, shifts);
            Assert.Equal("Heckaece", mogwai.Name);
        }

        [Fact]
        public void RequirementFighterTest()
        {
            var lvlClass = new LevelingAction(LevelingType.Class, ClassType.Barbarian, 0, 1);
            
            string mogAddress = "MJHYMxu2kyR1ci4pYwktbeCM7yjZyVxt2i";
            string pubMogAddressHex = HexHashUtil.ByteArrayToString(Base58Encoding.Decode(mogAddress));

            var creation = new Shift(0, 1530914381, pubMogAddressHex,
                2000, "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
                2, "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
                1.00m,
                0.0001m);
            var level = new Shift(1, 1530914381, pubMogAddressHex,
                2001, "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
                2, "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
                lvlClass.GetValue1(),
                lvlClass.GetValue2());

            var shifts = new Dictionary<double, Shift>
            {
                {2000, creation},
                {2001, level}
            };
            var mogwai = new Mogwai(mogAddress, shifts);
            mogwai.Evolve(out _);

            Assert.Equal(1, mogwai.GetRequirementValue(RequirementType.FighterLevel, 1));
            Assert.Equal(0, mogwai.GetRequirementValue(RequirementType.CasterLevel, 0));
        }

        [Fact]
        public void RequirementCasterTest()
        {
            var lvlClass = new LevelingAction(LevelingType.Class, ClassType.Sorcerer, 0, 1);

            string mogAddress = "MJHYMxu2kyR1ci4pYwktbeCM7yjZyVxt2i";
            string pubMogAddressHex = HexHashUtil.ByteArrayToString(Base58Encoding.Decode(mogAddress));

            var creation = new Shift(0, 1530914381, pubMogAddressHex,
                2000, "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
                2, "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
                1.00m,
                0.0001m);
            var level = new Shift(1, 1530914381, pubMogAddressHex,
                2001, "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
                2, "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
                lvlClass.GetValue1(),
                lvlClass.GetValue2());

            var shifts = new Dictionary<double, Shift>
            {
                {2000, creation},
                {2001, level}
            };
            var mogwai = new Mogwai(mogAddress, shifts);
            mogwai.Evolve(out _);

            Assert.Equal(0, mogwai.GetRequirementValue(RequirementType.FighterLevel));
            Assert.Equal(1, mogwai.GetRequirementValue(RequirementType.CasterLevel));
        }
    }
}
