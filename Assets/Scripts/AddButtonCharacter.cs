using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class AddButtonCharacter : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Sprite[] robots;
    public float paddingX;
    public float paddingY;
    // Start is called before the first frame update
    void Start()
    {
        createUIButtonMatrix();
    }

    void createUIButtonMatrix() {
        int numCols = 3;
        float initX = 35 + paddingX;

        float x = initX;
        float y = 344 - paddingY;
        for(int i=0;i<robots.Length; i++) {
            drawRobotWithPosition(robots[i], x, y);
            x+=100;
            if((i+1)%numCols == 0) {
                y -= 90;
                x = initX;
            }
        }
    }

    void drawRobotWithPosition(Sprite robot, float x, float y) {
        GameObject goButton = (GameObject)Instantiate(buttonPrefab);
        goButton.transform.SetParent(this.transform, false);
        goButton.transform.position = new Vector3(x, y, 0);

        Button tempButton = goButton.GetComponent<Button>();
        tempButton.GetComponent<Image>().sprite = robot;
        float tempInt = x;

        tempButton.onClick.AddListener(() => ButtonClicked(robot));

    }

    void ButtonClicked(Sprite robot)
    {
        SceneManager.LoadScene(robot);
    }
}
