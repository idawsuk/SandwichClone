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
    [SerializeField] CanvasGroup eatGameGroup;

    [SerializeField] RectTransform[] nomTextPositions;
    [SerializeField] CanvasGroup nomText;

    Sequence nomTextTween;

    // Start is called before the first frame update
    void Start()
    {
        nextLevelButton.onClick.AddListener(NextLevelButton_OnClick);
        resetLevelButton.onClick.AddListener(ResetLevelButton_OnClick);
        gameManager.OnLevelReady += FadeBlocker;
        gameManager.OnEatLevelStarted += EatGameStart;
        gameManager.OnLevelComplete += LevelComplete;
        gameManager.OnBiteEvent += OnBiteEvent;
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
        eatGameGroup.DOFade(0, .3f);
        nextLevelGroup.DOFade(1, .75f).SetEase(Ease.OutQuart).SetDelay(2f).OnComplete(() =>
        {
            nextLevelGroup.interactable = true;
            nextLevelGroup.blocksRaycasts = true;
        });
    }

    void EatGameStart()
    {
        eatGameGroup.DOFade(1, .3f).SetEase(Ease.OutQuart);
    }

    void OnBiteEvent()
    {
        if (nomTextTween != null)
            nomTextTween.Kill();

        int randomIndex = Random.Range(0, nomTextPositions.Length);

        RectTransform RT = (RectTransform)nomText.transform;
        RT.anchoredPosition = nomTextPositions[randomIndex].anchoredPosition;
        RT.rotation = nomTextPositions[randomIndex].rotation;

        nomText.alpha = 1;
        nomTextTween = DOTween.Sequence();
        nomTextTween.Append(nomText.DOFade(0, .5f).SetEase(Ease.OutSine));
        nomTextTween.Join(RT.DOPunchScale(Vector3.one * .3f, .3f, 4).SetEase(Ease.OutBounce));
    }
}
