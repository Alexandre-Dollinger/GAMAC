using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.Netcode;
using _Scripts.Health;
using System.Globalization;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject UI_Panel;
    [SerializeField] private TextMeshProUGUI _coinScoreText;
    public int _coinScore = 0;

    [SerializeField] private List<GameObject> _hearts;
    [SerializeField] private List<Sprite> _heartSprites;

    private enum HeartState 
    {
        Full,
        Half,
        Empty
    }

    #region Start and Update
     void Awake()
    {
        _coinScoreText.text = "0";
        _coinScoreText.color = Color.yellow;
        UI_Panel.SetActive(true);
        _hearts.ForEach(heart => ChangeHeartState(heart, HeartState.Full));
    }

    void Update()
    {
        _coinScoreText.text = $"{_coinScore}";
    }
    #endregion

    private void ChangeHeartState(GameObject heart, HeartState heartState)
    {
        Image heartImage = heart.GetComponent<Image>();
        heartImage.sprite = _heartSprites[(int) heartState];
    }

    public void UpdateHeartsState(int playerHealth)
    {
        int Hp = playerHealth / 10;
        //Debug.Log($"Current HP: {_playerHp.CurrentHp}, Hearts: {Hp}");

        for (int i = 0; i < Hp/2; i++)
        {
            ChangeHeartState(_hearts[i], HeartState.Full);
        }

        for (int i = _hearts.Count - 1; i >= Hp/2 ; i--)
        {
            ChangeHeartState(_hearts[i], HeartState.Empty);
        }

        if (Hp % 2 == 1) //For the case of Half a heart
        {
            ChangeHeartState(_hearts[Hp/2], HeartState.Half);
        }
    }
   

}