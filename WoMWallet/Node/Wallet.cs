using log4net;
using NBitcoin;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using WoMWallet.Tool;

namespace WoMWallet.Node
{
    public class WalletFile
    {
        public string WifKey;

        public string DepositAddress;

        public byte[] ChainCode;

        public Dictionary<string, uint> EncryptedSecrets = new Dictionary<string, uint>();

        public List<string> Unwatched = new List<string>();

        public WalletFile(string wifKey, byte[] chainCode)
        {
            WifKey = wifKey;
            ChainCode = chainCode;
        }

    }

    public class MogwaiWallet
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string DefaultWalletFile = "wallet.dat";

        private readonly Network _network = NBitcoin.Altcoins.Mogwai.Instance.Mainnet;

        private readonly string _path;

        private WalletFile _walletFile;

        private ExtKey _extKey;

        private Mnemonic _mnemo;
        public string MnemonicWords
        {
            get
            {
                var mnemoStr = _mnemo != null ? _mnemo.ToString() : string.Empty;
                _mnemo = null;
                return mnemoStr;
            }
        }

        public Dictionary<string, MogwaiKeys> MogwaiKeyDict { get; set; } = new Dictionary<string, MogwaiKeys>();

        public bool IsUnlocked => _extKey != null;

        public bool IsCreated => _walletFile != null;

        private MogwaiKeys _depositKeys;
        public MogwaiKeys Deposit => _depositKeys ?? (_depositKeys = GetMogwaiKeys(0));

        public int MogwaiAddresses => MogwaiKeyDict.Count;

        public int MogwaisBound => MogwaiKeyDict.Values.Count(p => p.Mogwai != null);

        public Block LastBlock { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MogwaiWallet(string path = DefaultWalletFile)
        {
            _path = path;

            if (!Caching.TryReadFile(path, out _walletFile))
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
            _path = path;

            if (!Caching.TryReadFile(path, out _walletFile))
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

            _mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
            _extKey = _mnemo.DeriveExtKey(password);
            var chainCode = _extKey.ChainCode;
            var encSecretWif = _extKey.PrivateKey.GetEncryptedBitcoinSecret(password, _network).ToWif();
            _walletFile = new WalletFile(encSecretWif, chainCode)
            {
                DepositAddress = Deposit.Address
            };
            Caching.Persist(_path, _walletFile);
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
                var masterKey = Key.Parse(_walletFile.WifKey, password, _network);
                _extKey = new ExtKey(masterKey, _walletFile.ChainCode);
            }
            catch (SecurityException ex)
            {
                Log.Error(ex);
                return false;
            }
            // finally load all keys
            LoadKeys();

            return true;
        }

        public void Unwatch(List<MogwaiKeys> mogwaikeys, bool flag)
        {
            foreach (var keys in mogwaikeys)
            {
                if (flag && !_walletFile.Unwatched.Contains(keys.Address))
                {
                    _walletFile.Unwatched.Add(keys.Address);
                    keys.IsUnwatched = true;
                }
                else if (!flag && _walletFile.Unwatched.Contains(keys.Address))
                {
                    _walletFile.Unwatched.Remove(keys.Address);
                    keys.IsUnwatched = false;
                }
            }

            // persist
            Caching.Persist(_path, _walletFile);
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadKeys()
        {
            foreach (var seed in _walletFile.EncryptedSecrets.Values)
            {
                var mogwaiKey = GetMogwaiKeys(seed);
                if (!MogwaiKeyDict.ContainsKey(mogwaiKey.Address))
                {
                    mogwaiKey.IsUnwatched = _walletFile.Unwatched.Contains(mogwaiKey.Address);
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
            if (_walletFile.EncryptedSecrets.Values.Count > 0)
            {
                seed = _walletFile.EncryptedSecrets.Values.Max() + 1;
            }

            for (var i = seed; i < seed + tryes; i++)
            {
                var mogwayKeysTemp = GetMogwaiKeys(i);
                if (mogwayKeysTemp.HasMirrorAddress)
                {
                    var wif = mogwayKeysTemp.GetEncryptedSecretWif();
                    if (!_walletFile.EncryptedSecrets.ContainsKey(wif))
                    {
                        _walletFile.EncryptedSecrets[wif] = i;
                        mogwaiKeys = mogwayKeysTemp;
                        // add to the current mogwai keys
                        MogwaiKeyDict[mogwaiKeys.Address] = mogwaiKeys;
                        // persist to not loose
                        Caching.Persist(_path, _walletFile);
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

            var extKeyDerived = _extKey.Derive(seed);
            var wif = _extKey.PrivateKey.GetWif(_network);
            return new MogwaiKeys(extKeyDerived, _network);
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
