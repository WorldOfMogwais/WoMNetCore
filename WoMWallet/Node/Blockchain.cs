using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using RestSharp;
using WoMFramework.Game.Interaction;
using WoMFramework.Tool;
using WoMWallet.Block;
using WoMWallet.Tool;

namespace WoMWallet.Node
{
    public class Blockchain
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //private const string ApiUrl = @"https://cristof.crabdance.com/mogwai/";
        private const string ApiUrl = @"https://rest.worldofmogwais.com/";

        private const string BlockhashesFile = "blockhashes.db";

        private const decimal MogwaiCost = 1.0m;

        private const decimal TxFee = 0.0001m;

        private static Blockchain _instance;

        private readonly RestClient _client;

        private readonly ConcurrentDictionary<int, string> _blockHashDict;

        public static Blockchain Instance => _instance ?? (_instance = new Blockchain());

        public int MaxCachedBlockHeight => _blockHashDict.Keys.Max();

        private Blockchain()
        {
            _client = new RestClient(ApiUrl);

            if (!Caching.TryReadFile(BlockhashesFile, out _blockHashDict))
            {
                _blockHashDict = new ConcurrentDictionary<int, string>();
            }
        }

        public async Task CacheBlockhashesAsync(IProgress<float> progress)
        {
            await Task.Run(() =>
            {
                var blockHeight = GetBlockCount();
                var fromHeight = _blockHashDict.Keys.Count > 0 ? _blockHashDict.Keys.Max() : 0;
                var bulkSize = 500;
                var count = 0;
                for (var i = fromHeight; i <= blockHeight; i++)
                {
                    count++;
                    if (count % bulkSize == 0 || i == blockHeight)
                    {
                        var currentMax = _blockHashDict.Keys.Count > 0 ? _blockHashDict.Keys.Max() : 0;
                        var list = GetBlockHashes(currentMax, count);
                        list.ForEach(p => _blockHashDict[int.Parse(p.Block)] = p.Hash);
                        Log.Debug($"cached from {fromHeight} {count} blockhashes...");
                        count = 0;
                    }
                    progress.Report((float)(i + 1) / blockHeight);
                }
                Caching.Persist(BlockhashesFile, _blockHashDict);
                Log.Debug("persisted all blocks!");
            });
        }

        public async Task CacheBlockhashesAsyncNoProgressAsync()
        {
            await CacheBlockhashesAsync(new Progress<float>());
        }

        //public void CacheBlockhashes()
        //{
        //    int maxBlockCount = GetBlockCount();
        //    var fromHeight = blockHashDict.Keys.Count > 0 ? blockHashDict.Keys.Max() : 0;
        //    int bulkSize = 500;
        //    List<BlockhashPair> list;
        //    int count = 0;
        //    for (int i = fromHeight; i < maxBlockCount; i++)
        //    {
        //        count++;
        //        if (count % bulkSize == 0 || i == maxBlockCount - 1)
        //        {
        //            var currentMax = blockHashDict.Keys.Count > 0 ? blockHashDict.Keys.Max() : 0;
        //            list = GetBlockHashes(currentMax, count);
        //            list.ForEach(p => blockHashDict[int.Parse(p.Block)] = p.Hash);
        //            _log.Debug($"cached from {fromHeight} {count} blockhashes...");
        //            count = 0;
        //        }
        //    }
        //    Caching.Persist(blockhashesFile, blockHashDict);
        //    _log.Debug($"persisted all blocks!");
        //}

        public Block.Block GetBlock(string hash)
        {
            var request = new RestRequest("getblock/{hash}", Method.GET);
            request.AddUrlSegment("hash", hash);
            var blockResponse = _client.Execute<Block.Block>(request);
            return blockResponse.Data;
        }

        public List<BlockhashPair> GetBlockHashes(int fromBlock, int count)
        {
            var request = new RestRequest("getblockhashes/{fromBlock}/{count}", Method.GET);
            request.AddUrlSegment("fromBlock", fromBlock);
            request.AddUrlSegment("count", count);
            var blockResponse = _client.Execute<List<BlockhashPair>>(request);
            return blockResponse.Data;
        }

        public string GetBlockHash(int height)
        {
            var request = new RestRequest("getblockhash/{height}", Method.GET);
            request.AddUrlSegment("height", height);
            var blockResponse = _client.Execute(request);
            return blockResponse.Content;
        }

        public int GetBlockCount()
        {
            var request = new RestRequest("getblockcount", Method.GET);
            var blockResponse = _client.Execute<int>(request);
            return blockResponse.Data;
        }

        public decimal GetBalance(string address)
        {
            var request = new RestRequest("getbalance/{address}", Method.GET);
            request.AddUrlSegment("address", address);
            var blockResponse = _client.Execute<decimal>(request);
            return blockResponse.Data;
        }

        public List<UnspentTx> GetUnspent(int minConf, int maxConf, string address)
        {
            var request = new RestRequest("listunspent/{minConf}/{maxConf}/{address}", Method.GET);
            request.AddUrlSegment("minConf", minConf);
            request.AddUrlSegment("maxConf", maxConf);
            request.AddUrlSegment("address", address);
            var blockResponse = _client.Execute<List<UnspentTx>>(request);
            return blockResponse.Data;
        }

        public string SendRawTransaction(string rawTransaction)
        {
            var request = new RestRequest("sendrawtransaction/{hex}", Method.GET);
            request.AddUrlSegment("hex", rawTransaction);
            var blockResponse = _client.Execute(request);
            return blockResponse.Content;
        }

        public List<TxDetail> ListTransactions(string address)
        {
            //:height/:numblocks
            var request = new RestRequest("listtransactions/{address}", Method.GET);
            request.AddUrlSegment("address", address);
            var blockResponse = _client.Execute<List<TxDetail>>(request);
            return blockResponse.Data;
        }

        public List<TxDetail> ListMirrorTransactions(string address)
        {
            //:height/:numblocks
            var request = new RestRequest("listmirrtransactions/{address}", Method.GET);
            request.AddUrlSegment("address", address);
            var blockResponse = _client.Execute<List<TxDetail>>(request);
            if (blockResponse.Data == null)
            {
                Log.Debug($"ListMirrorTransactions {address} {blockResponse.Content}");
            }
            return blockResponse.Data;
        }

        public bool BindMogwai(MogwaiKeys mogwaiKey, out string txid)
        {
            return BurnMogs(mogwaiKey, MogwaiCost, TxFee, out txid);
        }

        public bool Interaction(MogwaiKeys mogwaiKey, Interaction interaction, out string txid)
        {
            return SendMogs(mogwaiKey, new[] { mogwaiKey.MirrorAddress }, interaction.GetValue1(), TxFee + interaction.GetValue2(), out txid);
        }

        public bool BurnMogs(MogwaiKeys mogwaiKey, decimal burnMogs, decimal txFee, out string txid)
        {
            return SendMogs(mogwaiKey, new[] { mogwaiKey.MirrorAddress }, burnMogs, txFee, out txid);
        }

        public bool SendMogs(MogwaiKeys mogwaiKey, string[] toAddresses, decimal amount, decimal txFee, out string txid)
        {
            Log.Debug($"send mogs {mogwaiKey.Address}, [{string.Join(",", toAddresses)}] for {amount} with {txFee}.");

            txid = string.Empty;

            var unspentTxList = GetUnspent(0, 9999999, mogwaiKey.Address);
            var unspentAmount = unspentTxList.Sum(p => p.Amount);

            if (unspentAmount < amount * toAddresses.Length + txFee)
            {
                Log.Debug($"Address hasn't enough funds {unspentAmount} to burn that amount of mogs {amount * toAddresses.Length + txFee}!");
                return false;
            }

            // create transaction
            var tx = mogwaiKey.CreateTransaction(unspentTxList, unspentAmount, toAddresses, amount, txFee);

            Log.Info($"signedRawTx: {tx.ToHex()}");

            txid = SendRawTransaction(tx.ToHex());

            Log.Info($"sendRawTx[{(txid.Length != 0 ? "SUCCESS" : "FAILED")}]: {txid}");

            return txid.Length != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mirroraddress"></param>
        /// <returns></returns>
        public Dictionary<long, Shift> GetShifts(string mirroraddress)
        {
            var result = new Dictionary<long, Shift>();

            var allTxs = ListMirrorTransactions(mirroraddress);

            var validTx = allTxs.Where(p => p.Confirmations > 0).OrderBy(p => p.Blocktime).ThenBy(p => p.Blockindex)
                .ToList();

            // stop if there aren't any valid transactions ...
            if (!validTx.Any())
            {
                return result;
            }

            var pubMogAddressHex = HexHashUtil.ByteArrayToString(Base58Encoding.Decode(mirroraddress));

            var creation = false;
            var lastBlockHeight = 0;
            foreach (var tx in validTx)
            {
                var amount = Math.Abs(tx.Amount);
                if (!creation && amount < MogwaiCost)
                    continue;

                creation = true;

                var block = GetBlock(tx.Blockhash);

                if (lastBlockHeight != 0 && lastBlockHeight + 1 < block.Height)
                {
                    // add small shifts
                    for (var i = lastBlockHeight + 1; i < block.Height; i++)
                    {
                        // blockhashes aren't updated need to wait ...
                        if (!_blockHashDict.ContainsKey(i))
                        {
                            Log.Warn(
                                $"blockhashes aren't updated to the current block we got {block.Height} and our hashes are at {_blockHashDict.Keys.Max()}!");
                            return result;
                        }

                        result.Add(i, new Shift(result.Count, pubMogAddressHex, i, _blockHashDict[i]));
                    }
                }

                lastBlockHeight = block.Height;

                result.Add(block.Height,
                    new Shift(result.Count, tx.Blocktime, pubMogAddressHex, block.Height, tx.Blockhash, tx.Blockindex,
                        tx.Txid, amount, Math.Abs(tx.Fee + TxFee)));
            }

            // add small shifts
            if (creation)
            {
                int max = _blockHashDict.Keys.Max();
                for (var i = lastBlockHeight + 1; i < max; i++)
                {
                    result.Add(i, new Shift(result.Count, pubMogAddressHex, i, _blockHashDict[i]));
                }
            }

            return result;
        }

    }
}
