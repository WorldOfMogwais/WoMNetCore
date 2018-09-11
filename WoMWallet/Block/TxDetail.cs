namespace WoMWallet
{
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
        public double Amount_satoshi { get; set; }
        public decimal Fee { get; set; }
        public double Fee_satoshi { get; set; }
    }
}
