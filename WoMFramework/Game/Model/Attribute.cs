using log4net;
using System;
using System.Reflection;
using WoMFramework.Game.Enums;
using WoMFramework.Tool;

namespace WoMFramework.Game.Model
{
    public sealed class AttributBuilder
    {
        private string name;
        private bool salted = true;
        private int position = -1;
        private int size = 2;
        private int creation = 16;
        private int minRange = 0;
        private int maxRange = 32;
        private EvolutionPattern evoPat = Enums.EvolutionPattern.NONE;

        public HexValue hexValue;
        public string description;

        private AttributBuilder(string name) { this.name = name; }

        public static AttributBuilder Create(string name)
        {
            return new AttributBuilder(name);
        }

        public AttributBuilder Salted(bool salted)
        {
            this.salted = salted;
            return this;
        }

        public AttributBuilder Position(int position)
        {
            this.position = position;
            return this;
        }

        public AttributBuilder Size(int size)
        {
            this.size = size;
            return this;
        }

        public AttributBuilder Creation(int creation)
        {
            this.creation = creation;
            return this;
        }

        public AttributBuilder MinRange(int minRange)
        {
            this.minRange = minRange;
            return this;
        }

        public AttributBuilder MaxRange(int maxRange)
        {
            this.maxRange = maxRange;
            return this;
        }

        public AttributBuilder EvolutionPattern(EvolutionPattern evoPat)
        {
            this.evoPat = evoPat;
            return this;
        }

        public AttributBuilder HexValue(HexValue hexValue)
        {
            this.hexValue = hexValue;
            return this;
        }

        public AttributBuilder Description(string description)
        {
            this.description = description;
            return this;
        }

        public Attribute Build()
        {
            return new Attribute(name, salted, position, size, creation, minRange, maxRange, evoPat);
        }
    }

    public class Attribute
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private HexValue _hexValue;

        public string Name { get; }
        public bool Salted { get; }
        public int Position { get; }
        public int Size { get; }
        public int Creation { get; }
        public int MinRange { get; }
        public int MaxRange { get; }
        public EvolutionPattern EvoPat { get; set; }

        public bool Valid => value > -1;

        private double value = -1;

        public Attribute(string name, bool salted, int position, int size, int creation, int minRange, int maxRange, EvolutionPattern evoPat)
        {
            Name = name;
            Salted = salted;
            Position = position;
            Size = size;
            Creation = creation;
            MinRange = minRange;
            MaxRange = maxRange;
            EvoPat = evoPat;
        }

        public int GetValue()
        {
            return (int)value;
        }
        
        public bool CreateValue(HexValue hexValue)
        {
            this._hexValue = hexValue;

            if (this._hexValue == null)
            {
                Console.WriteLine($"HexValue is null, can't calculate value without.");
                return false;
            }

            if (!HexHashUtil.TryHexPosConversion(Position, Size, Salted? hexValue.Salted : hexValue.UnSalted, out double result))
            {
                Console.WriteLine($"CreateValue, failed trying to convert hex position.{Position}, {Size}, {Salted}");
                return false;
            }

            double modValue = (result % (Creation - MinRange)) + MinRange;

            if (modValue % 1 != 0)
            {
                Console.WriteLine($"CreateValue, failed to cast double in same int value.{modValue},{(int)modValue}");
                return false;
            }

            value = (int)modValue;
            return true;
        }

    }
}
