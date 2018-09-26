using System.Diagnostics.CodeAnalysis;

namespace WoMWallet.Block
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class TxDetail
    {
        public string Address { get; set; }
        public string Height { get; set; }
        public string Blockhash { get; set; }
        public int Blocktime { get; set; }
        public int Blockindex { get; set; }
        public int Confirmations { get; set; }
        public string Txid { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public double AmountSatoshi { get; set; }
        public decimal Fee { get; set; }
        public double FeeSatoshi { get; set; }
    }
}
