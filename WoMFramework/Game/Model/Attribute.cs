namespace WoMFramework.Game.Model
{
    using Enums;
    using Mogwai;
    using System;
    using Tool;

    public sealed class AttributeBuilder
    {
        private readonly string _name;
        private bool _salted = true;
        private int _position = -1;
        private int _size = 2;
        private int _creation = 16;
        private int _minRange;
        private int _maxRange = 32;
        private EvolutionPattern _evoPat = EvolutionPattern.None;

        public HexValue HexValue;

        public string Description;

        private AttributeBuilder(string name) { _name = name; }

        public static AttributeBuilder Create(string name)
        {
            return new AttributeBuilder(name);
        }

        public AttributeBuilder Salted(bool salted)
        {
            _salted = salted;
            return this;
        }

        public AttributeBuilder SetPosition(int position)
        {
            _position = position;
            return this;
        }

        public AttributeBuilder SetSize(int size)
        {
            _size = size;
            return this;
        }

        public AttributeBuilder SetCreation(int creation)
        {
            _creation = creation;
            return this;
        }

        public AttributeBuilder SetMinRange(int minRange)
        {
            _minRange = minRange;
            return this;
        }

        public AttributeBuilder SetMaxRange(int maxRange)
        {
            _maxRange = maxRange;
            return this;
        }

        public AttributeBuilder SetEvolutionPattern(EvolutionPattern evoPat)
        {
            _evoPat = evoPat;
            return this;
        }

        public AttributeBuilder SetHexValue(HexValue hexValue)
        {
            HexValue = hexValue;
            return this;
        }

        public AttributeBuilder SetDescription(string description)
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
                Console.WriteLine("HexValue is null, can\'t calculate value without.");
                return false;
            }

            if (!HexHashUtil.TryHexPosConversion(Position, Size, Salted ? hexValue.Salted : hexValue.UnSalted, out var result))
            {
                Console.WriteLine($"CreateValue, failed trying to convert hex position.{Position}, {Size}, {Salted}");
                return false;
            }

            var modValue = result % (Creation - MinRange) + MinRange;

            if (Math.Abs(modValue % 1) > Double.Epsilon)
            {
                Console.WriteLine($"CreateValue, failed to cast double in same int value.{modValue},{(int)modValue}");
                return false;
            }

            _value = (int)modValue;
            return true;
        }
    }
}
