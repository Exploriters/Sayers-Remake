using Verse;
using RimWorld;
using static SayersRemake.SayersRemakeBase;

namespace SayersRemake
{
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
}
