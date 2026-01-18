using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using UnityEngine;

namespace SayersRemake
{
    public static partial class SayersRemakeBase
    {
		//惨绝人寰的尖叫
		private static readonly Dictionary<Type, Def> malrefs = new Dictionary<Type, Def>();
		private static T GetModDef<T>(bool willAttempt, string defName, bool errorOnFail = true) where T : Def, new()
		{
			if (willAttempt)
			{
				return DefDatabase<T>.GetNamed(defName, errorOnFail);
			}
			else
			{
				return GetModDef<T>();
			}
		}
		private static T GetModDef<T>(bool _) where T : Def, new()
		{
			return GetModDef<T>();
		}
		private static T GetModDef<T>() where T : Def, new()
		{
			Type t = typeof(T);
			if (malrefs.TryGetValue(t, out Def value))
			{
				if ((value as T) != null)
				{
					return value as T;
				}
			}

			T def = new T
			{
				defName = "EXPLO_MALREF",
				label = "EXPLO_MALREF",
				description = $"Encountered bad reference of {nameof(T)}."
			};
			malrefs.Add(t, def);
			return def;
		}

		public static bool IsNonMal(this Def def)
		{
			if (def == null || malrefs.Any(kvp => kvp.Value == def))
				return false;
			else
				return true;
		}
		//等等 不是 这是什么？
		public static readonly Harmony harmonyInstance = new Harmony(id: "Explorite.rimworld.mod.HarmonyPatches");

		public static readonly ThingDef AlienSayersDef = GetModDef<ThingDef>(InstelledMods.Sayers, "Alien_Sayers");
		public static readonly PawnKindDef PawnkindDef_SayersMember = GetModDef<PawnKindDef>(InstelledMods.Sayers, "SayersMember");
		public static readonly PawnKindDef PawnkindDef_WildSayers = GetModDef<PawnKindDef>(InstelledMods.Sayers, "WildSayers");

		public static readonly ThoughtDef ThoughtDef_SayersAbandonedSadness = GetModDef<ThoughtDef>(InstelledMods.Sayers, "AbandonedSadness_Sayers");

		public static readonly XenotypeDef XenotypeDef_Sayers_Prototype = GetModDef<XenotypeDef>(InstelledMods.Sayers, "Sayers_Prototype");
		public static readonly GeneCategoryDef GeneCategory_SayersAlbinism_Head = GetModDef<GeneCategoryDef>(InstelledMods.Sayers, "SayersAlbinism_Head");
		public static readonly GeneCategoryDef GeneCategory_SayersAlbinism_Body = GetModDef<GeneCategoryDef>(InstelledMods.Sayers, "SayersAlbinism_Body");
		public static readonly GeneCategoryDef GeneCategory_SayersPattern = GetModDef<GeneCategoryDef>(InstelledMods.Sayers, "SayersPattern");
		public static readonly GeneCategoryDef GeneCategory_SayersFurLength = GetModDef<GeneCategoryDef>(InstelledMods.Sayers, "SayersFurLength");
		public static readonly GeneCategoryDef GeneCategory_SayersFlowers = GetModDef<GeneCategoryDef>(InstelledMods.Sayers, "SayersFlowers");

		public static readonly BodyDef SayersBodyDef = GetModDef<BodyDef>(InstelledMods.Sayers, "Body_Sayers");
		public static readonly BodyPartTagDef SayersPlantTissueDef = GetModDef<BodyPartTagDef>(InstelledMods.Sayers, "SayersPlantTissue");
		public static readonly BodyPartDef BodyPart_SayersTentacles = GetModDef<BodyPartDef>(InstelledMods.Sayers, "Tentacles");
		public static readonly BodyPartDef BodyPart_SayersTentaclesBone = GetModDef<BodyPartDef>(InstelledMods.Sayers, "TentaclesBone");
		public static readonly BodyPartDef BodyPart_SayersTailFur = GetModDef<BodyPartDef>(InstelledMods.Sayers, "TailFur");
		public static readonly BodyPartDef BodyPart_SayersTailBone = GetModDef<BodyPartDef>(InstelledMods.Sayers, "TailBone");
		public static readonly BodyPartDef BodyPart_SayersBornthroatFlowers = GetModDef<BodyPartDef>(InstelledMods.Sayers, "BornthroatFlowers");
		public static readonly BodyPartDef BodyPart_SayersBorneyeFlowers = GetModDef<BodyPartDef>(InstelledMods.Sayers, "BorneyeFlowers");

		public static readonly BodyPartDef BodyPart_Jaw = GetModDef<BodyPartDef>(InstelledMods.Core, "Jaw");

		public static readonly RulePackDef RulePack_SayersNickName = GetModDef<RulePackDef>(InstelledMods.Sayers, "SayersNickName");
		public static readonly RulePackDef RulePack_SayersFirstName = GetModDef<RulePackDef>(InstelledMods.Sayers, "SayersFirstName");

		public static readonly ToolCapacityDef Capacity_Sayers_Injection = GetModDef<ToolCapacityDef>(InstelledMods.Sayers, "Sayers_Injection");

		public static readonly JobDef Job_HaulToJuicer_Sayers = GetModDef<JobDef>(InstelledMods.Sayers, "HaulToJuicer");
		
		public static readonly TraitDef trait_vigilant_Sayers = GetModDef<TraitDef>(InstelledMods.Sayers, "trait_vigilant_Sayers");


		public static readonly ThingCategoryDef EW_category = GetModDef<ThingCategoryDef>(InstelledMods.Sayers, "EW_category");

		public static readonly Texture2D StopPoisoningIcon = ContentFinder<Texture2D>.Get("UI/Commands/StopPoisoning", true);
	};
}
