using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace SD.UI.Helpers
{
    // GridLengthAnimation (animates GridLength values - pixel only)
    public class GridLengthAnimation : AnimationTimeline
    {
        public GridLength From { get; set; }
        public GridLength To { get; set; }

        public override Type TargetPropertyType => typeof(GridLength);

        protected override Freezable CreateInstanceCore()
        {
            return this;
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            double fromVal = From.Value;
            double toVal = To.Value;
            if (animationClock.CurrentProgress == null)
                return new GridLength(fromVal, GridUnitType.Pixel);
            double progress = animationClock.CurrentProgress.Value;
            double current = ((toVal - fromVal) * progress) + fromVal;
            return new GridLength(current, GridUnitType.Pixel);
        }
    }
}
