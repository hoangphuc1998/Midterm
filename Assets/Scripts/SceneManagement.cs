using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public static class SceneManager
{
    private static int robot;

    public static void LoadScene(int robot)
    {
        SceneManager.robot = robot;
    }

    public static int getRobotSprite()
    {
        return SceneManager.robot;
    }
}