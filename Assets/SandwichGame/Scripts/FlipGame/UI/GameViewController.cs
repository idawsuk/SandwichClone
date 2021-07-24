using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameViewController : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] Button resetLevelButton;

    // Start is called before the first frame update
    void Start()
    {
        resetLevelButton.onClick.AddListener(ResetLevelButton_OnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ResetLevelButton_OnClick()
    {
        gameManager.ResetLevel();
    }
}
