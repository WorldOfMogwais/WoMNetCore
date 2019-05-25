using System.Diagnostics.CodeAnalysis;

namespace WoMWallet.Block
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class UnspentTx
    {
        public string Txid { get; set; }
        public int Vout { get; set; }
        public string Address { get; set; }
        public string ScriptPubKey { get; set; }
        public decimal Amount { get; set; }
        public int Confirmations { get; set; }
    }
}
