using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using TMPro;

public class DatabaseManager : MonoBehaviour
{
    DatabaseReference databaseReference;

    [SerializeField] private GameObject showScores;
    private bool isShowScore = false;
    private bool isSaveShowScore = false;

    Stack<ScoreData> scoreDatas;

    public Stack<ScoreData> ScoreDatas { get { return scoreDatas; } }

    void Awake()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        scoreDatas = new Stack<ScoreData>();
    }

    private void LateUpdate()
    {
        if(isShowScore)
        {
            GetScores();
        }
        else if (isSaveShowScore)
        {
            ReGetScores();
        }
    }

    public void GetData(bool isSave)
    {
        databaseReference.Child("users").OrderByChild("score").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
#if UNITY_EDITOR
                Debug.LogError("Error Database");
#endif
                return;
            }

            if (!task.IsCompleted)
            {
#if UNITY_EDITOR
                Debug.LogError("Fail Get");
#endif
                return;
            }


            DataSnapshot snapshot = task.Result;

            foreach (DataSnapshot child in snapshot.Children)
            {
                IDictionary data = (IDictionary)child.Value;

                scoreDatas.Push(new ScoreData(data["nickname"].ToString(), data["date"].ToString(), data["score"].ToString()));
            }

            if (isSave) isSaveShowScore = true;
            else isShowScore = true;
        });
    }

    public bool WriteData(string nickname, int score)
    {
        try
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            DatabaseReference data = databaseReference.Child("users").Push();

            data.Child("nickname").SetValueAsync(nickname);
            data.Child("date").SetValueAsync(date);
            data.Child("score").SetValueAsync(score);

            return true;
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogWarning(e);
#endif
            return false;
        }
    }

    private void GetScores()
    {
        isShowScore = false;

        // scoreManager�� �ִ� scoreDatas�� �ʱ�ȭ �� ��
        // databaseManager���� GetData�� ȣ���Ͽ� �����͸� scoreManager�� ������ ��
        // �����͸� ������ �������� �����ش�.
        //Stack<ScoreData> scoreDatas = GameManager.instance.scoreManager.ScoreDatas;

        while (scoreDatas.Count > 0)
        {
            GameObject scoreObj = GameManager.instance.prefabManager.GetScoreObj();
            ScoreData data = scoreDatas.Pop();

            // nickname
            scoreObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = data.Nickname;
            // date
            scoreObj.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = data.Date;
            // score
            scoreObj.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = data.Score;

            scoreObj.transform.SetParent(showScores.transform);
            scoreObj.transform.localScale = Vector3.one;
            scoreObj.SetActive(true);
        }
    }

    private void ReGetScores()
    {
        isSaveShowScore = false;
        PutBackScores();
        GetScores();
    }

    public void PutBackScores()
    {
        // ������ �ִ� ���ھ����� �ǵ������� �ٽ� �����´�.
        int childCount = showScores.transform.childCount;

        for (int i = 0; i < childCount; ++i)
        {
            GameManager.instance.prefabManager.PutBackObj(showScores.transform.GetChild(0).gameObject);
        }
    }
}
