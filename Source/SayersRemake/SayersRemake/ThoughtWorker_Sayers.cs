using Verse;
using RimWorld;
using static SayersRemake.SayersRemakeBase;

namespace SayersRemake
{
	/// <summary>猫猫和鹿狐之间始终存在正面评价。</summary>
    public class ThoughtWorker_AlwaysPositive_DeerFox_Sayers : ThoughtWorker
	{
		protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn otherPawn)
		{
			if (InstelledMods.DeerFox && otherPawn.def == DefDatabase<AlienRace.ThingDef_AlienRace>.GetNamed("DeerFox_Race"))
			{
				return true;
			}
			return false;
		}
    }
	/// <summary>猫猫之间始终存在正面评价。(来自半人马！)</summary>
    public class ThoughtWorker_AlwaysPositive_Sayers : ThoughtWorker
    {
		protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn otherPawn)
		{
			if (p != otherPawn && p.def == AlienSayersDef && otherPawn.def == AlienSayersDef)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
	/// <summary>猫猫与非同族之间始终存在负面评价。</summary>
	public class ThoughtWorker_AlwaysNegative_Sayers : ThoughtWorker
    {
		protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn otherPawn)
		{
			if (p != otherPawn && p.def == AlienSayersDef && otherPawn.def != AlienSayersDef)
			{
				if (LovePartnerRelationUtility.LovePartnerRelationExists(p, otherPawn))
                {
					return false;
                }
                if (p.story.traits.HasTrait(trait_vigilant_Sayers, 1))
                {
					return false;
                }
				// 鹿狐除外
				if (InstelledMods.DeerFox && otherPawn.def == DefDatabase<AlienRace.ThingDef_AlienRace>.GetNamed("DeerFox_Race"))
                {
					return false;
                }
				return true;
			}
			else
			{
				return false;
			}
		}
	}
	/// <summary>猫猫在没有同阵营同族时会获得心情惩罚</summary>
	public sealed class ThoughtWorker_SayersWillFeelLonely : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			if (p.def == AlienSayersDef && !(p?.Map?.mapPawns?.AllPawns?.Any(pawn => pawn != p && (pawn.Faction == null || pawn.Faction == p.Faction) && pawn.def == AlienSayersDef) ?? false))
			{
				return true;
			}
			else
			{
				return false;
			}
        }
	}

	/// <summary>科塔尔猫猫的心情惩罚</summary>
	public class ThoughtWorker_CotardSyndrome : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			if(hasTrait(p, "Trait_CotardSyndrome"))
            {
				return true;
            }
			return false;
		}
	}

    /// <summary>患有自闭症/科塔尔的猫猫会受到他人的负面评价</summary>
    public class ThoughtWorker_Other_Autism : ThoughtWorker
	{
		protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn otherPawn)
		{
			if (p != otherPawn && ((hasTrait(otherPawn, "Trait_Autism") || hasTrait(otherPawn, "Trait_CotardSyndrome"))))
			{
				return true;
            }
			return false;
		}
	}
}
