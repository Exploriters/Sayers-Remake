using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using static SayersRemake.SayersRemakeBase;

namespace SayersRemake
{
	public class CompProperties_AssignableToPawn_NoPostLoadSpecial : CompProperties_AssignableToPawn
	{
		public override void PostLoadSpecial(ThingDef parent) { }
	}

	///<summary>猫猫床，改自果果床。</summary>
	public class CompAssignableToPawn_Bed_Sayers : CompAssignableToPawn_Bed
	{
		public override AcceptanceReport CanAssignTo(Pawn pawn)
		{
			if (pawn.def != AlienSayersDef)
			{
				return "TooLargeForBed".Translate();
			}
			return base.CanAssignTo(pawn);
		}
	}
}
