using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using RevKitt.KS.KineticEnvironment.Effects.Order;
using RevKitt.KS.KineticEnvironment.Scenes;

namespace RevKitt.KS.KineticEnvironment.Effects.ColorEffect
{
    public class ImageEffect : IColorEffect
    {
        public const string EffectName = "ImageEffect";
        private string _imageName;
        private ImageRef _ref;

        public ImageEffect()
        {
            Colors = Enumerable.Empty<Color>().ToList();
        }

        public string ImageName
        {
            get { return _imageName; }
            set
            {
                _imageName = value;
                _ref = Images.GetImage(value);
                Colors = new List<Color> {_ref.GetValueAt(0, 0)};
            }
        }

        public string Name { get { return EffectName; } }
        public double Width = 1;
        public IOrdering Ordering { get; set; }

        public void SetColor(TimeRange range, double position, LEDNode led)
        {
            //get our position in space
            double posOffset = position*Width;
            //get our position in time
            double timeOffset = range.Portion*_ref.Width - Width;
            double xPosition = (posOffset + timeOffset)/_ref.Width;
            double yPosition = Ordering.GetAngle(led);

            led.Color = _ref.GetValueAt(xPosition,yPosition);
        }

        public IList<Color> Colors { get; set; } 
    }
}
