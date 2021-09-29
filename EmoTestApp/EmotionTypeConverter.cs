using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmoTestApp
{
    class EmotionTypeConverter
    {
        internal static EmotionType GetTypeEmotion(string emotionName)
        {
            switch (emotionName.ToLower())
            {
                case "anger":
                    return EmotionType.Anger;

                case "saddness":
                case "sadness":
                case "monoton":
                    // as confirmed by RTK we may take saddness as monotonous speech
                    return EmotionType.Monotonous;

                case "neutrality":
                    return EmotionType.Neutrality;

                case "happiness":
                    return EmotionType.Happiness;

                default:
                    return EmotionType.Unknown;
            }
        }
    }
}
