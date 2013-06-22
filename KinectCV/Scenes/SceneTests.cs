using System;
using ILNumerics;
using NUnit.Framework;

namespace RevKitt.ks.KinectCV.Scenes
{
    [TestFixture]
    public class SceneTests
    {

        private static ILArray<double> PointSet1
        {
            get
            {
                return ILMath.array(new[]
                                        {
                                           -0.5505, -0.4606, -0.4065, -0.4298, -0.5675, -0.3004, -0.5672,
                                            2.0836,  1.6215,  0.9123,  1.9087,  1.3548,  1.2120,  2.3265,
                                            0.2539,  0.0343, -0.2519,  0.1594, -0.0574, -0.1610,  0.3698
                                        }, 7,3).T;
            }
        }
        private static ILArray<double> PointSet2
        {
            get
            {
                return ILMath.array(new[]
                                        {
                                            0.7687, 0.5250,  0.1285, 0.7174,  0.3059,  0.3624, 0.9072,
                                            1.9107, 1.5832,  1.1575, 1.6828,  1.5939,  1.1902, 2.0325,
                                            0.1873, 0.0052, -0.2079, 0.0773, -0.0102, -0.1862, 0.2693
                                        }, 7,3).T;
            }
        }

        [Test]
        public void TestMatrix()
        {
            ILArray<double> transform;
            ILArray<double> rotMatrix = Scene.CreateMatrix(PointSet2, PointSet1, out transform);

            for(int i=0; i<PointSet2.Size[1]; i++)
            {
                ILArray<double> v1 = PointSet1.Subarray(ILMath.full, i);
                ILArray<double> v2 = PointSet2.Subarray(ILMath.full, i);

                double distance = ILMath.distL1(v1, ILMath.multiply(rotMatrix, v2) + transform).GetValue(0);
                Assert.IsTrue(1.5 > distance, "Vector distance was: " + distance);
            }

        }
    }
}
