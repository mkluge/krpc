using KRPC.Service.Attributes;
using KRPC.Schema.Orbit;

namespace KRPCServices
{
    [KRPCService]
    static public class Orbit
    {
        [KRPCProcedure]
        public static OrbitData GetOrbitData ()
        {
            var orbit = FlightGlobals.ActiveVessel.GetOrbit ();
            return OrbitData.CreateBuilder ()
                .SetApoapsis (orbit.ApR)
                .SetPeriapsis (orbit.PeR)
                .SetEccentricity (orbit.eccentricity)
                .SetInclination (orbit.inclination)
                .SetLongitudeOfAscendingNode (orbit.LAN)
                .SetArgumentOfPeriapsis (orbit.argumentOfPeriapsis)
                .SetMeanAnomalyAtEpoch (orbit.meanAnomalyAtEpoch)
                .SetReferenceBody (orbit.referenceBody.name)
                .Build ();
        }
    }
}
