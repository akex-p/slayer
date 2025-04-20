using System;
using UnityEngine;

public class BeatManager : MonoBehaviour
{

    [SerializeField] private float bpm;
    [SerializeField] private float firstBeatOffset;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource clickSource;

    private float secPerBeat;
    private float dspSongTime;

    private float songPosition;
    private float songPositionInBeats;
    private int lastBeat = -1;

    public static event Action OnBeat;

    private void Start()
    {
        secPerBeat = 60f / bpm;
        dspSongTime = (float)AudioSettings.dspTime;

        musicSource.Play();
    }

    private void Update()
    {
        songPosition = (float)(AudioSettings.dspTime - dspSongTime - firstBeatOffset);
        songPositionInBeats = songPosition / secPerBeat;

        int currentBeat = Mathf.FloorToInt(songPositionInBeats);

        if (currentBeat != lastBeat)
        {
            lastBeat = currentBeat;
            OnBeat?.Invoke(); // Fire event
            Debug.Log("Beat!");

            clickSource.Play();
        }
    }

    public TimingAccuracy CheckInputTiming()
    {
        float beatPosition = songPositionInBeats;
        float distanceToNearestBeat = Mathf.Abs(beatPosition - Mathf.Round(beatPosition));
        float window = 0.5f; // in beats (half-beat = max timing leniency)

        if (distanceToNearestBeat < 0.05f)
            return TimingAccuracy.PERFECT;
        else if (distanceToNearestBeat < 0.15f)
            return TimingAccuracy.GOOD;
        else if (distanceToNearestBeat < window)
            return TimingAccuracy.MISS;
        else
            return TimingAccuracy.MISS; // Too far off-beat
    }

}
