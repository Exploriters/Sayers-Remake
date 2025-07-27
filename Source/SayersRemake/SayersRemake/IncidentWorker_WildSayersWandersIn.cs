//基本上是复制粘贴sry
using System;
using Verse;
using RimWorld;
using static SayersRemake.SayersRemakeBase;

namespace SayersRemake
{
    public class IncidentWorker_WildSayersWandersIn : IncidentWorker
    {
		// Token: 0x06006502 RID: 25858 RVA: 0x0021BB94 File Offset: 0x00219D94
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			if (!base.CanFireNowSub(parms))
			{
				return false;
			}
			Faction faction;
			if (!this.TryFindFormerFaction(out faction))
			{
				return false;
			}
			Map map = (Map)parms.target;
			IntVec3 intVec;
			return !map.GameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout) && (!ModsConfig.BiotechActive || !map.GameConditionManager.ConditionIsActive(GameConditionDefOf.NoxiousHaze)) && map.mapTemperature.SeasonAcceptableFor(AlienSayersDef, 0f) && this.TryFindEntryCell(map, out intVec);
		}

		// Token: 0x06006503 RID: 25859 RVA: 0x0021BC18 File Offset: 0x00219E18
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			IntVec3 loc;
			if (!this.TryFindEntryCell(map, out loc))
			{
				return false;
			}
			Faction faction;
			if (!this.TryFindFormerFaction(out faction))
			{
				return false;
			}
			DevelopmentalStage developmentalStages = Find.Storyteller.difficulty.ChildrenAllowed ? (DevelopmentalStage.Child | DevelopmentalStage.Adult) : DevelopmentalStage.Adult;
			Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnkindDef_WildSayers, faction, PawnGenerationContext.NonPlayer, -1, false, false, false, true, false, 1f, false, true, false, true, true, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, false, false, false, false, null, null, XenotypeDef_Sayers_Prototype, null, null, 0f, developmentalStages, null, null, null, false, false, false, -1, 0, false));
			pawn.SetFaction(null, null);
			GenSpawn.Spawn(pawn, loc, map, WipeMode.Vanish);
			string value = pawn.DevelopmentalStage.Child() ? "FeralChild".Translate().ToString() : pawn.KindLabel;
			TaggedString value2 = pawn.DevelopmentalStage.Child() ? "Child".Translate() : "Person".Translate();
			TaggedString baseLetterLabel = this.def.letterLabel.Formatted(value, pawn.Named("PAWN")).CapitalizeFirst();
			TaggedString baseLetterText = this.def.letterText.Formatted(pawn.NameShortColored, value2, pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN", true).CapitalizeFirst();
			PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref baseLetterText, ref baseLetterLabel, pawn);
			base.SendStandardLetter(baseLetterLabel, baseLetterText, this.def.letterDef, parms, pawn, Array.Empty<NamedArgument>());
			return true;
		}

		// Token: 0x06006504 RID: 25860 RVA: 0x0021BE08 File Offset: 0x0021A008
		private bool TryFindEntryCell(Map map, out IntVec3 cell)
		{
			return CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => map.reachability.CanReachColony(c), map, CellFinder.EdgeRoadChance_Ignore, out cell);
		}

		// Token: 0x06006505 RID: 25861 RVA: 0x0021BE3F File Offset: 0x0021A03F
		private bool TryFindFormerFaction(out Faction formerFaction)
		{
			return Find.FactionManager.TryGetRandomNonColonyHumanlikeFaction(out formerFaction, false, true, TechLevel.Undefined, false);
		}
	}
}
