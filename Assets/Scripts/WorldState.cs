using System.Collections.Generic;

public class WorldState
{
    private OverworldState overworldState;
    private UnderworldState underworldState;

    public OverworldState OverworldState => overworldState;
    public UnderworldState UnderworldState => underworldState;

    public WorldState()
    {
        overworldState = new OverworldState();
        underworldState = new UnderworldState();
    }
}