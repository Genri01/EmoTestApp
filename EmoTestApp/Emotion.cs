using System;

namespace EmoTestApp
{
    public class Emotion
    { 
        public Guid OriginalId { get; set; }
        public int EmotionStartTime { get; set; }
        public int EmotionType { get; set; }
        public int Channel { get; set; }
        public int Duration { get; set; }
        public int Confidence { get; set; }
    }
}
