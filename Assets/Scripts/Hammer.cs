using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;

public class Hammer : MonoBehaviour
{
    public float explosionRadius;
    public float explositonPower;

    private Collider[] _results;

    private int _lastExplosionFrame;

    private void Awake()
    {
        _results = new Collider[200];
    }
    private void OnCollisionEnter(Collision mainCollision)
    {
        if(Time.frameCount == _lastExplosionFrame)
            return;

        _lastExplosionFrame = Time.frameCount;

        if (mainCollision.gameObject.layer != LayerMask.NameToLayer("placed lego"))
            return;

        float radius = Mathf.Max(3f, explosionRadius * mainCollision.relativeVelocity.magnitude);

        int size = Physics.OverlapSphereNonAlloc(mainCollision.transform.position, radius, _results, LayerMask.GetMask("placed lego"));

        for (int i = 0; i < size; i++)
        {
            Collider c = _results[i];

            // Performance optimization: Need to pre-build these real bricks
            GameObject brick = BrickSwapper.SwapToRealBrick(c.gameObject);
            if (brick == null) continue;

            brick.GetComponent<RealtimeTransform>().RequestOwnership();

            Rigidbody rb = brick.GetComponent<Rigidbody>();
            rb.isKinematic = false;

            brick.GetComponent<Rigidbody>().AddExplosionForce(explositonPower * mainCollision.relativeVelocity.magnitude, transform.position, radius, 1f, ForceMode.Impulse);
        }

    }
}
