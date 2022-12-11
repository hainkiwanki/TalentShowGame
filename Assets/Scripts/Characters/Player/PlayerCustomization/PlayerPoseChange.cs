using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerPoseChange : MonoBehaviour
{
    private List<string> poses = new List<string>()
    {
        "idle", "twerking", "making the doodoo", "hiphop", "surrender to god", "flair",
        "perky ass", "draw me like one of your french girls", "shuffling",
        "stir the milk", "workout", "wave", "swinging", "t pose", 
        "do the hanky panky", "samba"
    };

    public List<GameObject> music = new List<GameObject>();

    public Animator animator;
    private int currentIndex = 0;
    public TextMeshProUGUI currentPose;

    public void NextPose()
    {
        music[currentIndex].SetActive(false);
        currentIndex++;
        currentIndex %= poses.Count;
        animator.Play(poses[currentIndex], -1);
        currentPose.text = poses[currentIndex];
        music[currentIndex].SetActive(true);

    }

    public void PreviousPose()
    {
        music[currentIndex].SetActive(false);
        currentIndex--;
        if (currentIndex < 0) currentIndex = poses.Count - 1;
        animator.Play(poses[currentIndex], -1);
        currentPose.text = poses[currentIndex];
        music[currentIndex].SetActive(true);
    }

}
