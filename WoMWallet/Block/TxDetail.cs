namespace WoMWallet.Block
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class TxDetail
    {
        public string Address { get; set; }
        public string Height { get; set; }
        public string BlockHash { get; set; }
        public int BlockTime { get; set; }
        public int BlockIndex { get; set; }
        public int Confirmations { get; set; }
        public string TxId { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public double AmountSatoshi { get; set; }
        public decimal Fee { get; set; }
        public double FeeSatoshi { get; set; }
    }
}
