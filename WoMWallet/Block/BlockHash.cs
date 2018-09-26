using System.Diagnostics.CodeAnalysis;

namespace WoMWallet.Block
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class BlockhashPair
    {
        public string Block { get; set; }
        public string Hash { get; set; }
    }
}
