using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public static class SceneManager
{
    private static Sprite robot;

    public static void LoadScene(Sprite robot)
    {
        SceneManager.robot = robot;
        PhotonNetwork.LoadLevel(2);
    }

    public static Sprite getRobotSprite()
    {
        return SceneManager.robot;
    }
}