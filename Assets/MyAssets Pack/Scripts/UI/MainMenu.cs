using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour
{
    Button newGameButton;
    Button continueButtion;
    Button quitButton;

    PlayableDirector director;

    private void Awake()
    {
        //得到canvas下的三个按钮
        newGameButton = transform.GetChild(1).GetComponent<Button>();
        continueButtion = transform.GetChild(2).GetComponent<Button>();
        quitButton = transform.GetChild(3).GetComponent<Button>();

        //>三个按钮分别添加点击事件
        newGameButton.onClick.AddListener(PlayTimeline);
        continueButtion.onClick.AddListener(ContinueGame);
        quitButton.onClick.AddListener(QuitGame);

        //得到PlayableDirector组件
        director = FindObjectOfType<PlayableDirector>();
        //TimeLine播放完停止之后 再真正的开始游戏
        director.stopped += NewGame;
    }
    //新开始游戏 先播放TimeLine
    void PlayTimeline()
    {
        director.Play();
    }

    void NewGame(PlayableDirector pl)
    {
        //新游戏,删除所有本地数据
        PlayerPrefs.DeleteAll();
        SceneController.Instance.Transition_ToGameStart();
    }

    void ContinueGame()
    {
        SceneController.Instance.Transition_ToLoadGame();

    }

    void QuitGame()
    {
        Application.Quit();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ((IPointerEnterHandler)newGameButton).OnPointerEnter(eventData);
    }
}
