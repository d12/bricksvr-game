using UnityEngine;
using UnityEditor;

public static class SpawnBricks
{
    private static float _outerBounds = 10f;
    [MenuItem("Debug/Spawn Lots of Bricks")]
    public static void Spawn()
    {
        for (int i = 0; i < 100; i++)
        {
            Vector3 pos = RandomBrickPos();
            GameObject brick = 
                GameObject.Instantiate(Resources.Load<GameObject>("4x2 - Placed"), pos, Quaternion.identity);

            brick.transform.rotation = Quaternion.identity;
            brick.transform.position = pos;

            BrickUuid sync = brick.GetComponent<BrickUuid>();
            sync.uuid = BrickId.FetchNewBrickID();
        }
    }

    static Vector3 RandomBrickPos()
    {
        float x = Random.Range(0f, _outerBounds);
        if (Random.Range(0f, 1f) > 0.5f)
            x *= -1;

        float z = Random.Range(0f, _outerBounds);
        if (Random.Range(0f, 1f) > 0.5f)
            z *= -1;

        float y = Random.Range(1f, 4f);

        return new Vector3(x, y, z);;
    }
}