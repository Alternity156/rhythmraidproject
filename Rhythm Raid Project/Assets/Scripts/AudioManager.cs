using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }

    [SerializeField] public AudioSource audioSource;

    public bool audioLoaded = false;

    private void Awake()
    {
        I = this;
    }

    public AudioClip ConvertAudioClipToMono(AudioClip audioClip, string name)
    {
        AudioClip newAudioClip = AudioClip.Create(name, audioClip.samples, audioClip.channels, audioClip.frequency, false);
        float[] copyData = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(copyData, 0);

        List<float> monoData = new List<float>();

        for (int i = 0; i < copyData.Length; i += 2)
        {
            monoData.Add(copyData[i]);
        }
        newAudioClip.SetData(monoData.ToArray(), 0);

        return newAudioClip;
    }

    IEnumerator LoadAudioClip(string filePath)
    {
        string uriPath = $"file://{filePath}";

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uriPath, AudioType.OGGVORBIS))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error loading audio: {www.error}");
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);

                if (clip != null)
                { 
                    while (clip.loadState != AudioDataLoadState.Loaded)
                    {
                        yield return null;
                    }

                    audioSource.clip = clip;

                    if (GameManager.I.gameState == GameManager.GameState.Editor)
                    {
                        MainEditor.I.positionSlider.maxValue = GetLength();
                        MainEditor.I.positionSlider.value = 0;
                        MainEditor.I.ApplyWaveFormTexture();
                        MainEditor.I.SetWaveformCanvasPosition();
                        MainEditor.I.waveformPixelsPerSecond = Utilities.I.PixelsPerSeconds(GetLength(), MainEditor.I.GetWaveformWidth());
                        MainEditor.I.ApplyBpmLabels();
                        MainEditor.I.PlaceBeatLines();
                    }

                    audioLoaded = true;

                    Debug.Log($"Audio {filePath} loaded");
                }
            }
        }
    }

    public void LoadAudio(string filePath)
    {
        StartCoroutine(LoadAudioClip(filePath));
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void Play()
    {
        audioSource.Play();
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    public void Pause()
    {
        audioSource.Pause();
    }

    public void Seek(float time)
    {
        audioSource.time = time;
    }

    public float GetLength()
    {
        return audioSource.clip.length;
    }

    public float GetPosition()
    {
        return audioSource.time;
    }

    public bool IsPlaying() 
    { 
        return audioSource.isPlaying;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
