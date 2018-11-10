using System;
using System.Collections.Generic;

namespace WoMFramework.Game.Model
{
    public class Requirement
    {
        public RequirementType RequirementType { get; }
        public int Value { get; }

        public Requirement(RequirementType requirementTyp, int value)
        {
            RequirementType = requirementTyp;
            Value = value;
        }

        public bool Valid(Entity entity)
        {
            return entity.GetRequirementValue(RequirementType) >= Value;
        }

        public override string ToString()
        {
            return $"{RequirementType}:{Value}";
        }
    }
}