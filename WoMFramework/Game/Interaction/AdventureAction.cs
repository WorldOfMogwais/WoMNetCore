using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Interaction
{

    public class AdventureAction : Interaction
    {
        public AdventureType AdventureType { get; }

        public int ChallengeRating { get; }

        public DifficultyType DifficultyType { get; }

        public int AveragePartyLevel { get; }

        public AdventureAction(AdventureType adventureType, DifficultyType difficultyType, int averagePartyLevel) : base(InteractionType.ADVENTURE)
        {
            AdventureType = adventureType;
            ChallengeRating = averagePartyLevel + (int)difficultyType;
            DifficultyType = difficultyType;
            AveragePartyLevel = averagePartyLevel;
            ParamAdd1 = ((int)adventureType * 1000) + ChallengeRating;
            // not really used, but can be freed later ...
            ParamAdd2 = ((int)difficultyType * 1000) + averagePartyLevel;
        }

        public static bool TryGetAdventure(int paramAdd1, int paramAdd2, out AdventureAction adventure)
        {
            if (Enum.IsDefined(typeof(AdventureType), int.Parse(paramAdd1.ToString("0000").Substring(0, 1)))
             && Enum.IsDefined(typeof(DifficultyType), int.Parse(paramAdd2.ToString("0000").Substring(0, 1))))
            {
                adventure = new AdventureAction(paramAdd1, paramAdd2);
                return true;
            }

            adventure = null;
            return false;
        }

        private AdventureAction(int paramAdd1, int paramAdd2) : base(InteractionType.ADVENTURE)
        {
            string param1 = paramAdd1.ToString("0000");
            AdventureType = (AdventureType)int.Parse(param1.Substring(0, 1));
            ChallengeRating = int.Parse(param1.Substring(1));

            string param2 = paramAdd2.ToString("0000");
            DifficultyType = (DifficultyType)int.Parse(param2.Substring(0, 1));
            AveragePartyLevel = int.Parse(param2.Substring(1));

            ParamAdd1 = paramAdd1;
            ParamAdd2 = paramAdd2;
        }
    }
}
