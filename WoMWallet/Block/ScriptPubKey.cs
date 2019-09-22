namespace WoMWallet.Block
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class ScriptPubKey
    {
        public string Asm { get; set; }
        public string Hex { get; set; }
        public int ReqSigs { get; set; }
        public string Type { get; set; }
        public List<string> Addresses { get; set; }
    }
}
