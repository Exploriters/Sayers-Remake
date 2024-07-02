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
			if (initiator.relations.OpinionOf(recipient) < -20)
			{
				return 0f;
			}
			return 1f;
		}
	};

	public class InteractionWorker_DeepTalk_Sayers : InteractionWorker
    {
		public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
			if (initiator.Inhumanized() || initiator.def != AlienSayersDef || initiator.kindDef.race != AlienSayersDef)
			{
				return 0f;
			}
			if(initiator.relations.OpinionOf(recipient) < 0)
            {
				return 0f;
            }
			if((recipient.def == AlienSayersDef && initiator.def == AlienSayersDef) || LovePartnerRelationUtility.LovePartnerRelationExists(initiator, recipient))
            {
				return 1.25f;
			}
			return 0.5f;
		}
    };
}
