using log4net;
using System;
using System.Reflection;
using WoMFramework.Game.Enums;
using WoMFramework.Tool;

namespace WoMFramework.Game.Model
{
    public sealed class AttributBuilder
    {
        private readonly string _name;
        private bool _salted = true;
        private int _position = -1;
        private int _size = 2;
        private int _creation = 16;
        private int _minRange = 0;
        private int _maxRange = 32;
        private EvolutionPattern _evoPat = EvolutionPattern.None;

        public HexValue HexValue;

        public string Description;

        private AttributBuilder(string name) { _name = name; }

        public static AttributBuilder Create(string name)
        {
            return new AttributBuilder(name);
        }

        public AttributBuilder Salted(bool salted)
        {
            _salted = salted;
            return this;
        }

        public AttributBuilder SetPosition(int position)
        {
            _position = position;
            return this;
        }

        public AttributBuilder SetSize(int size)
        {
            _size = size;
            return this;
        }

        public AttributBuilder SetCreation(int creation)
        {
            _creation = creation;
            return this;
        }

        public AttributBuilder SetMinRange(int minRange)
        {
            _minRange = minRange;
            return this;
        }

        public AttributBuilder SetMaxRange(int maxRange)
        {
            _maxRange = maxRange;
            return this;
        }

        public AttributBuilder SetEvolutionPattern(EvolutionPattern evoPat)
        {
            _evoPat = evoPat;
            return this;
        }

        public AttributBuilder SetHexValue(HexValue hexValue)
        {
            HexValue = hexValue;
            return this;
        }

        public AttributBuilder SetDescription(string description)
        {
            Description = description;
            return this;
        }

        public Attribute Build()
        {
            return new Attribute(_name, _salted, _position, _size, _creation, _minRange, _maxRange, _evoPat);
        }
    }

    public class Attribute
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private HexValue _hexValue;

        public string Name { get; }
        public bool Salted { get; }
        public int Position { get; }
        public int Size { get; }
        public int Creation { get; }
        public int MinRange { get; }
        public int MaxRange { get; }
        public EvolutionPattern EvoPat { get; set; }

        public bool Valid => _value > -1;

        private double _value = -1;

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
            return (int)_value;
        }
        
        public bool CreateValue(HexValue hexValue)
        {
            _hexValue = hexValue;

            if (_hexValue == null)
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

            _value = (int)modValue;
            return true;
        }

    }
}
