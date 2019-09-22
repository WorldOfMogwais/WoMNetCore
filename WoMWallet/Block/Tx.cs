namespace WoMWallet.Block
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class Tx
    {
        public string TxId { get; set; }
        public int Size { get; set; }
        public int Version { get; set; }
        public int LockTime { get; set; }
        public List<Vin> Vin { get; set; }
        public List<Vout> Vout { get; set; }
    }
}
