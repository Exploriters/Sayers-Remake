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

	public class InteractionWorker_Insult_Sayers : InteractionWorker
    {
		public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
		{
			if (initiator.Inhumanized() || initiator.def != AlienSayersDef || initiator.kindDef.race != AlienSayersDef)
			{
				return 0f;
			}
			if (recipient.def == AlienSayersDef)
            {
				if(initiator.relations.OpinionOf(recipient) > 30)
                {
					return 0f;
                }
				return 0.0023f * NegativeInteractionUtility.NegativeInteractionChanceFactor(initiator, recipient);
            }
			if(initiator.Position.DistanceTo(recipient.Position) < 5f)
            {
				if(initiator.story.traits.HasTrait(trait_vigilant_Sayers, 1) && initiator.relations.OpinionOf(recipient) > -50)
                {
					return 0f;
				}
				if(initiator.story.traits.HasTrait(trait_vigilant_Sayers, -1))
                {
					if(initiator.relations.OpinionOf(recipient) > 30)
                    {
						return 0f;
					}
                    else
                    {
						return 0.01f * NegativeInteractionUtility.NegativeInteractionChanceFactor(initiator, recipient);
					}
				}
				if (initiator.relations.OpinionOf(recipient) > -30)
				{
					return 0f;
				}
				return 0.005f * NegativeInteractionUtility.NegativeInteractionChanceFactor(initiator, recipient);
			}
			return 0f;
		}

		private const float BaseSelectionWeight = 0.005f;
	}
}
