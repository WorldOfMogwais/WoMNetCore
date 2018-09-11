using Xunit;

using System.Collections.Generic;
using WoMFramework.Game.Interaction;
using WoMFramework.Tool;

namespace WoMFramework.Game.Model.Tests
{
    public class AttributeTest
    {
        [Fact]
        public void GenderTest()
        {
            Attribute GenderAttr = AttributBuilder.Create("Gender")
                .Salted(false).Position(2).Size(1).Creation(2).MaxRange(2).Build();

            Dictionary<int, int> dict = new Dictionary<int, int>();
            foreach (char b1 in Base58Encoding.Digits.ToCharArray())
            {
                foreach (char b2 in Base58Encoding.Digits.ToCharArray())
                {
                    string addr = "M" + b1 + b2 + "KtKS3AeNuRFWE5Qj9tFiNAahWvQMTiz";
                    var pubMogAddressHex = HexHashUtil.ByteArrayToString(Base58Encoding.Decode(addr));
                    HexValue hexValue = 
                        new HexValue(
                            new Shift(0D,
                            1530914381,
                            pubMogAddressHex,
                            7234,
                            "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
                            2,
                            "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
                            1.00m,
                            0.0001m));

                    GenderAttr.CreateValue(hexValue);

                    int value = GenderAttr.GetValue();
                    int orgValue = HexHashUtil.GetHexVal(pubMogAddressHex[1]);
                    if (dict.TryGetValue(value, out int count))
                    {
                        dict[value] = count + 1;
                    }
                    else
                    {
                        dict.Add(value, 1);
                    }
                }
            }

            Assert.Equal(2, dict.Count);
            var enumerator = dict.Keys.GetEnumerator();
            enumerator.MoveNext();
            Assert.Equal(1, enumerator.Current);
            enumerator.MoveNext();
            Assert.Equal(0, enumerator.Current);
            Assert.Equal(1692, dict[0]);
            Assert.Equal(1672, dict[1]);
        }
    }
}
