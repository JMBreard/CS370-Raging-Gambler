using TMPro;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    public ScoreManager scoreManager;
    public Transform entryContainer;
    public Transform entryTemplate;
    public Transform noScore;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreManager = (ScoreManager)GameObject.Find("Score Manager").GetComponent("ScoreManager");
        entryContainer = transform.Find("scoreEntryContainer");
        entryTemplate = entryContainer.Find("scoreEntryTemplate");
        noScore = entryContainer.Find("NoScore");
        entryTemplate.gameObject.SetActive(false);
        noScore.gameObject.SetActive(false);
        float templateHeight = 25f;
        if(scoreManager.scoreList.Count == 0 )
        {
            noScore.gameObject.SetActive(true);
        }
        for(int i = 0; i < scoreManager.scoreList.Count; i++) 
        {
            Transform entryTransform = Instantiate(entryTemplate, entryContainer);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * i);
            entryTransform.gameObject.SetActive(true);

            int rank = i + 1;
            string rankString;
            switch(rank)
            {
                default:
                    rankString = rank + "TH";
                    break;
                case 1:
                    rankString = "1ST";
                    break;
                case 2:
                    rankString = "2ND";
                    break;
                case 3:
                    rankString = "3RD";
                    break;
            }
            entryTransform.Find("posText").GetComponent<TextMeshProUGUI>().text = rankString;
            entryTransform.Find("nameText").GetComponent<TextMeshProUGUI>().text = scoreManager.scoreList[i].name;
            entryTransform.Find("scoreText").GetComponent<TextMeshProUGUI>().text = scoreManager.scoreList[i].score.ToString();
        }
    }
}
