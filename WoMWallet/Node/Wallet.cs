using log4net;
using NBitcoin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using WoMWallet.Tool;

namespace WoMWallet.Node
{
    public class WalletFile
    {
        public string wifKey;

        public string depositAddress;

        public byte[] chainCode;

        public Dictionary<string, uint> EncryptedSecrets = new Dictionary<string, uint>();

        public WalletFile(string wifKey, byte[] chainCode)
        {
            this.wifKey = wifKey;
            this.chainCode = chainCode;
        }

    }

    public class MogwaiWallet
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string defaultWalletFile = "wallet.dat";

        private readonly Network network = NBitcoin.Altcoins.Mogwai.Instance.Mainnet;

        private readonly string path;

        private WalletFile walletFile;

        private ExtKey extKey;

        private Mnemonic mnemo;
        public string MnemonicWords
        {
            get
            {
                string mnemoStr = mnemo != null ? mnemo.ToString() : string.Empty;
                mnemo = null;
                return mnemoStr;
            }
        }

        public Dictionary<string, MogwaiKeys> MogwaiKeyDict { get; set; } = new Dictionary<string, MogwaiKeys>();

        public bool IsUnlocked => extKey != null;

        public bool IsCreated => walletFile != null;

        private MogwaiKeys depositKeys;
        public MogwaiKeys Deposit
        {
            get
            {
                return depositKeys ?? (depositKeys = GetMogwaiKeys(0));
            }
        }

        public int MogwaiAddresses => MogwaiKeyDict.Count();

        public int MogwaisBound => MogwaiKeyDict.Values.Where(p => p.Mogwai != null).Count();

        public Block LastBlock { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MogwaiWallet()
        {
            if (!Caching.TryReadFile(defaultWalletFile, out walletFile))
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public MogwaiWallet(string path)
        {
            this.path = path;

            if (!Caching.TryReadFile(path, out walletFile))
            {
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="password"></param>
        /// <param name="path"></param>
        public MogwaiWallet(string password, string path)
        {
            this.path = path;

            if (!Caching.TryReadFile(path, out walletFile))
            {
                Create(password);
            }
            else
            {
                Unlock(password);
            }
        }

        /// <summary>
        /// Create a new wallet with mnemoic
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Create(string password)
        {
            if (IsCreated)
            {
                return true;
            }

            mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
            extKey = mnemo.DeriveExtKey(password);
            var chainCode = extKey.ChainCode;
            var encSecretWif = extKey.PrivateKey.GetEncryptedBitcoinSecret(password, network).ToWif();
            walletFile = new WalletFile(encSecretWif, chainCode)
            {
                depositAddress = Deposit.Address
            };
            Caching.Persist(path, walletFile);
            return true;
        }

        /// <summary>
        /// Unlock a locked wallet.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Unlock(string password)
        {
            if (IsUnlocked || !IsCreated)
            {
                return IsUnlocked && IsCreated;
            }
            try
            {
                var masterKey = Key.Parse(walletFile.wifKey, password, network);
                extKey = new ExtKey(masterKey, walletFile.chainCode);
            }
            catch (SecurityException ex)
            {
                _log.Error(ex);
                return false;
            }
            // finally load all keys
            LoadKeys();

            return true;
        }

        private void LoadKeys()
        {
            foreach (var seed in walletFile.EncryptedSecrets.Values)
            {
                var mogwaiKey = GetMogwaiKeys(seed);
                if (!MogwaiKeyDict.ContainsKey(mogwaiKey.Address))
                {
                    MogwaiKeyDict[mogwaiKey.Address] = mogwaiKey;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mogwaiKeys"></param>
        /// <param name="tryes"></param>
        /// <returns></returns>
        public bool GetNewMogwaiKey(out MogwaiKeys mogwaiKeys, int tryes = 10)
        {
            mogwaiKeys = null;

            if (!IsUnlocked)
            {
                return false; ;
            }

            uint seed = 1000;
            if (walletFile.EncryptedSecrets.Values.Count > 0)
            {
                seed = walletFile.EncryptedSecrets.Values.Max() + 1;
            }

            for (uint i = seed; i < seed + tryes; i++)
            {
                var mogwayKeysTemp = GetMogwaiKeys(i);
                if (mogwayKeysTemp.HasMirrorAddress)
                {
                    var wif = mogwayKeysTemp.GetEncryptedSecretWif();
                    if (!walletFile.EncryptedSecrets.ContainsKey(wif))
                    {
                        walletFile.EncryptedSecrets[wif] = i;
                        mogwaiKeys = mogwayKeysTemp;
                        // add to the current mogwai keys
                        MogwaiKeyDict[mogwaiKeys.Address] = mogwaiKeys;
                        // persist to not loose
                        Caching.Persist(path, walletFile);
                        return true;
                    }

                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="persist"></param>
        /// <returns></returns>
        private MogwaiKeys GetMogwaiKeys(uint seed)
        {
            if (!IsUnlocked)
            {
                return null;
            }

            var extKeyDerived = extKey.Derive(seed);
            var wif = extKey.PrivateKey.GetWif(network);
            return new MogwaiKeys(extKeyDerived, network);
        }

        /// <summary>
        /// 
        /// </summary>
        public async void Update()
        {
            await Task.Run(() =>
            {
                var height = Blockchain.Instance.MaxCachedBlockHeight;
                var hash = Blockchain.Instance.GetBlockHash(height);
                var block = Blockchain.Instance.GetBlock(hash);
                LastBlock = block;
            });
        }
    }

}
