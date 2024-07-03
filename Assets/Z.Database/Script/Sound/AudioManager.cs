using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

public enum soundType
{
    background,
    vfx
}
public class AudioManager:MonoBehaviour
{

    public static AudioManager Instance { get; private set; }
    public AudioMixer audiomixer;
    public enum playType
    {
        _3D,
        _2D
    }
    public enum soundtype
    {
        DesertEagleShot,
        ShotGunShot,
        SkeletonHit,
        Swing
    }
    public interface IECoroutine
    {
        Coroutine StartCoroutine(Coroutine coroutine);
        void StopCoroutine(Coroutine coroutine);
    }
    //1. ���带 ���¹��鿡 �ε�
    //2. ������ �̸������� json�� ����
    //3. �������� ������ �������̸�, �����̸�, ������ ����, json�� ������ �̸���

    //��� ���尡 startcoroutine���� ������. �̰��� ������ �ʿ�
    List<SoundPlayer> playingLoopSounds;
    Stack<SoundPlayer> soundpool;
    [SerializeField]
    AudioClip[] sounds_;
    Dictionary<soundtype, AudioClip> soundDictionary;

    int stack_;
    private void Awake()
    {
        Instance = this;
        SoundDataLoad();
        stack_ = 0;
    }
    public List<AudioClip> getClips(params soundtype[] type)
    {
        List<AudioClip> clips = new List<AudioClip>();
        foreach(soundtype st  in type)
        {
            clips.Add(soundDictionary[st]);
        }
        return clips;
    }
    /// <summary>
    /// ���� ������ �ε�
    /// </summary>
    void SoundDataLoad()
    {
        soundDictionary = new Dictionary<soundtype, AudioClip>();
        playingLoopSounds = new List<SoundPlayer>();
       for(int i=0;i<sounds_.Length; i++)
            soundDictionary.Add((soundtype)i, sounds_[i]);
    }
    private void Start()
    {
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            PlaySound(playType._2D, soundtype.ShotGunShot, transform);
        }
    }
    /// <summary>
    /// 3D ���� ���
    /// </summary>
    /// <param name="type"></param>
    /// <param name="target"></param>
    /// <param name="loop"></param>
    /// <returns></returns>
    public int PlaySound(playType ptype, soundtype type,Transform target,bool loop= false,bool follow= false)
    {
        //soundpool ���ÿ� �ƹ��͵� ���� ���
        if (soundpool == null || soundpool.Count == 0)
        {
            createPool(1);
        }
        SoundPlayer sp_ = soundpool.Pop();
        sp_.soundtransform.gameObject.SetActive(true);
        if (ptype == playType._3D) { sp_.PlaySound(SoundPlayer.Ptype._3D,soundDictionary[type], target, loop, follow); }
        else if (ptype == playType._2D) { sp_.PlaySound(SoundPlayer.Ptype._2D, soundDictionary[type], target, loop, false); }
        
        if (loop == false)
        {
            StartCoroutine(endMusic(sp_));
            return -1;
        }
        else
        {
            playingLoopSounds.Add(sp_);
            return sp_.id;
        }
    }
    ///// <summary>
    ///// 2D ���� ���
    ///// </summary>
    ///// <param name="type"></param>
    ///// <param name="loop"></param>
    ///// <returns></returns>
    //public int BackgroundPlaySound(soundtype type,bool loop=false,bool follow=false) //������Ʈ Ʈ����������
    //{
    //    //soundpool ���ÿ� �ƹ��͵� ���� ���
    //    if (soundpool == null || soundpool.Count == 0)
    //    {
    //        createPool(1);
    //    }
    //    SoundPlayer sp_ = soundpool.Pop();
    //    sp_.soundtransform.gameObject.SetActive(true); 
    //    sp_.PlaySound2D(soundDictionary[type], loop);
    //    if (loop == false)
    //    {
    //        StartCoroutine(endMusic(sp_));
    //        return -1;
    //    }
    //    else
    //    {
    //        playingLoopSounds.Add(sp_);
    //        return sp_.id;
    //    }
    //}
    /// <summary>
    /// ���� ���� ���
    /// </summary>
    /// <param name="audio"></param>
    /// <param name="tf_"></param>
    public int PlaySound(playType ptype, List<AudioClip> audio,Transform tf_, bool loop= false) //������Ʈ Ʈ����������
    {
        //soundpool ���ÿ� �ƹ��͵� ���� ���
        if (soundpool == null || soundpool.Count == 0)
        {
            createPool(1);
        }
        SoundPlayer sp_ = soundpool.Pop();
        int r = UnityEngine.Random.Range(0, audio.Count);

        AudioClip audio_ = audio[r];
        sp_.soundtransform.gameObject.SetActive(true);
        if (ptype == playType._3D) { sp_.PlaySound(SoundPlayer.Ptype._3D, audio_, tf_, loop); }
        else if (ptype == playType._2D) { sp_.PlaySound(SoundPlayer.Ptype._2D, audio_, tf_, loop); }
        if (loop == false)
        {
            StartCoroutine(endMusic(sp_));
            return -1;
        }
        else
        {
            playingLoopSounds.Add(sp_);
            return sp_.id;
        }
    }
    /// <summary>
    /// ���� ȹ��
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public AudioClip getSound(soundtype type)
    {
        return soundDictionary[type];
    }
    /// <summary>
    /// ���� Ǯ ����
    /// </summary>
    /// <param name="num"></param>
    void createPool(int num)
    {
        if(soundpool == null)
        {
            soundpool = new Stack<SoundPlayer>();
        }
        
        GameObject g_ = new GameObject("Sound");
        var au_ = g_.AddComponent<AudioSource>();
        var sf_ = g_.AddComponent<SoundFollower>();

        SoundPlayer sp = new SoundPlayer(au_, sf_, g_.GetComponent<Transform>());
        sp.id = stack_++;
        sp.pushMusic += pushSoundPlayer;
        soundpool.Push(sp);
        g_.transform.parent = transform;
    }

    /// <summary>
    /// ���� push
    /// </summary>
    /// <param name="sp_"></param>
    void pushSoundPlayer(SoundPlayer sp_)
    {
        sp_. stopClip();
        soundpool.Push(sp_);
        sp_.soundtransform.gameObject.SetActive(false);
    }
    /// <summary>
    /// loop ���� stop
    /// </summary>
    /// <param name="id"></param>
    public void stopSound(int id)
    {
        if (playingLoopSounds.Count == 0) return;
        SoundPlayer sp_ = playingLoopSounds.Find(x => x.id == id);
        if (sp_ == null) return;
        sp_.stopSound();
        playingLoopSounds.Remove(sp_);
    }
    /// <summary>
    /// ���峡������ üũ
    /// ���߿� startcoroutine���� stopwatch�� ���� ���� ����ϰ��Ѵ��� �ϴ°͵� ������
    /// </summary>
    /// <param name="sp_"></param>
    /// <returns></returns>
    IEnumerator endMusic(SoundPlayer sp_)
    {
        yield return new WaitForSeconds(sp_.getPlayTime() + 0.1f);
        sp_.pushMusic?.Invoke(sp_);
    }
    /// <summary>
    /// ���� innerclass
    /// </summary>
    private class SoundPlayer
    {
        public enum Ptype
        {
            _3D,
            _2D
        }
        public int id;
        public Action<SoundPlayer> pushMusic;
        public Transform  soundtransform;
        private AudioSource audiosource;
        private SoundFollower soundfollower;
        
        //���� ������ clip loop awake �ʱ�ȭ
        //������ �������� �ǵ������
        public SoundPlayer(AudioSource audiosource,SoundFollower soundFollower,Transform tf_)
        {
            this.audiosource = audiosource;
            this.audiosource.spatialBlend = 1f;
            this.audiosource.rolloffMode = AudioRolloffMode.Custom;
            this.audiosource.playOnAwake = false;
            this.audiosource.maxDistance = 30f;
            this.audiosource.outputAudioMixerGroup = AudioManager.Instance.audiomixer.FindMatchingGroups("attack")[0];
            this.soundfollower = soundFollower;
            soundtransform = tf_;
        }
        public void stopSound()
        {
            pushMusic?.Invoke(this);
        }
        public void stopClip()
        {
            audiosource.Stop();
        }
        public void PlaySound(Ptype type, AudioClip clip,Transform tf_, bool loopbool=false, bool follow = false)
        {
            if(audiosource.isPlaying) { Debug.Log("isPlaying"); return; }
            soundtransform.position = tf_.position;
            if(type==Ptype._3D) this.audiosource.spatialBlend = 1f;
            else if(type==Ptype._2D) this.audiosource.spatialBlend = 0f;
            if (follow)
            {
                soundfollower.setTarget(tf_);
            }
            Debug.Log(audiosource.clip);
            audiosource.clip = clip;
            Debug.Log(audiosource.clip);
            audiosource.loop = loopbool;
            audiosource.Play();
        }
        public float getPlayTime()
        {
            return audiosource.clip.length;
        }
    }
}

