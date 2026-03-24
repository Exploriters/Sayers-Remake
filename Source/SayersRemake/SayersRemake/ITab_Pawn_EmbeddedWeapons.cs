using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;
using RimWorld.SketchGen;
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
			Rect rect = new Rect(0f, 0f, this.size.x, this.size.y).ContractedBy(10f);
			Listing_Standard listing = new Listing_Standard();
			listing.Begin(rect);
			Text.Font = GameFont.Medium;
			listing.Label("系统维护中！");
			listing.End();
			/*weapons = Pawn.TryGetComp<CompEmbeddedWeaponStore>().getInfo();
			BodyPartRecord ThroatRecord = Pawn.health.hediffSet.GetBodyPartRecord(BodyPart_SayersBornthroatFlowers);
			BodyPartRecord JawRecord = Pawn.health.hediffSet.GetBodyPartRecord(BodyPart_Jaw);
			List<BodyPartRecord> embedableParts = new List<BodyPartRecord>();
			embedableParts.Add(JawRecord);
			foreach (BodyPartRecord TR in ThroatRecord.parts)
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
			Widgets.BeginScrollView(rect, ref this.scrollPosition, viewRect);
			float curY = 50f;
			foreach (BodyPartRecord EPR in embedableParts)
			{
				Text.Font = GameFont.Small;
				GUI.color = Color.gray;
				Widgets.ListSeparator(ref curY, viewRect.width, "部位：" + EPR.Label);
				Rect rect1 = new Rect(0f, curY, viewRect.width, 22f);
				Rect rect2 = new Rect(0f, curY + 22f, viewRect.width, 22f);
				Rect rect3 = new Rect(0f, curY + 44f, viewRect.width / 2, 22f);
				String curWeapon = "此部位暂无嵌入式武器";
				String curInfo = "暂无信息";
				foreach (weaponInfo weapon in weapons)
				{
					if (weapon.EPlabel == EPR.untranslatedCustomLabel)
					{
						GUI.color = Color.white;
						curWeapon = "已安装：" + weapon.weapon.Label;
						CompEmbeddedWeapon comp = weapon.weapon.TryGetComp<CompEmbeddedWeapon>();
						if (comp != null)
						{
							curInfo = "攻击加成：" + comp.Props.damage;
                        }
					}
				}
				GUI.color = Color.white;
				Widgets.Label(rect1, curWeapon);
				Text.Font = GameFont.Tiny;
				Widgets.Label(rect2, curInfo);
				if (Widgets.ButtonText(rect3, "管理"))
				{
					List<FloatMenuOption> options = new List<FloatMenuOption>();
					options.Add(new FloatMenuOption("卸载", () => uninstallWeapon(EPR), MenuOptionPriority.Default));
					foreach (Thing thing in Pawn.inventory.innerContainer)
					{
						if (thing.HasThingCategory(EW_category) && thing.TryGetComp<CompEmbeddedWeapon>() != null && thing.TryGetComp<CompEmbeddedWeapon>().Props.installableParts.Contains(EPR.def))
						{
							options.Add(new FloatMenuOption(thing.LabelCap, () => installWeapon(EPR, thing), MenuOptionPriority.Default));
						}
					}
					Find.WindowStack.Add(new FloatMenu(options));
				}
				curY += 66f;
				scrollHeight = curY;
			}

			listing.End();
			Widgets.EndScrollView();*/
		}

		/*public void uninstallWeapon(BodyPartRecord EPR)
		{
			foreach (weaponInfo weapon in weapons)
			{
				if (weapon.EPlabel == EPR.untranslatedCustomLabel)
				{
					Thing w = weapon.weapon;
					w.stackCount = 1;
					GenSpawn.Spawn(w, Pawn.Position, Pawn.Map);
					weapons.Remove(weapon);
					break;
				}
			}
		}
		public void installWeapon(BodyPartRecord EPR, Thing thing)
		{
			foreach (weaponInfo weapon in weapons)
			{
				if (weapon.EPlabel == EPR.untranslatedCustomLabel)
				{
					Thing w = weapon.weapon;
					w.stackCount = 1;
					GenSpawn.Spawn(w, Pawn.Position, Pawn.Map);
					weapons.Remove(weapon);
					break;
				}
			}
			weaponInfo newInfo = new weaponInfo()
			{
				weapon = thing,
				EPlabel = EPR.untranslatedCustomLabel
			};
			weapons.Add(newInfo);
			Pawn?.inventory?.innerContainer.Remove(thing);
			Pawn.TryGetComp<CompEmbeddedWeaponStore>().updateInfo(weapons);
			//Pawn.inventory.RemoveCount(thing.def, 1);
		}
		public List<weaponInfo> weapons = new List<weaponInfo>();
		private Vector2 scrollPosition = new Vector2(0f, 50f);
		private float scrollHeight = 0f;*/
	}

	public class CompProperties_CompEmbeddedWeaponStore : CompProperties
	{
		public CompProperties_CompEmbeddedWeaponStore()
		{
			compClass = typeof(CompEmbeddedWeaponStore);
		}
	}
	public class CompEmbeddedWeaponStore : ThingComp
    {
		/*public List<weaponInfo> weaponInfos = new List<weaponInfo>();

		public void updateInfo(List<weaponInfo> info)
		{
			weaponInfos = info;
			Log.Warning("[UPDATE]" + String.Join(",", weaponInfos));
		}
		public List<weaponInfo> getInfo()
		{
			Log.Warning("[GET]" + String.Join(",", weaponInfos));
			return weaponInfos;
		}
        public override void PostExposeData()
        {
            base.PostExposeData();
			List<Thing> weaponInfos_Things = new List<Thing>();
			List<string> weaponInfos_Parts = new List<string>();
			if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
				if(weaponInfos == null)
                {
					weaponInfos = new List<weaponInfo>();
					return;
				}
				Scribe_Collections.Look(ref weaponInfos_Things, "weaponInfos_Things", LookMode.Deep, new List<Thing>());
				Scribe_Collections.Look(ref weaponInfos_Parts, "weaponInfos_Parts", LookMode.Value, new List<string>());
				for (int i = 0; i < weaponInfos_Things.Count; i++)
				{
					weaponInfos.Add(new weaponInfo() { weapon = weaponInfos_Things[i], EPlabel = weaponInfos_Parts[i] });
				}
				Log.Warning("[EXPOSE]=== Now Loading ===");
				Log.Warning("[EXPOSE]" + String.Join(",", weaponInfos_Things));
				Log.Warning("[EXPOSE]" + String.Join(",", weaponInfos_Parts));
				Log.Warning("[EXPOSE]" + String.Join(",", weaponInfos));
			}
			else if(Scribe.mode == LoadSaveMode.Saving)
            {
				weaponInfos_Things = new List<Thing>();
				weaponInfos_Parts = new List<string>();
				foreach (weaponInfo info in weaponInfos)
                {
					weaponInfos_Things.Add(info.weapon);
					weaponInfos_Parts.Add(info.EPlabel);
				}
				Scribe_Collections.Look(ref weaponInfos_Things, "weaponInfos_Things", LookMode.Deep);
				Scribe_Collections.Look(ref weaponInfos_Parts, "weaponInfos_Parts", LookMode.Value);
				Log.Warning("[EXPOSE]=== Now Saving ===");
				Log.Warning("[EXPOSE]" + String.Join(",", weaponInfos_Things));
				Log.Warning("[EXPOSE]" + String.Join(",", weaponInfos_Parts));
				Log.Warning("[EXPOSE]" + String.Join(",", weaponInfos));
			}
        }*/
	}
	public class CompProperties_CompEmbeddedWeapon : CompProperties
	{
		public float damage;
		public List<BodyPartDef> installableParts;
		public List<StatModifier> statBases;
		public CompProperties_CompEmbeddedWeapon()
		{
			compClass = typeof(CompEmbeddedWeapon);
		}
	}
	public class CompEmbeddedWeapon : ThingComp
	{
		/*public CompProperties_CompEmbeddedWeapon Props => (CompProperties_CompEmbeddedWeapon)this.props;
		private float damage;
		private List<BodyPartDef> installableParts;
		private List<StatModifier> statBases;

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
			damage = Props.damage;
			installableParts = Props.installableParts;
			statBases = Props.statBases;
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
			Scribe_Values.Look(ref damage, "damage", 0f);
			Scribe_Collections.Look(ref installableParts, "installableParts", LookMode.BodyPart, new List<BodyPartDef>());
			Scribe_Collections.Look(ref statBases, "statBases", LookMode.Value, new List<StatModifier>());
		}*/
	}
	/*public struct weaponInfo : IExposable
	{
		public Thing weapon;
		public String EPlabel;
		public weaponInfo(Thing weapon, String EPlabel)
        {
			this.weapon = weapon;
			this.EPlabel = EPlabel;
        }
		public void ExposeData()
		{
			Scribe_References.Look(ref weapon, "weapon");
			Scribe_Deep.Look(ref EPlabel, "EPlabel");
		}
	};*/
}