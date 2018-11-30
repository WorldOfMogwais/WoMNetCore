using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using NBitcoin;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model.Mogwai;
using WoMFramework.Tool;
using WoMWallet.Block;

namespace WoMWallet.Node
{
    public enum MogwaiKeysState
    {
        None, Wait, Ready, Create, Bound
    }

    public class MogwaiKeys
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ExtKey _extkey;

        private readonly Network _network;

        private readonly PubKey _pubKey;

        private readonly PubKey _mirrorPubKey;

        public string Address => _pubKey.GetAddress(_network).ToString();

        public string MirrorAddress => _mirrorPubKey?.GetAddress(_network).ToString();

        public bool HasMirrorAddress => _mirrorPubKey != null;

        public Mogwai Mogwai { get; set; }

        public decimal Balance { get; set; }

        public Dictionary<long, Shift> Shifts { get; set; }

        public bool IsUnwatched { get; set; }

        public bool IsLocked { get; set; }

        public DateTime LastUpdated { get; set; }

        public MogwaiKeysState MogwaiKeysState { get; set; } = MogwaiKeysState.None;

        public Dictionary<string, Interaction> InteractionLock { get; set; }

        public MogwaiKeys(ExtKey extkey, Network network)
        {
            _extkey = extkey;
            _network = network;
            _pubKey = extkey.PrivateKey.PubKey;
            if (TryMirrorPubKey(extkey.PrivateKey.PubKey, out var mirrorPubKey))
            {
                _mirrorPubKey = mirrorPubKey;
            }
            InteractionLock = new Dictionary<string, Interaction>();
        }

        public MogwaiKeys()
        {
            // only for testing purpose !!!
            InteractionLock = new Dictionary<string, Interaction>();
        }

        public string GetEncryptedSecretWif()
        {
            return _extkey.PrivateKey.GetEncryptedBitcoinSecret(_extkey.ToString(_network), _network).ToWif();
        }

        private static bool TryMirrorPubKey(PubKey pubKey, out PubKey mirrorPubKey)
        {
            var pubKeyStr = HexHashUtil.ByteArrayToString(pubKey.ToBytes());

            var reversedArray = pubKeyStr.Substring(10, 54).ToCharArray();
            Array.Reverse(reversedArray);

            var mirrorPubKeyStr =
                   pubKeyStr.Substring(0, 8)
                 + pubKeyStr.Substring(8, 2)
                 + new string(reversedArray)
                 + pubKeyStr.Substring(64, 2);
            var mirrorPubKeyBytes = HexHashUtil.StringToByteArray(mirrorPubKeyStr);
            try
            {
                mirrorPubKey = new PubKey(mirrorPubKeyBytes, false);
                return false;
            }
            catch (Exception)
            {
                //Console.WriteLine(ex.InnerException);
                mirrorPubKey = new PubKey(mirrorPubKeyBytes, true);
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task Update()
        {
            await Task.Run(() =>
            {
                Balance = Blockchain.Instance.GetBalance(Address);
                 if (HasMirrorAddress)
                {
                    Shifts = Blockchain.Instance.GetShifts(MirrorAddress);
                    if (Shifts.Count > 0)
                    {
                        if (Mogwai != null)
                        {
                            Mogwai.UpdateShifts(Shifts);
                        }
                        else
                        {
                            Mogwai = new Mogwai(Address, Shifts);
                        }

                        // Updated interaction locks
                        foreach (var shift in Shifts.Values)
                        {
                            if (!shift.IsSmallShift && InteractionLock.ContainsKey(shift.TxHex))
                            {
                                InteractionLock.Remove(shift.TxHex);
                            }
                        }
                    }
                }

                if (Mogwai != null)
                {
                    MogwaiKeysState = MogwaiKeysState.Bound;
                }
                else if (Balance > 1.0001m && MogwaiKeysState != MogwaiKeysState.Create)
                {
                    MogwaiKeysState = MogwaiKeysState.Ready;
                }
                else if (Balance < 1.0001m && MogwaiKeysState != MogwaiKeysState.Wait)
                {
                    MogwaiKeysState = MogwaiKeysState.None;
                }

                LastUpdated = DateTime.Now;
            });

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="unspentTxList"></param>
        /// <param name="unspentAmount"></param>
        /// <param name="toAddresses"></param>
        /// <param name="amount"></param>
        /// <param name="txFee"></param>
        /// <returns></returns>
        public Transaction CreateTransaction(List<UnspentTx> unspentTxList, decimal unspentAmount, string[] toAddresses, decimal amount, decimal txFee)
        {
            // creating a new transaction
            var tx = Transaction.Create(_network);

            // adding all unspent txs to input
            unspentTxList.ForEach(p =>
            {
                tx.AddInput(new TxIn
                {
                    PrevOut = new OutPoint(new uint256(p.Txid), p.Vout),
                    ScriptSig = _pubKey.GetAddress(_network).ScriptPubKey
                    //ScriptSig = new Script(p.ScriptPubKey) // could be wrong then take ... prev row
                });
            });

            foreach (var address in toAddresses)
            {
                // adding output
                tx.AddOutput(new TxOut
                {
                    ScriptPubKey = BitcoinAddress.Create(address, _network).ScriptPubKey,
                    Value = Money.Coins(amount)
                });
            }

            // check if we need to add a change output
            if (unspentAmount - amount * toAddresses.Length > txFee)
            {
                tx.AddOutput(new TxOut
                {
                    ScriptPubKey = _pubKey.GetAddress(_network).ScriptPubKey,
                    Value = Money.Coins(unspentAmount - amount * toAddresses.Length - txFee)
                });
            }

            //tx.ToString(RawFormat.);

            Log.Info($"rawTx: {tx.ToHex()}");

            // sign transaction
            tx.Sign(_extkey.PrivateKey, false);

            Log.Info($"sigTx: {tx.ToHex()}");

            return tx;
        }
    }
}
