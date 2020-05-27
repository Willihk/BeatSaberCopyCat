﻿using UnityEngine;
using System.Collections;

public enum CutDirection
{
    Upwards = 0,
    Downwards = 1,
    TowardsLeft = 2,
    TowardsRight = 3,

    // Rotated
    TowardsTopLeft = 4,
    TowardsTopRight = 5,
    TowardsBottomLeft = 6,
    TowardsBottomRight = 7,

    Any = 8,
}
