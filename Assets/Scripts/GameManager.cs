using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Range(30, 60)]
    public int targetFPS = 60;
    private void Awake()
    {
        Application.targetFrameRate = targetFPS;
    }
}
