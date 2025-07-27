using HarmonyLib;
using RimWorld;
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
using static SayersRemake.SayersRemakeBase;

namespace SayersRemake
{
	public class ScenPart_StartThoughtSadness : ScenPart
	{
		public override void PostGameStart()
		{
			base.PostGameStart();
			PostGameStartEffect();
		}

		public static void PostGameStartEffect()
		{
			foreach (List<Thought_Memory> memories in Find.GameInitData.startingAndOptionalPawns.Where(pawn => pawn.def == AlienSayersDef).Select(pawn => pawn.needs.mood.thoughts.memories.Memories))
			{
				//memories.RemoveAll(memory => memory.def == ThoughtDefOf.NewColonyOptimism);
				memories.Add((Thought_Memory)ThoughtMaker.MakeThought(ThoughtDef_SayersAbandonedSadness));
			}
		}
	}
}
