using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterCollisionFlags
{
    public bool right, left, above, below;

    public void Reset()
    {
        right = left = above = below = false;
    }
}

