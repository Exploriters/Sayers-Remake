using System;
using RimWorld;
using Verse;
using Verse.Grammar;
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
		public static void allSayersGenes()
        {
			foreach (GeneDef geneDef in DefDatabase<GeneDef>.AllDefs)
			{
				if (geneDef.displayCategory == GeneCategory_SayersFurLength)
				{
					ExploritePatches.all_SayersFurLength.Add(geneDef);
				}
				else if (geneDef.displayCategory == GeneCategory_SayersFlowers)
				{
					ExploritePatches.all_SayersFlowers.Add(geneDef);
				}
				else if (geneDef.displayCategory == GeneCategory_SayersAlbinism_Head)
				{
					ExploritePatches.all_SayersAlbinism_Head.Add(geneDef);
				}
				else if (geneDef.displayCategory == GeneCategory_SayersAlbinism_Body)
				{
					ExploritePatches.all_SayersAlbinism_Body.Add(geneDef);
				}
				else if (geneDef.displayCategory == GeneCategory_SayersPattern)
				{
					ExploritePatches.all_SayersPattern.Add(geneDef);
				}
			}
		}

		public static List<GeneDef> getSayersGenes(GeneCategoryDef category)
		{
			if (category == GeneCategory_SayersFurLength)
			{
				return ExploritePatches.all_SayersFurLength;
			}
			else if (category == GeneCategory_SayersFlowers)
			{
				return ExploritePatches.all_SayersFlowers;
			}
			else if (category == GeneCategory_SayersAlbinism_Head)
			{
				return ExploritePatches.all_SayersAlbinism_Head;
			}
			else if (category == GeneCategory_SayersAlbinism_Body)
			{
				return ExploritePatches.all_SayersAlbinism_Body;
			}
			else if (category == GeneCategory_SayersPattern)
			{
				return ExploritePatches.all_SayersPattern;
			}
			return ExploritePatches.all_SayersFurLength;
		}

		public static GeneDef getRandomSayersGene(GeneCategoryDef category)
        {
			return GenCollection.RandomElement(getSayersGenes(category));
        }

		internal static bool GenerateSayersPostprocess(ref Pawn pawn, PawnGenerationRequest request, ref bool matchError)
		{
			// 确认目标是Sayers
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
			//限制Sayers的生物年龄是1到3岁，且实际年龄等于生物年龄
			pawn.ageTracker.AgeBiologicalTicks = Clamp(pawn.ageTracker.AgeBiologicalTicks / 10, 3600000, 10800000);
			pawn.ageTracker.AgeChronologicalTicks = pawn.ageTracker.AgeBiologicalTicks;
			//确认Sayers命名规范
			if (pawn.Name is NameTriple name)
			{
				//__result.Name = new NameTriple(name.Last, name.Last, null);
				//string nameFirst = PawnNameDatabaseShuffled.BankOf(PawnNameCategory.HumanStandard).GetName(PawnNameSlot.First, Rand.Bool ? Gender.Female : Gender.Male);

				GrammarRequest firstNameRequest = default(GrammarRequest);
				firstNameRequest.Includes.Add(RulePack_SayersFirstName);
				string nameFirst = GrammarResolver.Resolve("name", firstNameRequest);

				GrammarRequest nickNameRequest = default(GrammarRequest);
				nickNameRequest.Includes.Add(RulePack_SayersNickName);
				string nameNick = GrammarResolver.Resolve("nickname", nickNameRequest);
				pawn.Name = new NameTriple(nameFirst, nameNick, "Sayers");
			}
			//生成时基因随机抽取
			allSayersGenes();
			pawn.genes.AddGene(getRandomSayersGene(GeneCategory_SayersAlbinism_Body), false);
			pawn.genes.AddGene(getRandomSayersGene(GeneCategory_SayersAlbinism_Head), false);
			pawn.genes.AddGene(getRandomSayersGene(GeneCategory_SayersFurLength), false);
			pawn.genes.AddGene(getRandomSayersGene(GeneCategory_SayersFlowers), false);
			pawn.genes.AddGene(getRandomSayersGene(GeneCategory_SayersPattern), false);
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

		public static List<GeneDef> all_SayersFurLength = new List<GeneDef>();
		public static List<GeneDef> all_SayersFlowers = new List<GeneDef>();
		public static List<GeneDef> all_SayersAlbinism_Head = new List<GeneDef>();
		public static List<GeneDef> all_SayersAlbinism_Body = new List<GeneDef>();
		public static List<GeneDef> all_SayersPattern = new List<GeneDef>();
	}
}
