using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SayersRemake.SayersRemakeBase;

namespace SayersRemake
{
    class ITab_Pawn_EmbeddedWeapons : ITab
    {
		private Pawn Pawn
		{
			get
			{
				if (SelPawn != null)
				{
					return SelPawn;
				}
				else if (SelThing is Corpse corpse)
				{
					return corpse.InnerPawn;
				}
				Log.Error("Character tab found no selected pawn to display.");
				return null;
			}
		}

		private CompPawnEmbeddedWeapons compStash = null;
		public CompPawnEmbeddedWeapons Comp => compStash ??= Pawn?.GetComp<CompPawnEmbeddedWeapons>();
        //public override bool IsVisible => Comp != null;
        public override bool IsVisible
        {
            get
            {
				return Comp != null;
			}
        }

		public ITab_Pawn_EmbeddedWeapons()
        {
			labelKey = "嵌入式武器";
			this.size = new Vector2(400f, 400f);
		}

		protected override void FillTab()
        {
			BodyPartRecord ThroatRecord = Pawn.health.hediffSet.GetBodyPartRecord(BodyPart_SayersBornthroatFlowers);
			BodyPartRecord JawRecord = Pawn.health.hediffSet.GetBodyPartRecord(BodyPart_Jaw);
			List<BodyPartRecord> embedableParts = new List<BodyPartRecord>();
			embedableParts.Add(JawRecord);
			foreach(BodyPartRecord TR in ThroatRecord.parts)
            {
				embedableParts.Add(TR);
			}

			Rect rect = new Rect(0f, 0f, this.size.x, this.size.y).ContractedBy(10f);
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(rect);
			Text.Font = GameFont.Medium;
			listing.Label("嵌入式武器管理");
			listing.GapLine();
			Text.Font = GameFont.Small;
			Rect viewRect = new Rect(0f, 22f, this.size.x - 16f, this.scrollHeight - 22f);
			Widgets.BeginScrollView(rect, ref this.scrollPosition, viewRect) ;
			float curY = 50f;
			foreach (BodyPartRecord EPR in embedableParts)
            {
				Text.Font = GameFont.Small;
				GUI.color = Color.gray;
				Widgets.ListSeparator(ref curY, viewRect.width, "部位：" + EPR.Label);
				GUI.color = Color.white;
				Rect rect1 = new Rect(0f, curY, viewRect.width, 22f);
				Rect rect2 = new Rect(0f, curY + 22f, viewRect.width, 22f);
				Rect rect3 = new Rect(0f, curY + 44f, viewRect.width / 2, 22f);
				Widgets.Label(rect1, "此部位暂无嵌入式武器");
				Text.Font = GameFont.Tiny;
				Widgets.Label(rect2, "暂无信息");
				if(Widgets.ButtonText(rect3, "管理"))
                {
					List<FloatMenuOption> options = new List<FloatMenuOption>();
					options.Add(new FloatMenuOption("卸载", () => uninstallWeapon(EPR), MenuOptionPriority.Default));
					foreach (Thing thing in Pawn.inventory.innerContainer)
					{
						if (thing.HasThingCategory(EW_category))
						{
							options.Add(new FloatMenuOption(thing.LabelCap, () => installWeapon(EPR, thing), MenuOptionPriority.Default));
						}
					}
					Find.WindowStack.Add(new FloatMenu(options));
				}
				curY += 66f;
				scrollHeight = curY;
				/*Text.Font = GameFont.Small;
				GUI.color = Color.gray;
				listing.Label("部位：" + EPR.Label);
				listing.GapLine();
				GUI.color = Color.white;
				listing.Label("此部位还没有被嵌入武器……");
				Text.Font = GameFont.Tiny;
				listing.Label("Placeholder info\nLine 2");*/
			}

			listing.End();
			Widgets.EndScrollView();
		}

		public void uninstallWeapon(BodyPartRecord EPR)
        {

        }
		public void installWeapon(BodyPartRecord EPR, Thing thing)
        {

        }

		private Vector2 scrollPosition = new Vector2(0f, 50f);
		private float scrollHeight = 0f;
	}
}