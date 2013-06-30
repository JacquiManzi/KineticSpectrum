using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevKitt.KS.KineticEnvironment.Tweening
{
    public class Easings
    {

        public static IEasing GetEasingForName(string name)
        {
            return _easingNameMap[name];
        }

        public static bool IsEasingNameValid(string name)
        {
            return _easingNameMap.ContainsKey(name);
        }

        public static IDictionary<string, IEasing> EasingNameMap
        {
            get { return _easingNameMap; }
        }

        public static IEasing Linear = new LinearEasing();

        private class LinearEasing : IEasing
        {
            public string Name { get { return "Linear"; } }
            public EasingFunc In { get { return k => k; } }
            public EasingFunc Out { get { return k => k; } }
            public EasingFunc OutIn { get { return k => k; } }
        }

        
        public static IEasing Quadratic = new QuadraticEasing();

        private class QuadraticEasing : IEasing
        {
            public string Name { get { return "Quadratic"; } }

            public EasingFunc In
            {
                get { return i => i*i; }
            }

            public EasingFunc Out
            {
                get { return k => k*(2 - k); }
            }

            public EasingFunc OutIn
            {
                get { return k => ((k *= 2) < 1) ? 0.5*k*k : - 0.5*(--k*(k - 2) - 1); }
            }
        }

        public static IEasing Cubic = new CubicEasing();
        private class CubicEasing : IEasing
        {
            public string Name { get { return "Cubic"; } }

            public EasingFunc In
            {
                get { return k => k*k*k; }
            }

            public EasingFunc Out
            {
			    get { return k => --k*k*k + 1; }
            }

		    public EasingFunc OutIn
            {
			    get
			    {
			        return k =>
			                   {
			                       if ((k *= 2) < 1) 
                                       return 0.5*k*k*k;
			                       return 0.5*((k -= 2)*k*k + 2);
			                   };
			    }
		    }
	    }

        public static IEasing Quartic = new QuarticEasing();

        private class QuarticEasing : IEasing
        {
            public string Name { get { return "Quartic"; } }

            public EasingFunc In
            {
                get { return k => k*k*k*k; }
            }

            public EasingFunc Out
            {
                get { return k => 1 - (--k*k*k*k); }
            }

            public EasingFunc OutIn
            {
                get
                {
                    return k =>
                               {
                                   if ((k *= 2) < 1)
                                       return 0.5*k*k*k*k;
                                   return - 0.5*((k -= 2)*k*k*k - 2);
                               };
                }
            }
        }

        public static IEasing Quintic = new QuinticEasing();

        private class QuinticEasing : IEasing
        {
            public string Name { get { return "Quintic"; } }

            public EasingFunc In
            {
                get { return k => k*k*k*k*k; }
            }

            public EasingFunc Out
            {
                get { return k => --k*k*k*k*k + 1; }
            }

            public EasingFunc OutIn
            {
                get
                {
                    return k =>
                               {
                                   if ((k *= 2) < 1)
                                       return 0.5*k*k*k*k*k;
                                   return 0.5*((k -= 2)*k*k*k*k + 2);
                               };
                }
            }
        }


        public static IEasing Sinusoidal = new SinusoidalEasing();

        private class SinusoidalEasing : IEasing
        {
            public string Name { get { return "Sinusoidal"; } }

		    public EasingFunc In
            {
                get { return k => 1 - Math.Cos(k*Math.PI/2); }
            }

		    public EasingFunc Out
            {
                get { return k => Math.Sin(k*Math.PI/2); }
            }

		    public EasingFunc OutIn
            {
                get { return k => 0.5*(1 - Math.Cos(Math.PI*k)); }
            }
	    }


        public static IEasing Exponential = new ExponentialEasing(); 
        
	    private class ExponentialEasing : IEasing
        {
            public string Name { get { return "Exponential"; } }

		    public EasingFunc In
            {
                get { return k => k == 0 ? 0 : Math.Pow(1024, k - 1); }
            }

		    public EasingFunc Out
            {
                get { return k => k == 1 ? 1 : 1 - Math.Pow(2, - 10*k); }
            }

		    public EasingFunc OutIn
            {
                get
                {
                    return k =>
                               {
                                   if (k == 0) return 0;
                                   if (k == 1) return 1;
                                   if ((k *= 2) < 1) return 0.5*Math.Pow(1024, k - 1);
                                   return 0.5*(- Math.Pow(2, - 10*(k - 1)) + 2);
                               };
                }
            }
	    }


        public static IEasing Circular = new CircularEasing();

	    private class CircularEasing : IEasing 
        {
            public string Name { get { return "Circular"; } }

		    public EasingFunc In
            {
                get { return k => 1 - Math.Sqrt(1 - k*k); }
            }

		    public EasingFunc Out
            {
                get { return k => Math.Sqrt(1 - (--k*k)); }
            }

		    public EasingFunc OutIn
            {
                get
                {
                    return k =>
                               {
                                   if ((k *= 2) < 1) 
                                       return - 0.5*(Math.Sqrt(1 - k*k) - 1);
                                   return 0.5*(Math.Sqrt(1 - (k -= 2)*k) + 1);
                               };
                }
            }
	    }

//	    private class ElasticEasing : IEasing 
//        {
//		    public EasingFunc In
//            {
//                get
//                {
//                    return k =>
//                               {
//                                   double s, a = 0.1, p = 0.4;
//                                   if (k == 0) return 0;
//                                   if (k == 1) return 1;
//                                   if (!a || a < 1)
//                                   {
//                                       a = 1;
//                                       s = p/4;
//                                   }
//                                   else s = p*Math.Asin(1/a)/(2*Math.PI);
//                                   return - (a*Math.Pow(2, 10*(k -= 1))*Math.Sin((k - s)*(2*Math.PI)/p));
//                               };}
//
//                },
//
//		    Out: function ( k ) {
//
//			    var s, a = 0.1, p = 0.4;
//			    if ( k === 0 ) return 0;
//			    if ( k === 1 ) return 1;
//			    if ( !a || a < 1 ) { a = 1; s = p / 4; }
//			    else s = p * Math.asin( 1 / a ) / ( 2 * Math.PI );
//			    return ( a * Math.pow( 2, - 10 * k) * Math.sin( ( k - s ) * ( 2 * Math.PI ) / p ) + 1 );
//
//		    },
//
//		    InOut: function ( k ) {
//
//			    var s, a = 0.1, p = 0.4;
//			    if ( k === 0 ) return 0;
//			    if ( k === 1 ) return 1;
//			    if ( !a || a < 1 ) { a = 1; s = p / 4; }
//			    else s = p * Math.asin( 1 / a ) / ( 2 * Math.PI );
//			    if ( ( k *= 2 ) < 1 ) return - 0.5 * ( a * Math.pow( 2, 10 * ( k -= 1 ) ) * Math.sin( ( k - s ) * ( 2 * Math.PI ) / p ) );
//			    return a * Math.pow( 2, -10 * ( k -= 1 ) ) * Math.sin( ( k - s ) * ( 2 * Math.PI ) / p ) * 0.5 + 1;
//
//		    }
//
//	    },
        public static IEasing Back = new BackEasing();
	    private class BackEasing : IEasing
        {
            public string Name { get { return "Back"; } }

		    public EasingFunc In
            {
                get
                {
                    return k =>
                               {
                                   double s = 1.70158;
                                   return k*k*((s + 1)*k - s);
                               };
                }
            }

		    public EasingFunc Out
            {
                get
                {
                    return k =>
                               {
                                   var s = 1.70158;
                                   return --k*k*((s + 1)*k + s) + 1;
                               };
                }
            }

		    public EasingFunc OutIn
            {
                get
                {
                    return k =>
                               {
                                   double s = 1.70158*1.525;
                                   if ((k *= 2) < 1) return 0.5*(k*k*((s + 1)*k - s));
                                   return 0.5*((k -= 2)*k*((s + 1)*k + s) + 2);
                               };
                }
            }
	    }

        public static IEasing Bounce = new BounceEasing();
	    private class BounceEasing : IEasing
        {
            public string Name { get { return "Bounce"; } }

		    public EasingFunc In
            {
                get { return k => 1 - Out(1 - k); }
            }

		    public EasingFunc Out
            {
                get
                {
                    return k =>
                               {
                                   if (k < (1/2.75))
                                   {
                                       return 7.5625*k*k;
                                   }
                                   if (k < (2/2.75))
                                   {
                                       return 7.5625*(k -= (1.5/2.75))*k + 0.75;
                                   }
                                   if (k < (2.5/2.75))
                                   {
                                       return 7.5625*(k -= (2.25/2.75))*k + 0.9375;
                                   }
                                   return 7.5625*(k -= (2.625/2.75))*k + 0.984375;
                                   
                               };
                }
            }

		    public EasingFunc OutIn
            {
                get
                {
                    return k =>
                               {
                                   if (k < 0.5) return In(k*2)*0.5;
                                   return Out(k*2 - 1)*0.5 + 0.5;
                               };
                }
            }
	    }

        private static readonly IDictionary<string, IEasing> _easingNameMap = new Dictionary<string, IEasing>
        {{Linear.Name, Linear},{Quadratic.Name,Quadratic}, {Cubic.Name, Cubic}, {Quartic.Name,Quartic},{Quintic.Name, Quintic},
         {Sinusoidal.Name, Sinusoidal}, {Exponential.Name, Exponential}, {Circular.Name, Circular}, {Back.Name, Back}, 
         {Bounce.Name, Bounce}}; 
    }
}
