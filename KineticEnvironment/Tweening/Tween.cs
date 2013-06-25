using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevKitt.KS.KineticEnvironment.Tweening
{
    public class Tween
    {
        private readonly EasingFunc _easing;
        private readonly string     _name;
        private readonly TweenType       _type;

        public Tween(IEasing easing, TweenType easingType = TweenType.In)
        {
            _name = easing.Name;
            _type = easingType;
            switch (easingType)
            {
                    case TweenType.In:
                    _easing = easing.In;
                    break;
                    case TweenType.Out:
                    _easing = easing.Out;
                    break;
                    case TweenType.OutIn:
                    _easing = easing.OutIn;
                    break;
                default:
                    throw new ArgumentException("easingType must be one of [Tween.TweenType.In, Tween.TweenType.Out, Tween.TweenType.OutIn]. Value: " + easingType);
            }
            StartTime = 0;
            EndTime = 1000;
            StartValue = 0;
            EndValue = 1;
        }

        public string Name
        {
            get { return _name; }
        }

        public TweenType Type
        {
            get { return _type; }
        }

        public bool Applies(int time)
        {
            return time >= StartTime && time <= EndTime;
        }

        public double GetValue(int time)
        {
            if (time < StartTime)
                time = StartTime;
            else if (time > EndTime)
                time = EndTime;

            double timeEntry = 1.0 * (time - StartTime)/(EndTime - StartTime);

            double pos = _easing(timeEntry);

            return pos*(EndValue - StartValue) + StartValue;
        }

        public void Validate()
        {
            if(StartTime >= EndTime)
                throw new ArgumentException("StartTime("+StartTime+") must be before EndTime("+EndTime+").");
        }


        public int StartTime { get; set; }
        public int EndTime { get; set; }

        public double StartValue { get; set; }
        public double EndValue { get; set; }
    }

    public enum TweenType {In, Out, OutIn}
}
