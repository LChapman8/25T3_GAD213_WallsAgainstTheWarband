using UnityEngine;

public static class PlaySoundAtPosition
{
    public static void PlayClip(AudioClip clip, Vector3 position, float volume =0.5f)
    {
        if (clip == null) return;

        GameObject obj = new GameObject("OneShotSound");
        obj.transform.position = position;

        AudioSource source = obj.AddComponent<AudioSource>();
        source.clip = clip;

        source.spatialBlend = 0;   // still 2D sound
        source.volume = Mathf.Clamp01(volume); // ensure safe volume range

        source.Play();

        GameObject.Destroy(obj, clip.length);
    }
}
