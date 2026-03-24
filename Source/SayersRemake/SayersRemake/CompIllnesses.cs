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
    public class CompProperties_CompIllnesses : CompProperties
    {
        public CompProperties_CompIllnesses()
        {
            compClass = typeof(Comp_CompIllnesses);
        }
    }

    public class Comp_CompIllnesses : ThingComp
    {
        public Pawn pawn => (Pawn)parent;

        #region Narcolepsy Variant
        int LastNarcolepsyEffectTick = -1;
        int LastNarcolepsyHorrorTick = -1;
        #endregion
        #region CotardSyndrome Variant
        int LastCotardSyndromeEffectTick = -1;
        #endregion
        public override void CompTick()
        {
            base.CompTick();
            if (pawn.Map != Find.CurrentMap) return;
            if (hasTrait("Trait_Narcolepsy"))
            {
                if (LastNarcolepsyEffectTick < 0 ||
                    Find.TickManager.TicksGame >= LastNarcolepsyEffectTick + 82800)
                {
                    // ROLL THE DICE!
                    if (RandomFloatRange(0f,1f) < 0.25f && !pawn.health.hediffSet.HasHediff(DefDatabase<HediffDef>.GetNamedSilentFail("Hediff_Narcolepsy")))
                    {
                        pawn.health.GetOrAddHediff(DefDatabase<HediffDef>.GetNamedSilentFail("Hediff_Narcolepsy"));
                        Messages.Message(pawn.Name.ToStringShort + "发生了猝倒！", pawn, MessageTypeDefOf.NegativeHealthEvent, true);
                    }
                    LastNarcolepsyEffectTick = Find.TickManager.TicksGame;
                }
                if ((LastNarcolepsyHorrorTick < 0 ||
                    (Find.TickManager.TicksGame >= LastNarcolepsyHorrorTick + 86400) && pawn.needs.rest.CurCategory > 0))
                {
                    // ROLL THE DICE!
                    if (RandomFloatRange(0f,1f) < Math.Pow(0.25f, (double)pawn.needs.rest.CurCategory))
                    {
                        float stability = pawn.needs.TryGetNeed<Need_Stability_Sayers>().CurLevelPercentage;
                        pawn.needs.TryGetNeed<Need_Stability_Sayers>().CurLevelPercentage = Clamp(stability - 0.3f, 0f, stability);
                        pawn.needs.mood.thoughts.memories.TryGainMemoryFast(DefDatabase<ThoughtDef>.GetNamedSilentFail("Thought_Narcolepsy"));
                        Messages.Message(pawn.Name.ToStringShort + "遭遇了催眠性幻觉！", pawn, MessageTypeDefOf.NegativeHealthEvent, true);
                    }
                    LastNarcolepsyHorrorTick = Find.TickManager.TicksGame;
                }
            }
            if (hasTrait("Trait_CotardSyndrome"))
            {
                if (LastCotardSyndromeEffectTick < 0 ||
                    Find.TickManager.TicksGame >= LastCotardSyndromeEffectTick + 86400)
                {
                    // ROLL THE DICE!
                    if (RandomFloatRange(0f, 1f) < 0.25f)
                    {
                        List<BodyPartDef> PlantParts = new List<BodyPartDef> {
                            BodyPart_SayersBorneyeFlowers,
                            BodyPart_SayersBornthroatFlowers,
                            BodyPart_SayersTailBone,
                            BodyPart_SayersTailFur,
                            BodyPart_SayersTentacles,
                            BodyPart_SayersTentaclesBone
                        };
                        for (var i = 0; i < 2; i++)
                        {
                            List<BodyPartRecord> RottablePlantParts = new List<BodyPartRecord>();
                            foreach (var part in pawn.health.hediffSet.GetNotMissingParts())
                            {
                                if (PlantParts.Contains(part.def))
                                {
                                    RottablePlantParts.Add(part);
                                }
                            }
                            BodyPartRecord targetPart = RottablePlantParts.RandomElement();
                            pawn.health.AddHediff(HediffDefOf.MissingBodyPart, targetPart);
                            Messages.Message(pawn.Name.ToStringShort + "的" + targetPart.Label + "腐烂消失了！", pawn, MessageTypeDefOf.NegativeHealthEvent, true);
                        }
                    }
                    LastCotardSyndromeEffectTick = Find.TickManager.TicksGame;
                }
            }
        }
        public bool hasTrait(string trait)
        {
            return pawn.story.traits.HasTrait(DefDatabase<TraitDef>.GetNamedSilentFail(trait));
        }
    }
}
