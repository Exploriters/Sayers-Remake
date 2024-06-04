using Verse;
using System;
using UnityEngine;
using RimWorld;
using HarmonyLib;
using System.Linq;
using System.Reflection;
using static SayersRemake.SayersRemakeBase;

namespace SayersRemake
{
    public class InteractionWorker_Chitchat_Sayers : InteractionWorker
    {
		public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
		{
			if (initiator.Inhumanized() || initiator.def != AlienSayersDef || initiator.kindDef.race != AlienSayersDef)
			{
				return 0f;
			}
			return 1f;
		}
	}
}
