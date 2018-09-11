using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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

        public int CurrentMogwayKeysIndex { get; set; }

        public MogwaiKeys CurrentMogwayKeys
        {
            get
            {
                if (Wallet.MogwaiKeyDict.Count > CurrentMogwayKeysIndex)
                {
                    return MogwaiKeysList[CurrentMogwayKeysIndex];
                }
                return null;
            }
        }

        public string WalletMnemonicWords => Wallet.MnemonicWords;

        public bool HasMogwayKeys => MogwaiKeysDict.Count > 0;

        private Timer timer;

        public MogwaiController()
        {
            Wallet = new MogwaiWallet();
            TaggedMogwaiKeys = new List<MogwaiKeys>();
            CurrentMogwayKeysIndex = 0;
        }

        public void Refresh(int minutes)
        {
            Update();
            timer = new Timer(minutes * 60 * 1000);
            timer.Elapsed += OnTimedEventAsync;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private async void OnTimedEventAsync(object sender, ElapsedEventArgs e)
        {
            await Blockchain.Instance.CacheBlockhashesAsyncNoProgressAsync();
            Update();
        }

        private void Update()
        {
            Wallet.Update();
            Wallet.Deposit.Update();
            foreach (var mogwaiKey in Wallet.MogwaiKeyDict.Values)
            {
                mogwaiKey.Update();
            }
        }

        public void Next()
        {
            if (CurrentMogwayKeysIndex + 1 < Wallet.MogwaiKeyDict.Count)
            {
                CurrentMogwayKeysIndex++;
            }
        }

        public void Previous()
        {
            if (CurrentMogwayKeysIndex > 0)
            {
                CurrentMogwayKeysIndex--;
            }
        }

        public void Tag()
        {
            if (TaggedMogwaiKeys.Contains(CurrentMogwayKeys))
            {
                TaggedMogwaiKeys.Remove(CurrentMogwayKeys);
            }
            else
            {
                TaggedMogwaiKeys.Add(CurrentMogwayKeys);
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
            Wallet.GetNewMogwaiKey(out MogwaiKeys mogwaiKeys);
        }

        public bool SendMog()
        {
            if (!IsWalletUnlocked)
            {
                return false;
            }

            var mogwaiKeysList = TaggedMogwaiKeys.Count > 0 ? TaggedMogwaiKeys : new List<MogwaiKeys> { CurrentMogwayKeys };
            if (!Blockchain.Instance.SendMogs(Wallet.Deposit, mogwaiKeysList.Select(p => p.Address).ToArray(), 5m, 0.0001m))
            {
                return false;
            };

            mogwaiKeysList.ForEach(p => p.MogwaiKeysState = MogwaiKeysState.WAIT);
            return true;
        }

        public bool BindMogwai()
        {
            if (!IsWalletUnlocked)
            {
                return false;
            }

            if (!Blockchain.Instance.BindMogwai(CurrentMogwayKeys))
            {
                return false;
            };

            CurrentMogwayKeys.MogwaiKeysState = MogwaiKeysState.CREATE;
            return true;
        }
    }
}
