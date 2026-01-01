using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using RimWorld;
using Verse;
using RimWorld.QuestGen;
using static SayersRemake.SayersRemakeBase;

namespace SayersRemake
{
	// Token: 0x02003BE9 RID: 15337
	public class QuestNode_Root_WandererJoin_WalkIn_Sayers : QuestNode_Root_WandererJoin
	{
		// Token: 0x06014D74 RID: 85364 RVA: 0x0063E1CC File Offset: 0x0063C3CC
		protected override void RunInt()
		{
			base.RunInt();
			Quest quest = QuestGen.quest;
			quest.Delay(60000, delegate
			{
				quest.End(QuestEndOutcome.Fail, 0, null, null, QuestPart.SignalListenMode.OngoingOnly, false, false);
			}, null, null, null, false, null, null, false, null, null, null, false, QuestPart.SignalListenMode.OngoingOnly, false);
		}

		// Token: 0x06014D75 RID: 85365 RVA: 0x0063E21C File Offset: 0x0063C41C
		public override Pawn GeneratePawn()
		{
			Slate slate = QuestGen.slate;
			PawnGenerationRequest request;
			if (!slate.TryGet<PawnGenerationRequest>("overridePawnGenParams", out request, false))
			{
				request = new PawnGenerationRequest(PawnkindDef_WildSayers, null, PawnGenerationContext.NonPlayer, -1, false, false, false, true, false, 1f, false, true, false, true, true, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, Find.FactionManager.OfPlayer.ideos.PrimaryIdeo, false, false, false, false, null, null, XenotypeDef_Sayers_Prototype, null, null, 0f, DevelopmentalStage.Adult, null, null, null, false, false, false, -1, 0, false);
			}
			if (Find.Storyteller.difficulty.ChildrenAllowed)
			{
				request.AllowedDevelopmentalStages |= DevelopmentalStage.Child;
			}
			Pawn pawn = PawnGenerator.GeneratePawn(request);
			if (!pawn.IsWorldPawn())
			{
				Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
			}
			return pawn;
		}

		// Token: 0x06014D76 RID: 85366 RVA: 0x0063E314 File Offset: 0x0063C514
		protected override void AddSpawnPawnQuestParts(Quest quest, Map map, Pawn pawn)
		{
			this.signalAccept = QuestGenUtility.HardcodedSignalWithQuestID("Accept");
			this.signalReject = QuestGenUtility.HardcodedSignalWithQuestID("Reject");
			quest.Signal(this.signalAccept, delegate
			{
				quest.SetFaction(Gen.YieldSingle<Pawn>(pawn), Faction.OfPlayer, null);
				quest.PawnsArrive(Gen.YieldSingle<Pawn>(pawn), null, map.Parent, null, false, null, null, null, null, null, false, false, true);
				quest.End(QuestEndOutcome.Success, 0, null, null, QuestPart.SignalListenMode.OngoingOnly, false, false);
			}, null, QuestPart.SignalListenMode.OngoingOnly);
			quest.Signal(this.signalReject, delegate
			{
				quest.GiveDiedOrDownedThoughts(pawn, PawnDiedOrDownedThoughtsKind.DeniedJoining, null);
				quest.End(QuestEndOutcome.Fail, 0, null, null, QuestPart.SignalListenMode.OngoingOnly, false, false);
			}, null, QuestPart.SignalListenMode.OngoingOnly);
		}

		// Token: 0x06014D77 RID: 85367 RVA: 0x00634A46 File Offset: 0x00632C46
		[Obsolete]
		public override void SendLetter(Quest quest, Pawn pawn)
		{
			this.SendLetter_NewTemp(quest, pawn, Find.AnyPlayerHomeMap);
		}

		// Token: 0x06014D78 RID: 85368 RVA: 0x0063E39C File Offset: 0x0063C59C
		public override void SendLetter_NewTemp(Quest quest, Pawn pawn, Map map)
		{
			TaggedString label = "LetterLabelWandererJoins".Translate(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN", true);
			TaggedString taggedString = "SayersRemake_SayersJoin_Label".Translate(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN", true);
			QuestNode_Root_WandererJoin_WalkIn.AppendCharityInfoToLetter("JoinerCharityInfo".Translate(pawn), ref taggedString);
			PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref taggedString, ref label, pawn);
			if (pawn.def == AlienSayersDef)
			{
				int count = PawnsFinder.HomeMaps_FreeColonistsSpawned.Count;
				for (int i = 0; i < count; i++)
				{
					Pawn existPawn = PawnsFinder.HomeMaps_FreeColonistsSpawned[i];
					if (existPawn.def == AlienSayersDef)
					{
						taggedString = "SayersRemake_SayersJoin_Text_HasSayers".Translate(pawn.Name.ToStringShort);
						break;
					}
					else
					{
						taggedString = "SayersRemake_SayersJoin_Text_HasntSayers".Translate(pawn.Name.ToStringShort);
					}
				}
			}
			QuestNode_Root_WandererJoin_WalkIn.ApplyBestSkillInfoToLetter(ref taggedString, pawn);
			ChoiceLetter_AcceptJoiner choiceLetter_AcceptJoiner = (ChoiceLetter_AcceptJoiner)LetterMaker.MakeLetter(label, taggedString, LetterDefOf.AcceptJoiner, null, null);
			choiceLetter_AcceptJoiner.signalAccept = this.signalAccept;
			choiceLetter_AcceptJoiner.signalReject = this.signalReject;
			choiceLetter_AcceptJoiner.quest = quest;
			choiceLetter_AcceptJoiner.overrideMap = map;
			choiceLetter_AcceptJoiner.StartTimeout(60000);
			Find.LetterStack.ReceiveLetter(choiceLetter_AcceptJoiner, null, 0, true);
		}

		// Token: 0x06014D79 RID: 85369 RVA: 0x0063E4CC File Offset: 0x0063C6CC
		public static void AppendCharityInfoToLetter(TaggedString charityInfo, ref TaggedString letterText)
		{
			if (ModsConfig.IdeologyActive)
			{
				IEnumerable<Pawn> source = IdeoUtility.AllColonistsWithCharityPrecept();
				if (source.Any<Pawn>())
				{
					letterText += "\n\n" + charityInfo + "\n\n" + "PawnsHaveCharitableBeliefs".Translate() + ":";
					foreach (IGrouping<Ideo, Pawn> grouping in from c in source
															   group c by c.Ideo)
					{
						letterText += "\n  - " + "BelieversIn".Translate(grouping.Key.name.Colorize(grouping.Key.TextColor), grouping.Select((Pawn f) => f.NameShortColored.Resolve()).ToCommaList(false, false));
					}
				}
			}
		}

		// Token: 0x06014D7A RID: 85370 RVA: 0x0063E604 File Offset: 0x0063C804
		public static void ApplyBestSkillInfoToLetter(ref TaggedString letterText, Pawn pawn)
		{
			if (pawn.skills == null || !pawn.skills.skills.Any<SkillRecord>())
			{
				return;
			}
			SkillRecord skillRecord = pawn.skills.skills.MaxBy((SkillRecord s) => s.Level);
			if (skillRecord == null)
			{
				return;
			}
			letterText += "\n\n" + "BestSkillLetterLabel".Translate(pawn.Named("PAWN")) + ": " + skillRecord.def.LabelCap + " (" + "BestSkillInfoLevel".Translate(skillRecord.Level) + ")";
		}

		// Token: 0x0400E815 RID: 59413
		private const int TimeoutTicks = 60000;

		// Token: 0x0400E816 RID: 59414
		public const float RelationWithColonistWeight = 20f;

		// Token: 0x0400E817 RID: 59415
		private string signalAccept;

		// Token: 0x0400E818 RID: 59416
		private string signalReject;
	}
	/*// Token: 0x02001F45 RID: 8005
	public class QuestNode_Root_WandererJoin_WalkIn_Sayers : QuestNode_Root_WandererJoin
	{
		// Token: 0x0600C062 RID: 49250 RVA: 0x0045DECC File Offset: 0x0045C0CC
		protected override void RunInt()
		{
			base.RunInt();
			Quest quest = QuestGen.quest;
			quest.Delay(60000, delegate
			{
				quest.End(QuestEndOutcome.Fail, 0, null, null, QuestPart.SignalListenMode.OngoingOnly, false, false);
			}, null, null, null, false, null, null, false, null, null, null, false, QuestPart.SignalListenMode.OngoingOnly, false);
		}

		// Token: 0x0600C063 RID: 49251 RVA: 0x0045DF1C File Offset: 0x0045C11C
		public override Pawn GeneratePawn()
		{
			Slate slate = QuestGen.slate;
			PawnGenerationRequest request;
			if (!slate.TryGet<PawnGenerationRequest>("overridePawnGenParams", out request, false))
			{
				request = new PawnGenerationRequest(PawnkindDef_WildSayers, null, PawnGenerationContext.NonPlayer, -1, false, false, false, true, false, 1f, false, true, false, true, true, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, Find.FactionManager.OfPlayer.ideos.PrimaryIdeo, false, false, false, false, null, null, XenotypeDef_Sayers_Prototype, null, null, 0f, DevelopmentalStage.Adult, null, null, null, false, false, false, -1, 0, false);
			}
			if (Find.Storyteller.difficulty.ChildrenAllowed)
			{
				request.AllowedDevelopmentalStages |= DevelopmentalStage.Child;
			}
			Pawn pawn = PawnGenerator.GeneratePawn(request);
			if (!pawn.IsWorldPawn())
			{
				Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
			}
			pawn.SetFaction(null, null);
			return pawn;
		}

		// Token: 0x0600C064 RID: 49252 RVA: 0x0045E004 File Offset: 0x0045C204
		protected override void AddSpawnPawnQuestParts(Quest quest, Map map, Pawn pawn)
		{
			this.signalAccept = QuestGenUtility.HardcodedSignalWithQuestID("Accept");
			this.signalReject = QuestGenUtility.HardcodedSignalWithQuestID("Reject");
			quest.Signal(this.signalAccept, delegate
			{
				quest.SetFaction(Gen.YieldSingle<Pawn>(pawn), Faction.OfPlayer, null);
				quest.PawnsArrive(Gen.YieldSingle<Pawn>(pawn), null, map.Parent, null, false, null, null, null, null, null, false, false, true);
				quest.End(QuestEndOutcome.Success, 0, null, null, QuestPart.SignalListenMode.OngoingOnly, false, false);
			}, null, QuestPart.SignalListenMode.OngoingOnly);
			quest.Signal(this.signalReject, delegate
			{
				quest.GiveDiedOrDownedThoughts(pawn, PawnDiedOrDownedThoughtsKind.DeniedJoining, null);
				quest.End(QuestEndOutcome.Fail, 0, null, null, QuestPart.SignalListenMode.OngoingOnly, false, false);
			}, null, QuestPart.SignalListenMode.OngoingOnly);
		}

		// Token: 0x0600C065 RID: 49253 RVA: 0x0045E08C File Offset: 0x0045C28C
		public override void SendLetterr_NewTemp(Quest quest, Pawn pawn, Map map)
		{
			//TaggedString label = "LetterLabelWandererJoins".Translate(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN", true);
			TaggedString label = "SayersRemake_SayersJoin_Label".Translate(pawn.Name.ToStringShort);
			//TaggedString taggedString = "LetterWandererJoins".Translate(pawn.Named("PAWN")).AdjustedFor(pawn, "PAWN", true);
			TaggedString taggedString;
			if (pawn.def == AlienSayersDef)
			{
				int count = PawnsFinder.HomeMaps_FreeColonistsSpawned.Count;
				for (int i = 0; i < count; i++)
				{
					Pawn existPawn = PawnsFinder.HomeMaps_FreeColonistsSpawned[i];
					if (existPawn.def == AlienSayersDef)
					{
						taggedString = "SayersRemake_SayersJoin_Text_HasSayers".Translate(pawn.Name.ToStringShort);
						break;
					}
					else
					{
						taggedString = "SayersRemake_SayersJoin_Text_HasntSayers".Translate(pawn.Name.ToStringShort);
					}
				}
			}
			QuestNode_Root_WandererJoin_WalkIn.AppendCharityInfoToLetter("JoinerCharityInfo".Translate(pawn), ref taggedString);
			PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref taggedString, ref label, pawn);
			if (pawn.DevelopmentalStage.Juvenile())
			{
				string arg = (pawn.ageTracker.AgeBiologicalYears * 3600000).ToStringTicksToPeriod(true, false, true, true, false);
				taggedString += "\n\n" + "RefugeePodCrash_Child".Translate(pawn.Named("PAWN"), arg.Named("AGE"));
			}
			ChoiceLetter_AcceptJoiner choiceLetter_AcceptJoiner = (ChoiceLetter_AcceptJoiner)LetterMaker.MakeLetter(label, taggedString, LetterDefOf.AcceptJoiner, null, null);
			choiceLetter_AcceptJoiner.signalAccept = this.signalAccept;
			choiceLetter_AcceptJoiner.signalReject = this.signalReject;
			choiceLetter_AcceptJoiner.quest = quest;
			choiceLetter_AcceptJoiner.StartTimeout(60000);
			Find.LetterStack.ReceiveLetter(choiceLetter_AcceptJoiner, null, 0, true);
		}

		// Token: 0x0600C066 RID: 49254 RVA: 0x0045E1B0 File Offset: 0x0045C3B0
		public static void AppendCharityInfoToLetter(TaggedString charityInfo, ref TaggedString letterText)
		{
			if (ModsConfig.IdeologyActive)
			{
				IEnumerable<Pawn> source = IdeoUtility.AllColonistsWithCharityPrecept();
				if (source.Any<Pawn>())
				{
					letterText += "\n\n" + charityInfo + "\n\n" + "PawnsHaveCharitableBeliefs".Translate() + ":";
					foreach (IGrouping<Ideo, Pawn> grouping in from c in source
															   group c by c.Ideo)
					{
						letterText += "\n  - " + "BelieversIn".Translate(grouping.Key.name.Colorize(grouping.Key.TextColor), grouping.Select((Pawn f) => f.NameShortColored.Resolve()).ToCommaList(false, false));
					}
				}
			}
		}

		// Token: 0x040078EE RID: 30958
		private const int TimeoutTicks = 60000;

		// Token: 0x040078EF RID: 30959
		public const float RelationWithColonistWeight = 20f;

		// Token: 0x040078F0 RID: 30960
		private string signalAccept;

		// Token: 0x040078F1 RID: 30961
		private string signalReject;
	}*/
}
