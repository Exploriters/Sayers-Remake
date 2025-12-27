/********************
 * 包含多个补丁的合集文件。
 * --siiftun1857
 */
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using static SayersRemake.SayersRemakeBase;
using static Verse.DamageInfo;
using static Verse.PawnCapacityUtility;


namespace SayersRemake
{
    //其实是核心的东西来着！？
    ///<summary>包含多个补丁的合集类。</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, "IDE0055")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, "IDE0058")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(null, "IDE0060")]
    [StaticConstructorOnStartup]
    internal static partial class ExploritePatches
    {
        internal static readonly Type patchType = typeof(ExploritePatches);
        static ExploritePatches()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                Patch(AccessTools.Method(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new[] { typeof(PawnGenerationRequest) }),
                    postfix: new HarmonyMethod(patchType, nameof(GeneratePawnPostfix)));

				Patch(AccessTools.Method(typeof(JobGiver_GetFood), "TryGiveJob"),
					postfix: new HarmonyMethod(patchType, nameof(GetFoodTryGiveJobPostfix)));
				Patch(AccessTools.Method(typeof(SkillRecord), nameof(SkillRecord.Interval)),
					transpiler: new HarmonyMethod(patchType, nameof(SkillRecordIntervalTranspiler)));
				Patch(AccessTools.Method(typeof(SkillRecord), "get_LearningSaturatedToday"),
					postfix: new HarmonyMethod(patchType, nameof(SkillRecordLearningSaturatedTodayPostfix)));
				Patch(AccessTools.Method(typeof(FoodUtility), nameof(FoodUtility.GetFoodPoisonChanceFactor)),
					prefix: new HarmonyMethod(patchType, nameof(FoodUtilityGetFoodPoisonChanceFactorPrefix)));
				Patch(AccessTools.Method(typeof(MeditationFocusDef), nameof(MeditationFocusDef.CanPawnUse)),
					postfix: new HarmonyMethod(patchType, nameof(MeditationFocusCanPawnUsePostfix)));
				Patch(AccessTools.Method(typeof(MeditationFocusDef), nameof(MeditationFocusDef.EnablingThingsExplanation)),
					postfix: new HarmonyMethod(patchType, nameof(MeditationFocusExplanationPostfix)));
				// 依赖 类 AlienRace.RaceRestrictionSettings
				Patch(AccessTools.Method(AccessTools.TypeByName("AlienRace.RaceRestrictionSettings"), "CanWear"),
					willResolve: InstelledMods.HAR,
					postfix: new HarmonyMethod(patchType, nameof(RaceRestrictionSettingsCanWearPostfix))); ;
				Patch(AccessTools.Method(typeof(ApparelProperties), nameof(ApparelProperties.GetCoveredOuterPartsString)),
					prefix: new HarmonyMethod(patchType, nameof(ApparelPropertiesGetCoveredOuterPartsStringPostfix)));
				////////////////////////////////////////////////////////////////////////////////////
				///SOCIAL
				Patch(AccessTools.Method(typeof(InteractionWorker_Chitchat), "RandomSelectionWeight"),
					postfix: new HarmonyMethod(patchType, nameof(InteractionWorkerPostfix)));//闲聊
				Patch(AccessTools.Method(typeof(InteractionWorker_DeepTalk), "RandomSelectionWeight"),
					postfix: new HarmonyMethod(patchType, nameof(InteractionWorkerPostfix)));//深入交流
				Patch(AccessTools.Method(typeof(InteractionWorker_Slight), "RandomSelectionWeight"),
					postfix: new HarmonyMethod(patchType, nameof(InteractionWorkerPostfix)));//怠慢
				Patch(AccessTools.Method(typeof(InteractionWorker_Insult), "RandomSelectionWeight"),
					postfix: new HarmonyMethod(patchType, nameof(InteractionWorkerPostfix)));//侮辱
				Patch(AccessTools.Method(typeof(InteractionWorker_KindWords), "RandomSelectionWeight"),
					postfix: new HarmonyMethod(patchType, nameof(InteractionWorkerPostfix)));//美言
				////////////////////////////////////////////////////////////////////////////////////
				Patch(AccessTools.Method(typeof(StatPart_Age), "ActiveFor"),
					prefix: new HarmonyMethod(patchType, nameof(StatPartAgePrefix)));
				Patch(AccessTools.Method(typeof(StatPart_AgeOffset), "ActiveFor"),
					prefix: new HarmonyMethod(patchType, nameof(StatPartAgeOffsetPrefix)));
				Patch(AccessTools.Method(typeof(LovePartnerRelationUtility), "LovinMtbSinglePawnFactor"),
					prefix: new HarmonyMethod(patchType, nameof(LovinMtbSinglePawnFactorPrefix)));
				//Patch(AccessTools.Method(typeof(WildManUtility), "IsWildMan"),
				//	prefix: new HarmonyMethod(patchType, nameof(IsWildManPrefix)));
				Patch(AccessTools.Method(typeof(MentalBreaker), "TryDoMentalBreak"),
					prefix: new HarmonyMethod(patchType, nameof(TryDoMentalBreakPrefix)));
				Patch(AccessTools.Method(typeof(Pawn_AgeTracker), "get_BiologicalTicksPerTick"),
					postfix: new HarmonyMethod(patchType, nameof(get_BiologicalTicksPerTickPostfix)));
			}
			catch (Exception e)
			{
				ExplortiePatchActionRecord last = records.Last();
				Log.Error(string.Concat(
					$"[Explorite]Patch sequence crashed, an exception ({e.GetType().Name}) occurred. Last patch was ",
					last,
					$"\n",
					$"Message:\n   {e.Message}\n",
					$"Stack Trace:\n{e.StackTrace}\n"
					));
			}
			stopwatch.Stop();
			Log.Message($"[Explorite]Patch sequence complete, solved total {records.Count()} patches, " +
				$"{records.Count(d => d.state == ExplortiePatchActionRecordState.Success)} success, " +
				$"{records.Count(d => d.state == ExplortiePatchActionRecordState.Failed)} failed, " +
				$"{records.Count(d => d.state == ExplortiePatchActionRecordState.Unresolved)} unresolved, " +
				$"in {stopwatch.ElapsedMilliseconds}ms.\n" +
				$"Printing patch records below...\n" +
				PrintPatches());
			Log.Message($"[Explorite]All patch targets:\n" +
				string.Join("\n", records.Where(s => s.original != null).OrderBy(s => s.SortValue).Select(r => r.original?.FullDescription()).Distinct())
				);
		}
		public static bool DisableThoughtWorkerPreceptHasUncoveredPredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExDisableThoughtWorkerPreceptHasUncovered") ?? false;
		}
		public static bool DisableDailySkillTrainLimitPredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExDisableDailySkillTrainLimit") ?? false;
		}

		public static bool DisableSkillsDegreesPredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExDisableSkillsDegrees") ?? false;
		}
		public static bool DisableFoodPoisoningPredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExDisableFoodPoisoning") ?? false;
		}
		public static bool DisableStatFactorSightPsychicSenstivityOffsetPredicate(ThingDef def)
		{
			return def?.tradeTags?.Contains("ExDisableStatFactorSightPsychicSenstivityOffset") ?? false;
		}
		///<summary>使Sayers优先选择尸体和生肉、生食作为食物。</summary>
		[HarmonyPostfix] public static void GetFoodTryGiveJobPostfix(JobGiver_GetFood __instance, ref Job __result, Pawn pawn)
		{
			if (pawn?.def != AlienSayersDef)
			{
				return;
			}
			if (pawn.Downed)
			{
				return;
			}
			if (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Malnutrition)?.Severity > 0.4f)
			{
				return;
			}
			Need_Food food = pawn.needs.food;
			if (
				__instance.GetType().GetField("minCategory", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is HungerCategory minCategory &&
				__instance.GetType().GetField("maxLevelPercentage", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is float maxLevelPercentage &&
				(food == null || (int)food.CurCategory < (int)minCategory || food.CurLevelPercentage > maxLevelPercentage))
			{
				return;
			}
			Thing thing = GenClosest.ClosestThingReachable(
				root: pawn.Position,
				map: pawn.Map,
				thingReq: ThingRequest.ForGroup(ThingRequestGroup.Corpse),
				peMode: PathEndMode.Touch,
				traverseParams: TraverseParms.For(pawn),
				maxDistance: 9999f,
				validator: delegate (Thing t)
				{
					/*if (!(t is Corpse) && !t.def.IsRawFood() && (t.def.ingestible.foodType & FoodTypeFlags.Meat) != 0)
                    {
						return false;
                    }*/
					if (!(t is Corpse) & !t.def.IsRawFood() & (t.def.ingestible.foodType & FoodTypeFlags.Meat) != 0)
					{
						return false;
					}
					if ((t.def.ingestible.foodType & FoodTypeFlags.Meal) != 0)
                    {
						return false;
                    }
					if (t.IsForbidden(pawn))
					{
						return false;
					}
					/*if (!pawn.foodRestriction.CurrentFoodRestriction.Allows(t))
					{
						return false;
					}*/
					if (!t.IngestibleNow)
					{
						return false;
					}
					if (!pawn.RaceProps.CanEverEat(t))
					{
						return false;
					}
					return pawn.CanReserve(t);
				});
			if (thing == null)
			{
				return;
			}

			Job job = JobMaker.MakeJob(JobDefOf.Ingest, thing);
			if (job != null)
				__result = job;
		}

		///<summary>使半人马和Sayers不被判断为裸体。</summary>
		[HarmonyPrefix] public static bool ThoughtWorkerPreceptHasUncoveredPrefix(Pawn p, ref bool __result)
		{
			if (DisableThoughtWorkerPreceptHasUncoveredPredicate(p.def))
			{
				__result = false;
				return false;
			}
			return true;
		}

		///<summary>阻止半人马的技能衰退。(Kitty needs this too!)</summary>
		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> SkillRecordIntervalTranspiler(IEnumerable<CodeInstruction> instr, ILGenerator ilg)
		{
			FieldInfo pawnInfo = typeof(SkillRecord).GetField("pawn", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			FieldInfo thingDefInfo = typeof(Thing).GetField("def", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			Label label1 = ilg.DefineLabel();
			Label label2 = ilg.DefineLabel();
			byte patchActionStage = 0;

			yield return new CodeInstruction(OpCodes.Ldarg_0);
			yield return new CodeInstruction(OpCodes.Ldfld, pawnInfo);
			yield return new CodeInstruction(OpCodes.Ldfld, thingDefInfo);
			yield return new CodeInstruction(OpCodes.Call, ((Predicate<ThingDef>)(DisableSkillsDegreesPredicate)).Method);
			yield return new CodeInstruction(OpCodes.Brfalse_S, label1);
			//yield return new CodeInstruction(OpCodes.Ldsfld, typeof(ExploriteCore).GetField(nameof(AlienCentaurDef), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static));
			//yield return new CodeInstruction(OpCodes.Bne_Un, label1);
			yield return new CodeInstruction(OpCodes.Ldc_R4, 0f);
			yield return new CodeInstruction(OpCodes.Br_S, label2);
			foreach (CodeInstruction ins in instr)
			{
				if (patchActionStage == 0)
				{
					patchActionStage++;
					ins.labels.Add(label1);
					yield return ins;
				}
				else if (patchActionStage == 1 && ins.opcode == OpCodes.Stloc_0)
				{
					patchActionStage++;
					ins.labels.Add(label2);
					yield return ins;
				}
				else
				{
					yield return ins;
				}
				continue;
			}
			TranspilerStageCheckout(patchActionStage, 2);
			yield break;
		}

		///<summary>移除半人马每日技能训练上限。(猫猫也需要！)</summary>
		[HarmonyPostfix]
		public static void SkillRecordLearningSaturatedTodayPostfix(SkillRecord __instance, ref bool __result)
		{
			if (__result &&
				__instance.GetType().GetField("pawn", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn
				&& DisableDailySkillTrainLimitPredicate(pawn.def)
				)
			{
				__result = false;
			}
		}

		///<summary>禁止种族食物中毒。</summary>
		[HarmonyPrefix]
		public static bool FoodUtilityGetFoodPoisonChanceFactorPrefix(Pawn ingester, ref float __result)
		{
			if (DisableFoodPoisoningPredicate(ingester.def))
			{
				__result = 0f;
				return false;
			}
			return true;
		}

		///<summary>使半人马和Sayers可以使用额外类型的冥想媒介。</summary>
		[HarmonyPostfix]
		public static void MeditationFocusCanPawnUsePostfix(MeditationFocusDef __instance, ref bool __result, Pawn p)
		{
			// if ((p.def == AlienCentaurDef || p.def == AlienSayersDef || p.def == AlienGuoguoDef
			if ((p.def == AlienSayersDef
				) && (__instance == DefDatabase<MeditationFocusDef>.GetNamed("Natural")     //自然
					|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Artistic")   //艺术
					|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Dignified")  //庄严
					|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Morbid")     //病态
																							//|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Minimal")  //简约
																							//|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Flame")    //火焰
					)
				)
			{
				__result = true;
			}
		}

		///<summary>为半人马和Sayers补充启用冥想类型的原因。</summary>
		[HarmonyPostfix]
		public static void MeditationFocusExplanationPostfix(MeditationFocusDef __instance, ref string __result, Pawn pawn)
		{
			// if ((p.def == AlienCentaurDef || p.def == AlienSayersDef || p.def == AlienGuoguoDef
			if ((pawn.def == AlienSayersDef
				) && (__instance == DefDatabase<MeditationFocusDef>.GetNamed("Natural")     //自然
					|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Artistic")   //艺术
					|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Dignified")  //庄严
					|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Morbid")     //病态
																							//|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Minimal")  //简约
																							//|| __instance == DefDatabase<MeditationFocusDef>.GetNamed("Flame")    //火焰
					)
				)
			{
				//__result = $"  - {"MeditationFocusEnabledByExploriteRace".Translate(pawn.def.label)}"
				__result = $"  - {"Race".Translate()}: {pawn.def.label}"
					//+ (__result.Length > 0 ? "\n" + __result : null)
					;
			}
		}

		///<summary>为Sayers禁用年龄工作效率修正曲线(Age)</summary>
		[HarmonyPrefix] public static bool StatPartAgePrefix(Pawn pawn, ref bool __result)
        {
			if (pawn.def == AlienSayersDef)
            {
				return false;

			}
			return __result;
        }
		///<summary>为Sayers禁用年龄工作效率修正曲线(AgeOffset)</summary>
		[HarmonyPrefix] public static bool StatPartAgeOffsetPrefix(Pawn pawn, ref bool __result)
        {
			if (pawn.def == AlienSayersDef)
			{
				return false;

			}
			return __result;
		}
		///<summary>为Sayers禁用性爱行为(不是哥们儿你是真饿了啊啊啊啊？？？？？)</summary>
		[HarmonyPrefix] public static void LovinMtbSinglePawnFactorPrefix(Pawn pawn, ref float __result)
        {
			if (pawn.def == AlienSayersDef)
            {
				__result = 0f;
            }
		}
		///<summary>为Sayers禁用人类互动</summary>
		[HarmonyPostfix] public static void InteractionWorkerPostfix(Pawn initiator, Pawn recipient, ref float __result)
        {
			if (initiator?.def == AlienSayersDef)
			{
				__result = 0f;
			}
		}
		[HarmonyPostfix] public static void get_BiologicalTicksPerTickPostfix(Pawn_AgeTracker __instance, ref float __result)
        {
			if (__instance.GetType().GetField("pawn", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn && pawn?.def == AlienSayersDef)
			{
				__result = 1f;
			}
		}
		///<summary>Sayers怎么不是野生的呢</summary>
		[HarmonyPrefix] public static bool IsWildManPrefix(Pawn p, ref bool __result)
		{
			return (p.kindDef == PawnkindDef_WildSayers || p.kindDef == PawnKindDefOf.WildMan) && !p.IsMutant;
		}

		///<summary>用稳定度阻止Sayers精神崩溃</summary>
		[HarmonyPrefix] public static bool TryDoMentalBreakPrefix(MentalBreaker __instance, string reason, MentalBreakDef breakDef, ref bool __result)
        {
			if (__instance.GetType().GetField("pawn", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) is Pawn pawn && pawn?.def == AlienSayersDef)
            {
				Pawn_NeedsTracker needs = pawn.needs;
				Need_Stability_Sayers need_stability = (needs != null) ? needs.TryGetNeed<Need_Stability_Sayers>() : null;
				if (need_stability == null)
                {
					return __result;
				}
				if (breakDef.intensity == MentalBreakIntensity.Extreme)
				{
					return StabilityPreventMentalbreak(pawn, 0.2f, reason, breakDef);
				}
				else if (breakDef.intensity == MentalBreakIntensity.Major)
				{
					return StabilityPreventMentalbreak(pawn, 0.1f, reason, breakDef);
				}
                else
                {
					return false;
				}
			}
			return __result;
		}

		public static bool StabilityPreventMentalbreak(Pawn pawn, float request, string reason, MentalBreakDef breakDef)
        {
			Need_Stability_Sayers need_stability = (pawn.needs != null) ? pawn.needs.TryGetNeed<Need_Stability_Sayers>() : null;
			if(need_stability.CurLevelPercentage <= request)
            {
				need_stability.CurLevelPercentage = 0f;
				Messages.Message("SayersRemake_StabilityDown".Translate(pawn.Name.ToStringShort), pawn, MessageTypeDefOf.ThreatSmall, true);
				pawn.TakeDamage(new DamageInfo(DamageDefOf.ElectricalBurn, 5f, 0f, -1f, null, pawn.health.hediffSet.GetBrain()));
				return breakDef.Worker.TryStart(pawn, reason, true);
			}
            else
            {
				need_stability.CurLevelPercentage -= request;
				Messages.Message("SayersRemake_StabilityBreak".Translate(pawn.Name.ToStringShort), pawn, MessageTypeDefOf.NegativeEvent, true);
				return false;
			}
        }
	}
}
