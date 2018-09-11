namespace WoMWallet
{
    public class Vout
    {
        public int Value { get; set; }
        public long ValueSat { get; set; }
        public int N { get; set; }
        public ScriptPubKey ScriptPubKey { get; set; }
        public string SpentTxId { get; set; }
        public int SpentIndex { get; set; }
        public int SpentHeight { get; set; }
    }
}
