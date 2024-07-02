using UnityEngine;
using Verse;
using static SayersRemake.SayersRemakeBase;
using System;

namespace SayersRemake
{
    public class ColorGenerator_SayersFlower : ColorGenerator
    {
        public override Color NewRandomizedColor()
        {
            float H = Rand.Value;
            float S = Rand.Value;
            float V = Rand.Value;
            return Color.HSVToRGB(H, S, V);
        }
    }
    public class ColorGenerator_SayersFur : ColorGenerator
    {
        //public override Color ExemplaryColor => new Color(0.85098039215686274509803921568627f, 0f, 0f);

        string colorType = "";
        float H = 0f;
        float S = 0f;
        float V = 0f;

        public override Color NewRandomizedColor()
        {
            float randomValueColorType = Rand.Value;
            int randomColorType = 1;
            if (randomValueColorType < 0.9f)
            {
                //Normal Fur Colors
                randomColorType = Rand.Range(1, 4);
            }
            else
            {
                //Special Fur Colors
                randomColorType = Rand.Range(4, 7);
            }
            switch (randomColorType)
            {
                case 1:
                    H = 0f;
                    S = 0f;
                    V = RandomFloatRange(0.08f, 1f);
                    colorType = "COLORLESS";
                    break;
                case 2:
                    H = RandomFloatRange(0.9139f, 1f);
                    S = RandomFloatRange(0.16f, 0.75f);
                    V = RandomFloatRange(0.16f, 0.9f);
                    colorType = "RED";
                    break;
                case 3:
                    H = RandomFloatRange(0.0778f, 0.1333f);
                    S = RandomFloatRange(0.23f, 0.75f);
                    V = RandomFloatRange(0.21f, 0.9f);
                    colorType = "YELLOW";
                    break;
                case 4:
                    H = RandomFloatRange(0.4806f, 0.6556f);
                    S = RandomFloatRange(0.13f, 0.75f);
                    V = RandomFloatRange(0.12f, 0.9f);
                    colorType = "BLUE";
                    break;
                case 5:
                    H = RandomFloatRange(0.7472f, 0.8056f);
                    S = RandomFloatRange(0.19f, 0.75f);
                    V = RandomFloatRange(0.26f, 0.9f);
                    colorType = "PURPLE";
                    break;
                case 6:
                    H = RandomFloatRange(0.3111f, 0.4f);
                    S = RandomFloatRange(0.33f, 0.75f);
                    V = RandomFloatRange(0.09f, 0.9f);
                    colorType = "GREEN";
                    break;
                default:
                    H = 1f;
                    S = 1f;
                    V = 1f;
                    colorType = "ERROR";
                    break;
            }
            //Log.Message("[SAYERS REMAKE]Sayers furColor:INT(" + randomColorType.ToString() + "),HSV(" + H.ToString() + "," + S.ToString() + "," + V.ToString() + "),TYPE(" + colorType + ")");
            return Color.HSVToRGB(H, S, V);
        }
    }
}
