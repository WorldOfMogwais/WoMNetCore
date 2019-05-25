using System;
using Newtonsoft.Json;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Random;

namespace WoMFramework.Game.Interaction
{
    public class Shift
    {
        public int Index { get; }

        public double Time { get; }
        public string AdHex { get; }
        public int Height { get; }
        public string BkHex { get; }
        public double BkIndex { get; }
        public string TxHex { get; }
        public decimal Amount { get; }
        public decimal Fee { get; }

        public bool IsSmallShift => Interaction == null;

        [JsonIgnore]
        private Dice _mogwaiDice;

        [JsonIgnore]
        public Dice MogwaiDice => _mogwaiDice ?? (_mogwaiDice = new Dice(this));

        [JsonIgnore]
        public InteractionType InteractionType => Interaction?.InteractionType ?? InteractionType.None;

        [JsonIgnore]
        public Interaction Interaction { get; }

        [JsonIgnore]
        public GameLog History { get; }

        [JsonConstructor]
        public Shift(int index, double time, string adHex, int height, string bkHex, double bkIndex, string txHex, decimal amount, decimal fee)
        {
            Index = index;
            Time = time;
            AdHex = adHex;
            Height = height;
            BkHex = bkHex;
            BkIndex = bkIndex;
            TxHex = txHex;
            Amount = amount;
            Fee = fee;
            Interaction = index > 0 ? Interaction.GetInteraction(amount, fee) : new CreationAction();
            History = new GameLog();
        }

        public Shift(int index, string adHex, int height, string bkHex)
        {
            Index = index;
            AdHex = adHex;
            Height = height;
            BkHex = bkHex;
            Interaction = null;
            History = new GameLog();
        }

        public override string ToString()
        {
            return $"Interaction[{Interaction?.InteractionType}]\n" +
                   $"Time = {Time}, " +
                   $"BkIndex = {BkIndex}, " +
                   $"Amount = {Amount}m, " +
                   $"Fee = {Fee}m, " +
                   $"Height = {Height}, " +
                   $"AdHex = \"{AdHex}\", " +
                   $"BkHex = \"{BkHex}\", " +
                   $"TxHex = \"{TxHex}\"";
        }

    }

}
