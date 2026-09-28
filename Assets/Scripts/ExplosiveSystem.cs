using System.Collections;
using UnityEngine;

public class ExplosiveSystem : MonoBehaviour
{
    [SerializeField]
    private float fuseTime = 5.0f;

    private DrilledOpening entrance;
    private CaveManager caveManager;

    public void Initialize(DrilledOpening targetEntrance, CaveManager manager)
    {
        entrance = targetEntrance;
        caveManager = manager;

        StartCoroutine(Fuse());
    }

    private IEnumerator Fuse()
    {
        yield return new WaitForSeconds(fuseTime);

        caveManager.BlastEntrance(entrance);
    }
}
