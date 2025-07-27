using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using static SayersRemake.SayersRemakeBase;

namespace SayersRemake
{
    public class CompProperties_CompSelfhealingByTime_Sayers : CompProperties
    {
        public CompProperties_CompSelfhealingByTime_Sayers()
        {
            compClass = typeof(CompSelfhealingByTime_Sayers);
        }
    }
    public class CompSelfhealingByTime_Sayers : ThingComp
    {
        public Pawn pawn => (Pawn)parent;
        public int lastHealTick = -1;
        // Heal in every 3600 ticks.
        public override void CompTick()
        {
            base.CompTick();
            if(!(pawn.def == AlienSayersDef))
            {
                return;
            }
            if(lastHealTick <0 || Find.TickManager.TicksGame >= lastHealTick + 3600)
            {
                List<BodyPartRecord> InjuredParts = pawn.health.hediffSet.GetInjuredParts();
                List<BodyPartRecord> InjuredPlantParts = new List<BodyPartRecord>();
                List<BodyPartDef> PlantParts = new List<BodyPartDef> { 
                    BodyPart_SayersBorneyeFlowers,
                    BodyPart_SayersBornthroatFlowers,
                    BodyPart_SayersTailBone,
                    BodyPart_SayersTailFur,
                    BodyPart_SayersTentacles,
                    BodyPart_SayersTentaclesBone
                };
                foreach (Hediff_MissingPart missingPartHediff in pawn.health.hediffSet.GetMissingPartsCommonAncestors())
                {
                    BodyPartRecord part = missingPartHediff.Part;
                    if (PlantParts.Contains(part.def))
                    {
                        pawn.health.RemoveHediff(missingPartHediff);
                        lastHealTick = Find.TickManager.TicksGame;
                        //Messages.Message(pawn.Name.ToStringShort + "的" + part.Label + "重新生长出来了！", pawn, MessageTypeDefOf.PositiveEvent, true);
                        Messages.Message("SayersRemake_PartReborn".Translate(pawn.Name.ToStringShort, part.Label), pawn, MessageTypeDefOf.PositiveEvent, true);
                        return;
                    }
                }
                foreach (BodyPartRecord part in InjuredParts)
                {
                    if (PlantParts.Contains(part.def))
                    {
                        InjuredPlantParts.Add(part);
                    }
                }
                foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
                {
                    
                    if (InjuredPlantParts.Contains(hediff.Part))
                    {
                        pawn.health.RemoveHediff(hediff);
                        lastHealTick = Find.TickManager.TicksGame;
                        //Messages.Message(pawn.Name.ToStringShort + "的" + hediff.Part.Label + "迭代了一部分！", pawn, MessageTypeDefOf.PositiveEvent, true);
                        Messages.Message("SayersRemake_PartHealing".Translate(pawn.Name.ToStringShort, hediff.Part.Label), pawn, MessageTypeDefOf.PositiveEvent, true);
                        return;
                    }
                }
            }
        }
    }
}
