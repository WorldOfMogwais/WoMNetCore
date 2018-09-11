﻿using log4net;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using WoMWallet.Tool;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model;
using WoMFramework.Tool;

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

        public decimal Balance { get; set; } = 0.0000m;

        public Dictionary<double, Shift> Shifts { get; set; }

        private MogwaiKeysState _oldMogwaiKeysState = MogwaiKeysState.None;
        private MogwaiKeysState _mogwaiKeysState = MogwaiKeysState.None;
        public MogwaiKeysState MogwaiKeysState
        {
            get { return _mogwaiKeysState; }
            set
            {
                _oldMogwaiKeysState = _mogwaiKeysState;
                _mogwaiKeysState = value;
            }
        }

        public MogwaiKeys(ExtKey extkey, Network network)
        {
            this._extkey = extkey;
            this._network = network;
            this._pubKey = extkey.PrivateKey.PubKey;
            if (TryMirrorPubKey(extkey.PrivateKey.PubKey, out PubKey mirrorPubKey))
            {
                this._mirrorPubKey = mirrorPubKey;
            }
        }

        public string GetEncryptedSecretWif()
        {
            return _extkey.PrivateKey.GetEncryptedBitcoinSecret(_extkey.ToString(_network), _network).ToWif();
        }

        private bool TryMirrorPubKey(PubKey pubKey, out PubKey mirrorPubKey)
        {
            var pubKeyStr = HexHashUtil.ByteArrayToString(pubKey.ToBytes());

            char[] reversedArray = pubKeyStr.Substring(10, 54).ToCharArray();
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
        public async void Update()
        {
            await Task.Run(() =>
            {
                Balance = Blockchain.Instance.GetBalance(Address);
                if (HasMirrorAddress)
                {
                    Shifts = Blockchain.Instance.GetShifts(MirrorAddress);
                    if (Shifts.Count > 0)
                    {
                        Mogwai = new Mogwai(Address, Shifts);
                    }
                }

                if (Mogwai != null)
                {
                    MogwaiKeysState = MogwaiKeysState.Bound;
                }
                else if(Balance > 1.0001m && MogwaiKeysState != MogwaiKeysState.Create)
                {
                    MogwaiKeysState = MogwaiKeysState.Ready;
                }
                else if(Balance < 1.0001m && MogwaiKeysState != MogwaiKeysState.Wait)
                {
                    MogwaiKeysState = MogwaiKeysState.None;
                }
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
            Transaction tx = Transaction.Create(_network);

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
            if ((unspentAmount - (amount * toAddresses.Length)) > txFee)
            {
                tx.AddOutput(new TxOut
                {
                    ScriptPubKey = _pubKey.GetAddress(_network).ScriptPubKey,
                    Value = Money.Coins(unspentAmount - (amount * toAddresses.Length) - txFee)
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