using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapCoordinateProbe : EditorWindow
{
    private Tilemap targetTilemap;

    [MenuItem("Tools/Tilemap Coordinate Probe")]
    public static void ShowWindow()
    {
        GetWindow<TilemapCoordinateProbe>("Tilemap Probe");
    }

    private void OnGUI()
    {
        targetTilemap = (Tilemap)EditorGUILayout.ObjectField(
            "Target Tilemap",
            targetTilemap,
            typeof(Tilemap),
            true
        );

        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            "Assign a Tilemap, then hold Shift and click in the Scene view.",
            MessageType.Info
        );
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (targetTilemap == null)
            return;

        Event currentEvent = Event.current;

        if (currentEvent.type == EventType.MouseDown &&
            currentEvent.button == 0 &&
            currentEvent.shift)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(
                currentEvent.mousePosition
            );

            Plane plane = new Plane(Vector3.forward, Vector3.zero);

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 worldPosition =
                    ray.GetPoint(distance);

                Vector3Int cellPosition =
                    targetTilemap.WorldToCell(worldPosition);

                TileBase tile =
                    targetTilemap.GetTile(cellPosition);

                Debug.Log(
                    $"Tilemap Cell: {cellPosition} | " +
                    $"World Position: {worldPosition} | " +
                    $"Tile: {(tile != null ? tile.name : "EMPTY")}"
                );

                currentEvent.Use();
            }
        }
    }
}
