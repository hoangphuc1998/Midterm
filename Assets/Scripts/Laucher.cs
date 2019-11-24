using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Laucher : MonoBehaviourPunCallbacks
{
    public GameObject connectedScreen;
    public GameObject disconnectedScreen;
    public GameObject chooseCharacterScreen;
    public GameObject connectButton;
    public GameObject quitButton;
    public GameObject buttonPrefab;
    public Sprite[] robots;

    private void Start()
    {

    }
    public void OnClick_Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {

        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        connectButton.SetActive(false);
        quitButton.SetActive(false);
        disconnectedScreen.SetActive(true);
    }

    public override void OnJoinedLobby()
    {
        //Choose characters
        chooseCharacter();
        
        
    }
    void chooseCharacter()
    {
        createUIButtonMatrix();
        connectButton.SetActive(false);
        quitButton.SetActive(false);
        if (disconnectedScreen.activeSelf)
        {
            disconnectedScreen.SetActive(false);
        }
        connectedScreen.SetActive(false);
    }

    void createUIButtonMatrix()
    {
        chooseCharacterScreen.SetActive(true);
        float camVertExtent = Screen.height / 2;
        float camHorExtent = Screen.width / 2;
        Debug.Log(camVertExtent);
        int numCols = 3;
        float initX = camHorExtent / 2;

        float x = initX;
        float y = camVertExtent * 3 / 2;
        for (int i = 0; i < robots.Length; i++)
        {
            drawRobotWithPosition(robots[i], x, y);
            x += camHorExtent / 2;
            if ((i + 1) % numCols == 0)
            {
                y -= camVertExtent / 2;
                x = initX;
            }
        }
    }

    void drawRobotWithPosition(Sprite robot, float x, float y)
    {
        GameObject goButton = (GameObject)Instantiate(buttonPrefab);
        goButton.transform.SetParent(chooseCharacterScreen.transform, false);
        goButton.transform.position = new Vector3(x, y, 0);
        Button tempButton = goButton.GetComponent<Button>();
        tempButton.GetComponent<Image>().sprite = robot;
        tempButton.GetComponentInChildren<Text>().text = "";
        float tempInt = x;

        tempButton.onClick.AddListener(() => ButtonClicked(robot));

    }
    void ButtonClicked(Sprite robot)
    {
        SceneManager.LoadScene(robot);
        chooseCharacterScreen.SetActive(false);
        connectedScreen.SetActive(true);
    }
    public void OnClickQuit()
    {
        Application.Quit();
    }
}
