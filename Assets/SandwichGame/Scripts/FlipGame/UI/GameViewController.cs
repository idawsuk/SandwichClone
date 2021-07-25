using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameViewController : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] Button resetLevelButton;
    [SerializeField] Button nextLevelButton;

    [SerializeField] Image blocker;
    [SerializeField] CanvasGroup nextLevelGroup;

    // Start is called before the first frame update
    void Start()
    {
        nextLevelButton.onClick.AddListener(NextLevelButton_OnClick);
        resetLevelButton.onClick.AddListener(ResetLevelButton_OnClick);
        gameManager.OnLevelReady += FadeBlocker;
        gameManager.OnLevelComplete += LevelComplete;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ResetLevelButton_OnClick()
    {
        gameManager.ResetLevel();
    }

    void NextLevelButton_OnClick()
    {
        gameManager.LoadNextLevel();
    }

    void FadeBlocker()
    {
        blocker.DOFade(0, .3f).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            blocker.raycastTarget = false;
        });
    }

    void LevelComplete()
    {
        nextLevelGroup.DOFade(1, .75f).SetEase(Ease.OutQuart).SetDelay(2f).OnComplete(() =>
        {
            nextLevelGroup.interactable = true;
            nextLevelGroup.blocksRaycasts = true;
        });
    }
}
