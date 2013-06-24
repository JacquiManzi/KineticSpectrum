using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevKitt.KS.KineticEnvironment.Tweening
{
    class Tween
    {
        public enum Type {In, Out, OutIn}

        private readonly EasingFunc _easing;

        public Tween(IEasing easing, Type easingType = Type.In)
        {
            switch (easingType)
            {
                    case Type.In:
                    _easing = easing.In;
                    break;
                    case Type.Out:
                    _easing = easing.Out;
                    break;
                    case Type.OutIn:
                    _easing = easing.OutIn;
                    break;
                default:
                    throw new ArgumentException("easingType must be one of [Tween.Type.In, Tween.Type.Out, Tween.Type.OutIn]. Value: " + easingType);
            }
            StartTime = 0;
            EndTime = 1000;
            StartValue = 0;
            EndValue = 1;
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
}
