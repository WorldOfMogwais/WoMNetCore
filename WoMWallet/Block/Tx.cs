using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WoMWallet.Block
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class Tx
    {
        public string Txid { get; set; }
        public int Size { get; set; }
        public int Version { get; set; }
        public int Locktime { get; set; }
        public List<Vin> Vin { get; set; }
        public List<Vout> Vout { get; set; }
    }
}
