using System;

namespace Kalimag.Modding.Unity.Visualization
{
	[Flags]
    public enum ColliderTypes
    {
        None = 0,
        Trigger = 1,
        Collision = 2,
        Both = Collision | Trigger
    }
}
