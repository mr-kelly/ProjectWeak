using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class CAudioModule : MonoBehaviour
{
    Transform _Transform;
    static CAudioModule _Instance;
    Dictionary<string, AudioSource> _LoopAudios = new Dictionary<string, AudioSource>();

    public static CAudioModule Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = new GameObject("CAudioManager").AddComponent<CAudioModule>();

            return _Instance;
        }
    }

    void Awake()
    {
        _Transform = transform;
        gameObject.AddComponent<AudioListener>();
    }

    void Update()
    {
    }

    public void PlayAudio(GameObject playObject, string path, bool loop = false)
    {
        if (loop)
        {
            if (_LoopAudios.ContainsKey(path))
            {
                return;
            }

            _LoopAudios[path] = null;
        }

        CAssetBundleLoader loader = new CAssetBundleLoader(path + "_Audio" + CCosmosEngine.GetConfig("AssetBundleExt"), (_sz, _ab, _args) =>
        {
            OnLoadAudioClip(_ab.mainAsset as AudioClip, new object[] { path, playObject, loop });
        });
    }
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="path">音效路径</param>
    /// <param name="loop">是否循环</param>
    public void PlayAudio(string path, bool loop = false)
    {
        PlayAudio(gameObject, path, loop);
    }

    public void StopPlay(string path)
    {
        AudioSource audio;
        if (_LoopAudios.TryGetValue(path, out audio))
        {
            GameObject.Destroy(audio);
            _LoopAudios.Remove(path);
        }
        else
            CBase.LogWarning("Not found playing music : {0}", path);
    }

    void OnLoadAudioClip(Object asset, params object[] args)
    {
        string path = (string)args[0];
        GameObject playObject = (GameObject)args[1];
        bool loop = (bool)args[2];
        AudioSource audioSource = playObject.AddComponent<AudioSource>();

        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = 40;
        audioSource.clip = asset as AudioClip;
        audioSource.loop = loop;
        audioSource.Play();

        CBase.Assert(audioSource.clip.length > 0);


        if (loop)
        {
            _LoopAudios[path] = audioSource;
        }
        else
        {
            Destroy(audioSource, audioSource.clip.length);
        }
    }
}