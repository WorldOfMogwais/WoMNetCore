using System.Collections.Generic;
using System.Linq;
using System.Timers;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model;
using WoMFramework.Tool;
using WoMWallet.Tool;

namespace WoMWallet.Node
{
    public class MogwaiController
    {
        private MogwaiWallet Wallet { get; }

        public bool IsWalletUnlocked => Wallet.IsUnlocked;

        public bool IsWalletCreated => Wallet.IsCreated;

        public Block WalletLastBlock => Wallet.LastBlock;

        public string DepositAddress => Wallet.IsUnlocked ? Wallet.Deposit.Address : string.Empty;

        public Dictionary<string, MogwaiKeys> MogwaiKeysDict => Wallet.MogwaiKeyDict;

        public List<MogwaiKeys> MogwaiKeysList => Wallet.MogwaiKeyDict.Values.ToList();

        public List<MogwaiKeys> TaggedMogwaiKeys { get; set; }

        public int CurrentMogwaiKeysIndex { get; set; }

        public Mogwai CurrentMogwai => CurrentMogwaiKeys?.Mogwai;

        public MogwaiKeys CurrentMogwaiKeys
        {
            get
            {
                if (Wallet.MogwaiKeyDict.Count > CurrentMogwaiKeysIndex)
                {
                    return MogwaiKeysList[CurrentMogwaiKeysIndex];
                }
                return null;
            }
        }

        public string WalletMnemonicWords => Wallet.MnemonicWords;

        public bool HasMogwayKeys => MogwaiKeysDict.Count > 0;

        private Timer _timer;

        public MogwaiController()
        {
            Wallet = new MogwaiWallet();
            TaggedMogwaiKeys = new List<MogwaiKeys>();
            CurrentMogwaiKeysIndex = 0;
        }
        
        public void RefreshCurrent(int minutes)
        {
            Update();
            _timer?.Close();
            _timer = new Timer(minutes * 60 * 1000);
            _timer.Elapsed += OnTimedRefreshCurrent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private async void OnTimedRefreshCurrent(object sender, ElapsedEventArgs e)
        {
            await Blockchain.Instance.CacheBlockhashesAsyncNoProgressAsync();
            Update(false);
        }

        public void RefreshAll(int minutes)
        {
            Update();
            _timer?.Close();
            _timer = new Timer(minutes * 60 * 1000);
            _timer.Elapsed += OnTimedRefreshAll;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private async void OnTimedRefreshAll(object sender, ElapsedEventArgs e)
        {
            await Blockchain.Instance.CacheBlockhashesAsyncNoProgressAsync();
            Update();
        }

        private void Update(bool all = true)
        {
            Wallet.Update();

            if (all)
            {
                Wallet.Deposit.Update();

                foreach (var mogwaiKey in Wallet.MogwaiKeyDict.Values)
                {
                    if (!mogwaiKey.IsUnwatched)
                    {
                        mogwaiKey.Update();
                    }
                }
            }
            else
            {
                CurrentMogwaiKeys.Update();
            }
        }

        public void Next()
        {
            if (CurrentMogwaiKeysIndex + 1 < Wallet.MogwaiKeyDict.Count)
            {
                CurrentMogwaiKeysIndex++;
            }
        }

        public void Previous()
        {
            if (CurrentMogwaiKeysIndex > 0)
            {
                CurrentMogwaiKeysIndex--;
            }
        }

        public void Tag()
        {
            if (TaggedMogwaiKeys.Contains(CurrentMogwaiKeys))
            {
                TaggedMogwaiKeys.Remove(CurrentMogwaiKeys);
            }
            else
            {
                TaggedMogwaiKeys.Add(CurrentMogwaiKeys);
            }
        }

        public void ClearTag()
        {
            TaggedMogwaiKeys.Clear();
        }

        public void CreateWallet(string password)
        {
            Wallet.Create(password);
        }

        public void UnlockWallet(string password)
        {
            Wallet.Unlock(password);
        }

        public decimal GetDepositFunds()
        {
            if (!IsWalletUnlocked)
            {
                return -1;
            }
            return Wallet.Deposit.Balance;
        }

        public void PrintMogwaiKeys()
        {
            if (!IsWalletUnlocked)
            {
                return;
            }
            Caching.Persist("mogwaikeys.txt", new { Wallet.Deposit.Address, Wallet.MogwaiKeyDict.Keys });
        }

        public void NewMogwaiKeys()
        {
            if (!IsWalletUnlocked)
            {
                return;
            }
            Wallet.GetNewMogwaiKey(out var mogwaiKeys);
        }

        public bool SendMog(int amount)
        {
            if (!IsWalletUnlocked)
            {
                return false;
            }

            var mogwaiKeysList = TaggedMogwaiKeys.Count > 0 ? TaggedMogwaiKeys : new List<MogwaiKeys> { CurrentMogwaiKeys };
            if (!Blockchain.Instance.SendMogs(Wallet.Deposit, mogwaiKeysList.Select(p => p.Address).ToArray(), amount, 0.0001m))
            {
                return false;
            };

            mogwaiKeysList.ForEach(p => p.MogwaiKeysState = MogwaiKeysState.Wait);
            return true;
        }

        public bool BindMogwai()
        {
            if (!IsWalletUnlocked)
            {
                return false;
            }

            if (!Blockchain.Instance.BindMogwai(CurrentMogwaiKeys))
            {
                return false;
            };

            CurrentMogwaiKeys.MogwaiKeysState = MogwaiKeysState.Create;
            return true;
        }

        public void WatchToggle()
        {
            if (!IsWalletUnlocked)
            {
                return;
            }

            var mogwaiKeysList = TaggedMogwaiKeys.Count > 0 ? TaggedMogwaiKeys : new List<MogwaiKeys> { CurrentMogwaiKeys };
            Wallet.Unwatch(mogwaiKeysList, !CurrentMogwaiKeys.IsUnwatched);
        }

        public Mogwai TestMogwai()
        {
            var pubMogAddressHex = HexHashUtil.ByteArrayToString(Base58Encoding.Decode("MJHYMxu2kyR1Bi4pYwktbeCM7yjZyVxt2i"));
            return new Mogwai("MJHYMxu2kyR1Bi4pYwktbeCM7yjZyVxt2i", new Dictionary<double, Shift>()
                {
                    { 1001, new Shift(0, 1530914381, pubMogAddressHex,
                        1001, "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
                        2, "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
                        1.00m,
                        0.0001m)},
                    { 1002, new Shift(1, 1535295740, pubMogAddressHex,
                        1002, "0000000033dbfc3cc9f3671ba28b41ecab6f547219bb43174cc97bf23269fa88",
                        1, "db5639553f9727c42f80c22311bd8025608edcfbcfc262c0c2afe9fc3f0bcb29",
                        0.01040003m,
                        0.00001002m)}}
            );
        }
    }
}
