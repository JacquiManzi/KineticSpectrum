using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using NUnit.Framework;

namespace RevKitt.KS.KineticEnvironment
{
    [TestFixture]
    class ColorUtilTest
    {
        [Test]
        public void TestColorInt()
        {
            var redInt = ColorUtil.ToInt(Colors.Red);
            var redColor = ColorUtil.FromInt(redInt);
            Assert.AreEqual(Colors.Red, redColor);

            var greenInt = ColorUtil.ToInt(Colors.Green);
            var greenColor = ColorUtil.FromInt(greenInt);
            Assert.AreEqual(Colors.Green, greenColor);

            var blueInt = ColorUtil.ToInt(Colors.Blue);
            var blueColor = ColorUtil.FromInt(blueInt);
            Assert.AreEqual(Colors.Blue, blueColor);

            Assert.AreEqual(Colors.Aqua, ColorUtil.FromInt(ColorUtil.ToInt(Colors.Aqua)));

        }

    }
}
