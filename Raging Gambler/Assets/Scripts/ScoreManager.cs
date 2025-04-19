using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public class scoreEntry
    {
        public int score;
        public string name;

        public scoreEntry(int score, string name)
        {
            this.score = score;
            this.name = name;
        }
    }
    public List<scoreEntry> scoreList = new List<scoreEntry>();
    public int playerCount = 1;
    [SerializeField] public AudioSource mainGameMusic;
    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("Score").Length > 1)
        {
            Destroy(this);
        }
    }
    void Start()
    {
        DontDestroyOnLoad(this);
        mainGameMusic.Play();
    }

    void sortList()
    {
        for(int i = 1; i < scoreList.Count; i++)
        {
            scoreEntry key = scoreList[i];
            int j = i - 1;
            while(j >= 0 && scoreList[j].score < key.score)
            {
                scoreList[j + 1] = scoreList[j];
                j--;
            }
            scoreList[j + 1] = key;
        }
    }
    
    public void addEntry(int score, string name)
    {
        if(scoreList.Count == 0)
        {
            scoreEntry newEntry = new scoreEntry(score, name);
            scoreList.Add(newEntry);
            return;
        }
        int lowestScore = scoreList[scoreList.Count - 1].score;
        if(scoreList.Count >= 10 && score > lowestScore)
        {
            scoreEntry newEntry = new scoreEntry(score, name);
            scoreList.Remove(scoreList[scoreList.Count - 1]);
            scoreList.Add(newEntry);
            sortList();
        }
        else if (scoreList.Count < 10)
        {
            scoreEntry newEntry = new scoreEntry(score, name);
            scoreList.Add(newEntry);
            sortList();
        }
    }
}
