namespace WoMWalletTest.Node
{
    using System.Linq;
    using WoMWallet.Node;
    using Xunit;

    [Collection("SystemInteractionFixture")]
    public class BlockchainTest
    {
        [Fact]
        public void GetBlock()
        {
            WoMWallet.Block.Block blockResponse = Blockchain.Instance.GetBlock("00000000077d796dabe050ee7d80c4e329b601e263c3e522d3abf2fbf9a9263f");
            Assert.Equal("00000000077d796dabe050ee7d80c4e329b601e263c3e522d3abf2fbf9a9263f", blockResponse.Hash);
        }

        [Fact]
        public void GetBlockHash()
        {
            var blockResponse = Blockchain.Instance.GetBlockHash(47863);
            Assert.Equal("00000000077d796dabe050ee7d80c4e329b601e263c3e522d3abf2fbf9a9263f", blockResponse);
        }

        [Fact]
        public void GetBlockHashes()
        {
            System.Collections.Generic.List<WoMWallet.Block.BlockhashPair> blockResponse = Blockchain.Instance.GetBlockHashes(0, 100);
            Assert.Equal(100, blockResponse.Count);
            Assert.Equal("1", blockResponse[1].Block);
            Assert.Equal("000004a3418bf6f7a085b0a489d56eea4fbc094be8ec48ad7ec11621a4dd7431", blockResponse[1].Hash);
        }

        [Fact]
        public void GetBlockCount()
        {
            var blockResponse = Blockchain.Instance.GetBlockCount();
            Assert.True(blockResponse > 40000);
        }

        [Fact]
        public void GetBalance()
        {
            var blockResponse1 = Blockchain.Instance.GetBalance("MMo7QQkJcZV6ZzC6Vs9tTph3LddwceoxHP");
            Assert.Equal(1.96807372m, blockResponse1);

            var blockResponse2 = Blockchain.Instance.GetBalance("MKV7YPRfB7PZd2gZ7nZfthT6fXLU6Mnv9L");
            Assert.Equal(0.97816968m, blockResponse2);
        }

        [Fact]
        public void GetUnspent()
        {
            System.Collections.Generic.List<WoMWallet.Block.UnspentTx> blockResponse1 = Blockchain.Instance.GetUnspent(0, 999999, "MMo7QQkJcZV6ZzC6Vs9tTph3LddwceoxHP");
            Assert.Single(blockResponse1);
            Assert.Equal(1.96807372m, blockResponse1[0].Amount);

            System.Collections.Generic.List<WoMWallet.Block.UnspentTx> blockResponse2 = Blockchain.Instance.GetUnspent(0, 999999, "MKV7YPRfB7PZd2gZ7nZfthT6fXLU6Mnv9L");
            Assert.Single(blockResponse2);
            Assert.Equal(0.97816968m, blockResponse2[0].Amount);
        }

        [Fact]
        public void ListTransaction()
        {
            System.Collections.Generic.List<WoMWallet.Block.TxDetail> blockResponse = Blockchain.Instance.ListTransactions("MJHYMxu2kyR1Bi4pYwktbeCM7yjZyVxt2i");
            Assert.Equal(4, blockResponse.Count);
            Assert.Equal("MJHYMxu2kyR1Bi4pYwktbeCM7yjZyVxt2i", blockResponse[0].Address);
            Assert.Equal("00000000759514788f612f69606edd5751d517c39880f72b03772e26827671c4", blockResponse[0].Blockhash);
            Assert.Equal("receive", blockResponse[0].Category);
        }

        [Fact]
        public void ListMirrorTransaction()
        {
            System.Collections.Generic.List<WoMWallet.Block.TxDetail> blockResponse = Blockchain.Instance.ListMirrorTransactions("MEYUySQDPzgbTuZSjGfPVikgHtDJZHL8WE");
            Assert.Equal(4, blockResponse.Count);
            Assert.Equal("41317", blockResponse[0].Height);
            Assert.Equal("000000003f6f8fa173c4d2ddaa07911b06fb41a72744dc646de29377fc04b19e", blockResponse[0].Blockhash);
            Assert.Equal("receive", blockResponse[0].Category);

            System.Collections.Generic.List<WoMWallet.Block.TxDetail> blockResponse1 = Blockchain.Instance.ListMirrorTransactions("MShh36ohJgMvaqJvyd6E2KYFSU7NsyeS99");
            Assert.Equal("000000006a98b2ed01fd21374a04f91022d78d3bd182c7812ec44792625822fa", blockResponse1[0].Blockhash);
        }

        [Fact]
        public void BurnMogs()
        {
            var wallet = new MogwaiWallet("1234", "test.dat");

            MogwaiKeys mogwaiKeys0 = wallet.MogwaiKeyDict["MWG1HtzRAjZMxQDzeoFoHQbzDygGR13aWG"];
            Assert.Equal("MWG1HtzRAjZMxQDzeoFoHQbzDygGR13aWG", mogwaiKeys0.Address);

            MogwaiKeys mogwaiKeys1 = wallet.MogwaiKeyDict["MNtnWbBjUhRvNnd9YxM2mnxeLPNkxb4Fio"];
            Assert.Equal("MNtnWbBjUhRvNnd9YxM2mnxeLPNkxb4Fio", mogwaiKeys1.Address);

            var blockResponse0 = Blockchain.Instance.GetBalance("MWG1HtzRAjZMxQDzeoFoHQbzDygGR13aWG");
            Assert.Equal(8.99999m, blockResponse0);

            var blockResponse1 = Blockchain.Instance.GetBalance("MNtnWbBjUhRvNnd9YxM2mnxeLPNkxb4Fio");
            Assert.Equal(12.12344678m, blockResponse1);

            System.Collections.Generic.List<WoMWallet.Block.UnspentTx> unspentTxList = Blockchain.Instance.GetUnspent(6, 9999999, mogwaiKeys0.Address);
            var unspentAmount = unspentTxList.Sum(p => p.Amount);

            // create transaction
            NBitcoin.Transaction tx = mogwaiKeys0.CreateTransaction(unspentTxList, unspentAmount, new[] { mogwaiKeys1.Address }, 1.0m, 0.00001m);
            Assert.Equal("01000000019d0262999e5eacc32c2c6921e730d57a7e51938c2d3e22158979c72a7be318e3010000006a4730440220263c7d3955de95901f70fd66210a7ba095581e6261b3f71c1165d80583fe768b022062cd638398ba735d55dcaa17de225143997307c24e84b1a2be29bc4b7a73c61f012103007f99a5c4754d67c9fed1852ed451bec7371c1b0907b8488ee5aa6593b865c4ffffffff0200e1f505000000001976a914a477c1319360114de9f3ed88381cc4dfa9147f3288ac3000af2f000000001976a914f5440a1dd1ada4c5b4160b8c754f9148eb4a505388ac00000000", tx.ToHex());

            //var blockResponse = Blockchain.Instance.SendRawTransaction(tx.ToHex());
            //Assert.Equal("", blockResponse);
        }

        [Fact]
        public void GetShifts()
        {
            System.Collections.Generic.Dictionary<long, WoMFramework.Game.Interaction.Shift> shifts1 = Blockchain.Instance.GetShifts("MEYUySQDPzgbTuZSjGfPVikgHtDJZHL8WE");
            Assert.True(shifts1.Count > 6734);
            Assert.Equal(4, shifts1.Count(p => !p.Value.IsSmallShift));
            //Assert.Equal("", shifts1[2].TxHex);
            //var shifts2 = Blockchain.Instance.GetShifts("MHULbsPvAVCCbaYm9b5wBuJ79eQPXeGgbF");
            //Assert.True(shifts2.Count > 0);
        }
    }
}
