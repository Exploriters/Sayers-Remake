using System;
using RimWorld;
using Verse;
using static SayersRemake.SayersRemakeBase;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using HarmonyLib;

namespace SayersRemake
{
    internal static partial class ExploritePatches
	{
		internal static bool GenerateSayersPostprocess(ref Pawn pawn, PawnGenerationRequest request, ref bool matchError)
		{
			if (matchError)
			{
				return false;
			}
			if (pawn.def != AlienSayersDef)
			{
				return false;
			}
			if (pawn.kindDef.race != AlienSayersDef)
			{
				matchError = true;
				return false;
			}

			pawn.ageTracker.AgeBiologicalTicks = Clamp(pawn.ageTracker.AgeBiologicalTicks / 10, 3600000, 10800000);
			pawn.ageTracker.AgeChronologicalTicks = pawn.ageTracker.AgeBiologicalTicks;

			if (pawn.Name is NameTriple name)
			{
				//__result.Name = new NameTriple(name.Last, name.Last, null);
				string nameFirst = PawnNameDatabaseShuffled.BankOf(PawnNameCategory.HumanStandard).GetName(PawnNameSlot.First, Rand.Bool ? Gender.Female : Gender.Male);
				pawn.Name = new NameTriple(nameFirst, nameFirst, "Sayers");
			}

			return true;
		}

		internal static bool GeneratePawnFinalPostprocess(ref Pawn pawn, PawnGenerationRequest request)
		{
			if (pawn == null)
			{
				return false;
			}
			/*if (pawn?.TryGetComp<CompEnsureAbility>() != null)
			{
				pawn?.TryGetComp<CompEnsureAbility>().ApplayAbilities();
			}*/

			try
			{
				/*if (pawn?.RaceProps?.hediffGiverSets != null)
				{
					foreach (HediffGiverSetDef hediffGiverSetDef in pawn?.RaceProps?.hediffGiverSets ?? Enumerable.Empty<HediffGiverSetDef>())
					{
						foreach (HediffGiver hediffGiver in hediffGiverSetDef?.hediffGivers ?? Enumerable.Empty<HediffGiver>())
						{
							if (hediffGiver is HediffGiver_EnsureForAlways giver)
							{
								giver?.TryApply(pawn);
							}
						}
					}
				}*/
			}
			catch (NullReferenceException e)
			{
				Log.Message(string.Concat(
					$"[Explorite]an exception ({e.GetType().Name}) occurred during pawn generating.\n",
					$"Message:\n   {e.Message}\n",
					$"Stack Trace:\n{e.StackTrace}\n"
					));
			}
			catch (Exception e)
			{
				Log.Error(string.Concat(
					$"[Explorite]an exception ({e.GetType().Name}) occurred during pawn generating.\n",
					$"Message:\n   {e.Message}\n",
					$"Stack Trace:\n{e.StackTrace}\n"
					));
			}

			return true;
		}

		/**
		  * <summary>
		  * 对人物生成器的补丁。
		  * </summary>
		  */
				[HarmonyPostfix]
		public static void GeneratePawnPostfix(ref Pawn __result, PawnGenerationRequest request)
		{
			bool matchError = false;
			GenerateSayersPostprocess(ref __result, request, ref matchError);

			if (matchError)
			{
				request.KindDef = PawnKindDefOf.Villager;
				__result = PawnGenerator.GeneratePawn(request);
				matchError = false;
			}

			GeneratePawnFinalPostprocess(ref __result, request);
		}
	}
}
