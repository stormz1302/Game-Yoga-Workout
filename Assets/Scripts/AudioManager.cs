using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer Settings")]
    public AudioMixer audioMixer;           // Audio Mixer tổng
    public AudioMixerGroup musicGroup;      // Nhóm mixer dành cho Music
    public AudioMixerGroup soundGroup;      // Nhóm mixer dành cho Sounds

    [Header("Audio Clips")]
    public List<Sound> sounds;              // Danh sách hiệu ứng âm thanh (Sounds)
    public List<Sound> musicTracks;         // Danh sách nhạc nền (Music)

    [Header("Audio Sources")]
    public AudioSource audioSourceMusic;    // AudioSource dành cho nhạc nền
    public AudioSource audioSourceSounds;   // AudioSource dành cho hiệu ứng âm thanh
    public GameObject audioOb;

    private Dictionary<string, AudioClip> soundDictionary; // Từ điển Sounds
    private Dictionary<string, AudioClip> musicDictionary; // Từ điển Music

    [System.Serializable]
    public class Sound
    {
        public string name;                 // Tên âm thanh hoặc nhạc nền
        public AudioClip clip;              // Tệp âm thanh hoặc nhạc nền
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ AudioManager qua các scene
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Tạo từ điển cho Sounds và Music
        soundDictionary = new Dictionary<string, AudioClip>();
        foreach (var sound in sounds)
        {
            soundDictionary[sound.name] = sound.clip;
        }

        musicDictionary = new Dictionary<string, AudioClip>();
        foreach (var track in musicTracks)
        {
            musicDictionary[track.name] = track.clip;
        }

        // Gán mixer group cho các AudioSource
        audioSourceMusic.outputAudioMixerGroup = musicGroup;
        audioSourceSounds.outputAudioMixerGroup = soundGroup;

        // Load cài đặt âm lượng
        LoadVolumeSettings();
        SceneManager.sceneLoaded += OnSceneLoaded;
        Application.targetFrameRate = -60;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadVolumeSettings();
    }
    public void Start()
    {
        PlayMusic("Music");
        if (Application.platform == RuntimePlatform.Android)
        {
            // Giới hạn FPS, ví dụ: 90
            Application.targetFrameRate = 90;
        }
        else
        {
            // Không giới hạn FPS trên các nền tảng khác
            Application.targetFrameRate = -1;
        }
    }

    // Phát nhạc nền
    public void PlayMusic(string name)
    {
        if (musicDictionary.ContainsKey(name))
        {
            audioSourceMusic.clip = musicDictionary[name];
            audioSourceMusic.Play();
        }
        else
        {
            Debug.LogWarning($"Nhạc nền '{name}' không tồn tại!");
        }
    }

    // Dừng nhạc nền
    public void StopMusic()
    {
        audioSourceMusic.Stop();
        audioMixer.SetFloat("Music", -80f);
    }

    // Phát hiệu ứng âm thanh
    public void PlaySound(string name)
    {
        if (soundDictionary.ContainsKey(name))
        {
            if (audioSourceSounds.isPlaying)
            {
                GameObject newAudioOb = Instantiate(audioOb, gameObject.transform);
                AudioSource newAudioSource = newAudioOb.GetComponent<AudioSource>();
                newAudioSource.clip = soundDictionary[name];
                newAudioSource.Play();
                StartCoroutine(DestroyAfterSound(newAudioOb, newAudioSource));
            }
            else
            {
                audioSourceSounds.clip = soundDictionary[name];
                audioSourceSounds.Play();
            }
        }
        else
        {
            Debug.LogWarning($"Âm thanh '{name}' không tồn tại!");
        }
    }

    private IEnumerator DestroyAfterSound(GameObject audioObject, AudioSource audioSource)
    {
        // Đợi đến khi âm thanh kết thúc
        while (audioSource.isPlaying)
        {
            yield return null; // Đợi 1 frame
        }

        // Hủy GameObject sau khi âm thanh phát xong
        Destroy(audioObject);
    }

    // Dừng hiệu ứng âm thanh
    public void StopSound()
    {
        audioSourceSounds.Stop();
        audioMixer.SetFloat("Sounds", -80f);
    }

    // Chỉnh âm lượng qua Audio Mixer và lưu vào PlayerPrefs
    public void SetVolume(string parameter, bool isOn)
    {
        float volume = isOn ? 0f : -80f;
        audioMixer.SetFloat(parameter, volume);
        SaveVolumeSetting(parameter, volume);
    }

    // Lưu cài đặt âm lượng
    private void SaveVolumeSetting(string parameter, float volume)
    {
        PlayerPrefs.SetFloat(parameter, volume);
        PlayerPrefs.Save();
    }

    // Tải cài đặt âm lượng
    public void LoadVolumeSettings()
    {
        string[] parameters = { "Music", "Sounds" }; // Tên các tham số trong AudioMixer
        foreach (var parameter in parameters)
        {
            float volume = PlayerPrefs.GetFloat(parameter, 0f);
            audioMixer.SetFloat(parameter, volume);
            //Debug.Log($"Tải cài đặt âm lượng {parameter}: {volume}");
        }
        audioSourceMusic.Play();
        audioSourceSounds.Play();
    }
}
