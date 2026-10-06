using System.Collections;
using UnityEngine;

public class OverworldState
{
    private WildernessState wildernessState;

    public WildernessState WildernessState => wildernessState;

    public OverworldState()
    {
        wildernessState = new WildernessState();
    }
}