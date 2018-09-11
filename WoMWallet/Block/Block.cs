using System.Collections.Generic;

namespace WoMWallet
{
    public class Block
    {
        public string Hash { get; set; }
        public int Confirmations { get; set; }
        public int Size { get; set; }
        public int Height { get; set; }
        public int Version { get; set; }
        public string Merkleroot { get; set; }
        public List<string> Tx { get; set; }
        public int Time { get; set; }
        public int Mediantime { get; set; }
        public long Nonce { get; set; }
        public string Bits { get; set; }
        public double Difficulty { get; set; }
        public string Chainwork { get; set; }
        public string Previousblockhash { get; set; }
        public string Nextblockhash { get; set; }
    }
}
