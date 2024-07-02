using Verse;
using System;
using UnityEngine;
using RimWorld;
using HarmonyLib;
using System.Linq;
using System.Reflection;

namespace SayersRemake
{
    ///<summary>核心函数集，参考于siiftun1857的Explorite Core（等待更新……！）。</summary>
    public static partial class SayersRemakeBase
    {
        public static class InstelledMods
        {
            public static bool Sayers => ModLister.GetActiveModWithIdentifier("Exploriters.Abrel.Sayers.REMAKE") != null;
			public static bool Core => ModLister.GetActiveModWithIdentifier("Ludeon.RimWorld") != null;
			public static bool Royalty => ModLister.GetActiveModWithIdentifier("Ludeon.RimWorld.Royalty") != null;
			public static bool Ideology => ModLister.GetActiveModWithIdentifier("Ludeon.RimWorld.Ideology") != null;
			public static bool HAR => ModLister.GetActiveModWithIdentifier("erdelf.HumanoidAlienRaces") != null;
		};
		/**
	 * <summary>
	 * 限制值的取值范围。
	 * </summary>
	 * <param name="value">需要被限制在范围内的值。</param>
	 * <param name="min">最小值。</param>
	 * <param name="max">最大值。</param>
	 * <returns>处理后的值。</returns>
	 */
		public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
		{
			if (value.CompareTo(max) > 0)
				return max;
			if (value.CompareTo(min) < 0)
				return min;
			return value;
		}
		/**
		* <summary>
		 * 随机范围浮点数。
		 * </summary>
		 * <param name = "min" > 最小值。</param>
		 * <param name = "max" > 最大值。</param>
		 * <returns>随机数结果。</returns>
		 */
		public static float RandomFloatRange(float min, float max)
		{
			return Rand.Value * (max - min) + min;
		}
	};
}
