namespace WoMWallet.Block
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class BlockHashPair
    {
        public string Block { get; set; }
        public string Hash { get; set; }
    }
}
