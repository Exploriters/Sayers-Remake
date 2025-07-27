using System;
using Verse;
using RimWorld;
using static SayersRemake.SayersRemakeBase;

namespace SayersRemake
{
    public class IncidentWorker_WandererJoinSayers : IncidentWorker_GiveQuest
    {
        public override float ChanceFactorNow(IIncidentTarget target)
        {
            float result = 1.0f;
            int count = PawnsFinder.HomeMaps_FreeColonistsSpawned.Count;
            for (int i = 0; i < count; i++)
            {
                Pawn pawn = PawnsFinder.HomeMaps_FreeColonistsSpawned[i];
                if (pawn.def == AlienSayersDef)
                {
                    result = 1.5f;
                    break;
                }
                else
                {
                    result = 0.1f;
                }
            }
            return result;
        }
    }
}
