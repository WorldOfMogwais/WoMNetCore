namespace WoMWallet.Block
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class Block
    {
        public string Hash { get; set; }
        public int Confirmations { get; set; }
        public int Size { get; set; }
        public int Height { get; set; }
        public int Version { get; set; }
        public string MerkleRoot { get; set; }
        public List<string> Tx { get; set; }
        public int Time { get; set; }
        public int MedianTime { get; set; }
        public long Nonce { get; set; }
        public string Bits { get; set; }
        public double Difficulty { get; set; }
        public string ChainWork { get; set; }
        public string PreviousBlockHash { get; set; }
        public string NextBlockHash { get; set; }
    }
}
