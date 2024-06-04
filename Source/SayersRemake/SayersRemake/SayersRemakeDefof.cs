using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

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
		public static readonly BodyDef SayersBodyDef = GetModDef<BodyDef>(InstelledMods.Sayers, "Body_Sayers");
	};
}
