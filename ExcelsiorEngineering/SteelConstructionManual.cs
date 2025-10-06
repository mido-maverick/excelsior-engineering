using UnitsNet;

namespace ExcelsiorEngineering;

public static class SteelConstructionManual
{
    public static class BeamFormulas
    {
        public static class SimpleBeam
        {
            public static class UniformlyDistributedLoad
            {
                public static Force Vmax(ForcePerLength w, Length l) => (w * l) / 2;
                public static Torque Mmax(ForcePerLength w, Length l) => (w * (l * l)) / 8;
                public static Length Δmax(ForcePerLength w, Length l, Pressure E, AreaMomentOfInertia I) => throw new NotImplementedException();
            }
        }

        public static class CantileverBeam
        {
            public static class UniformlyDistributedLoad
            {
                public static Force Vmax(ForcePerLength w, Length l) => w * l;
                public static Torque Mmax(ForcePerLength w, Length l) => (w * (l * l)) / 2;
                public static Length Δmax(ForcePerLength w, Length l, Pressure E, AreaMomentOfInertia I) => throw new NotImplementedException();
            }
        }

        public static class ContinuousBeam
        {
            public static class ThreeEqualSpans
            {
                public static class AllSpansLoaded
                {
                    public static Force Vmax(ForcePerLength w, Length l) => 0.6 * (w * l);
                    public static Torque Mmax(ForcePerLength w, Length l) => 0.1 * (w * (l * l));
                    public static Length Δmax(ForcePerLength w, Length l, Pressure E, AreaMomentOfInertia I) => throw new NotImplementedException();
                }
            }
        }
    }
}
