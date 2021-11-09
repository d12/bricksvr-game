using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSound : MonoBehaviour
{
    public enum MaterialType {Metal, Floor};
    public MaterialType material;
    public AudioClip[] metalOnMetalClips;

    private float _nextSoundTime;
    private float _timeBetweenSounds = 0.3f;

    private AudioSource _source;

    // Start is called before the first frame update
    private void Start()
    {
        _source = GetComponent<AudioSource>();
    }

    private float NextSoundTime() {
        return _nextSoundTime;
    }

    private void OnCollisionEnter(Collision collision) {
        if(Time.time < NextSoundTime())
            return;

        GameObject otherObject = collision.gameObject;
        CollisionSound otherObjectCollisionSound = otherObject.GetComponent<CollisionSound>();

        if(otherObjectCollisionSound == null)
            return;

        // If the other collider _just_ made a sound, it was most likely making a sound with _this_ collider.
        // Skip so that we don't double sounds.
        // If we don't skip, we get weird sounds when the randomized track is different.
        if(Time.time < otherObjectCollisionSound.NextSoundTime()) {
            _nextSoundTime = otherObjectCollisionSound.NextSoundTime();
            return;
        }

        float volume = CollisionVelocityToVolume(collision.relativeVelocity.magnitude);

        List<MaterialType> collisionMaterials = new List<MaterialType> { material, otherObjectCollisionSound.material };
        if(collisionMaterials.SequenceEqual(new List<MaterialType> { MaterialType.Metal, MaterialType.Metal })) {
            MetalOnMetalCollision(volume);
        }

        _nextSoundTime = Time.time + _timeBetweenSounds;
    }

    private void MetalOnMetalCollision(float volume) {
        int clipIndex = Random.Range(0, metalOnMetalClips.Length);
        _source.PlayOneShot(metalOnMetalClips[clipIndex], volume);
    }

    private static float CollisionVelocityToVolume(float velocity) {
        if(velocity > 8) {
            return 1;
        }

        return velocity / 8.0f;
    }
}
