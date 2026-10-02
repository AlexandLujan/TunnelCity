using System.Collections;
using UnityEngine;

public class UnderworldState
{
    private CaveState caveState;
    private TunnelCityState tunnelCityState;
    private MiningCompanyState miningCompanyState;

    public CaveState CaveState => caveState;
    public TunnelCityState TunnelCityState => tunnelCityState;
    public MiningCompanyState MiningCompanyState => miningCompanyState;

    public UnderworldState()
    {
        caveState = new CaveState();
        tunnelCityState = new TunnelCityState();
        miningCompanyState = new MiningCompanyState();
    }
}