using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace TransparentHandledAircraft;

internal class Utils
{
    public static bool IsBoundaryWarning(Aircraft aircraft)
        => aircraft.boundaryWarner.arrow.gameObject.activeInHierarchy;

    public static bool IsTCASWarning(Aircraft aircraft)
        => FastMemberAccessor<Aircraft, Dictionary<GameObject, VWIndicator>>.Get("TCASWarns", aircraft).Count > 0;
}
