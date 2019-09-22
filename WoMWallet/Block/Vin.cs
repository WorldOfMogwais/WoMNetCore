namespace WoMWallet.Block
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class Vin
    {
        public string CoinBase { get; set; }
        public long Sequence { get; set; }
    }
}
