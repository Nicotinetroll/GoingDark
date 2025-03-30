using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class TimelineDebugDisplay : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private TMP_Text debugText;

    private void Update()
    {
        if (director == null || debugText == null) return;

        double time = director.time;
        double frameRate = 60.0; // You can replace with director.playableAsset.duration if needed
        int currentFrame = Mathf.FloorToInt((float)(time * frameRate));

        debugText.text = $"Time: {time:F2}s\nFrame: {currentFrame}";
    }
}