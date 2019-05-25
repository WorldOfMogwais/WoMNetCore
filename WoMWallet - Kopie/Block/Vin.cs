using System.Diagnostics.CodeAnalysis;

namespace WoMWallet.Block
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class Vin
    {
        public string Coinbase { get; set; }
        public long Sequence { get; set; }
    }
}
