using log4net;
using NBitcoin;
using RestSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WoMWallet.Tool;
using WoMFramework.Game.Interaction;
using WoMFramework.Tool;

namespace WoMWallet.Node
{
    public class Blockchain
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string apiUrl = @"https://cristof.crabdance.com/mogwai/";

        private const string blockhashesFile = "blockhashes.db";

        private const decimal mogwaiCost = 1.0m;

        private const decimal txFee = 0.0001m;

        private static Blockchain instance;

        private RestClient client;

        private ConcurrentDictionary<int, string> blockHashDict;

        public static Blockchain Instance => instance ?? (instance = new Blockchain());

        public int MaxCachedBlockHeight => blockHashDict.Keys.Max();

        private Blockchain()
        {
            client = new RestClient(apiUrl);

            if (!Caching.TryReadFile(blockhashesFile, out blockHashDict))
            {
                blockHashDict = new ConcurrentDictionary<int, string>();
            }
        }

        public async Task CacheBlockhashesAsync(IProgress<float> progress)
        {
            await Task.Run(() =>
            {
                int blockHeight = GetBlockCount();
                var fromHeight = blockHashDict.Keys.Count > 0 ? blockHashDict.Keys.Max() : 0;
                int bulkSize = 500;
                List<BlockhashPair> list;
                int count = 0;
                for (int i = fromHeight; i <= blockHeight; i++)
                {
                    count++;
                    if (count % bulkSize == 0 || i == blockHeight)
                    {
                        var currentMax = blockHashDict.Keys.Count > 0 ? blockHashDict.Keys.Max() : 0;
                        list = GetBlockHashes(currentMax, count);
                        list.ForEach(p => blockHashDict[int.Parse(p.Block)] = p.Hash);
                        _log.Debug($"cached from {fromHeight} {count} blockhashes...");
                        count = 0;
                    }
                    progress.Report((float)(i + 1) / blockHeight);
                }
                Caching.Persist(blockhashesFile, blockHashDict);
                _log.Debug($"persisted all blocks!");
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

        public Block GetBlock(string hash)
        {
            var request = new RestRequest("getblock/{hash}", Method.GET);
            request.AddUrlSegment("hash", hash);
            IRestResponse<Block> blockResponse = client.Execute<Block>(request);
            return blockResponse.Data;
        }

        public List<BlockhashPair> GetBlockHashes(int fromBlock, int count)
        {
            var request = new RestRequest("getblockhashes/{fromBlock}/{count}", Method.GET);
            request.AddUrlSegment("fromBlock", fromBlock);
            request.AddUrlSegment("count", count);
            IRestResponse<List<BlockhashPair>> blockResponse = client.Execute<List<BlockhashPair>>(request);
            return blockResponse.Data;
        }

        public string GetBlockHash(int height)
        {
            var request = new RestRequest("getblockhash/{height}", Method.GET);
            request.AddUrlSegment("height", height);
            IRestResponse blockResponse = client.Execute(request);
            return blockResponse.Content;
        }

        public int GetBlockCount()
        {
            var request = new RestRequest("getblockcount", Method.GET);
            IRestResponse<int> blockResponse = client.Execute<int>(request);
            return blockResponse.Data;
        }

        public decimal GetBalance(string address)
        {
            var request = new RestRequest("getbalance/{address}", Method.GET);
            request.AddUrlSegment("address", address);
            IRestResponse<decimal> blockResponse = client.Execute<decimal>(request);
            return blockResponse.Data;
        }

        public List<UnspentTx> GetUnspent(int minConf, int maxConf, string address)
        {
            var request = new RestRequest("listunspent/{minConf}/{maxConf}/{address}", Method.GET);
            request.AddUrlSegment("minConf", minConf);
            request.AddUrlSegment("maxConf", maxConf);
            request.AddUrlSegment("address", address);
            IRestResponse<List<UnspentTx>> blockResponse = client.Execute<List<UnspentTx>>(request);
            return blockResponse.Data;
        }

        public string SendRawTransaction(string rawTransaction)
        {
            var request = new RestRequest("sendrawtransaction/{hex}", Method.GET);
            request.AddUrlSegment("hex", rawTransaction);
            IRestResponse blockResponse = client.Execute(request);
            return blockResponse.Content;
        }

        public List<TxDetail> ListTransactions(string address)
        {
            //:height/:numblocks
            var request = new RestRequest("listtransactions/{address}", Method.GET);
            request.AddUrlSegment("address", address);
            IRestResponse<List<TxDetail>> blockResponse = client.Execute<List<TxDetail>>(request);
            return blockResponse.Data;
        }

        public List<TxDetail> ListMirrorTransactions(string address)
        {
            //:height/:numblocks
            var request = new RestRequest("listmirrtransactions/{address}", Method.GET);
            request.AddUrlSegment("address", address);
            IRestResponse<List<TxDetail>> blockResponse = client.Execute<List<TxDetail>>(request);
            if (blockResponse.Data == null)
            {
                _log.Debug($"ListMirrorTransactions {address} {blockResponse.Content}");
            }
            return blockResponse.Data;
        }

        
        public bool BindMogwai(MogwaiKeys mogwaiKey)
        {
            return BurnMogs(mogwaiKey, mogwaiCost, txFee);
        }

        public bool BurnMogs(MogwaiKeys mogwaiKey, decimal burnMogs, decimal txFee)
        {
            return SendMogs(mogwaiKey, new string[] { mogwaiKey.MirrorAddress }, burnMogs, txFee);
        }

        public bool SendMogs(MogwaiKeys mogwaiKey, string[] toAddresses, decimal amount, decimal txFee)
        {
            _log.Debug($"send mogs {mogwaiKey.Address}, [{string.Join(",", toAddresses)}] for {amount} with {txFee}.");

            var unspentTxList = GetUnspent(0, 9999999, mogwaiKey.Address);
            var unspentAmount = unspentTxList.Sum(p => p.Amount);

            if (unspentAmount < ((amount * toAddresses.Length) + txFee))
            {
                _log.Debug($"Address hasn't enough funds {unspentAmount} to burn that amount of mogs {((amount * toAddresses.Length) + txFee)}!");
                return false;
            }

            // create transaction
            Transaction tx = mogwaiKey.CreateTransaction(unspentTxList, unspentAmount, toAddresses, amount, txFee);

            _log.Info($"signedRawTx: {tx.ToHex()}");

            var answer = SendRawTransaction(tx.ToHex());

            _log.Info($"sendRawTx[{(answer.Length != 0 ? "SUCCESS":"FAILED")}]: {answer}");

            return answer.Length != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mirroraddress"></param>
        /// <returns></returns>
        public Dictionary<double, Shift> GetShifts(string mirroraddress)
        {
            var result = new Dictionary<double, Shift>();

            List<TxDetail> allTxs = ListMirrorTransactions(mirroraddress);

            var validTx = allTxs.Where(p => p.Confirmations > 0).OrderBy(p => p.Blocktime).ThenBy(p => p.Blockindex).ToList();

            // stop if there aren't any valid transactions ...
            if (!validTx.Any())
            {
                return result;
            } 

            var pubMogAddressHex = HexHashUtil.ByteArrayToString(Base58Encoding.Decode(mirroraddress));

            bool creation = false;
            int lastBlockHeight = 0;
            foreach (var tx in validTx)
            {
                decimal amount = Math.Abs(tx.Amount);
                if (!creation && amount < mogwaiCost)
                    continue;

                creation = true;

                var block = GetBlock(tx.Blockhash);

                if (lastBlockHeight != 0 && lastBlockHeight + 1 < block.Height)
                {
                    // add small shifts
                    for (int i = lastBlockHeight + 1; i < block.Height; i++)
                    {
                        result.Add(i, new Shift(result.Count(), pubMogAddressHex, i, blockHashDict[i]));
                    }
                }

                lastBlockHeight = block.Height;

                result.Add(block.Height, new Shift(result.Count(), tx.Blocktime, pubMogAddressHex, block.Height, tx.Blockhash, tx.Blockindex, tx.Txid, amount, Math.Abs(tx.Fee + txFee)));
            }

            // add small shifts
            if (creation)
            {
                for (int i = lastBlockHeight + 1; i < blockHashDict.Keys.Max(); i++)
                {
                    result.Add(i, new Shift(result.Count(), pubMogAddressHex, i, blockHashDict[i]));
                }
            }

            //result.ForEach(p => Console.WriteLine(p.ToString()));
            return result;
        }

    }
}
