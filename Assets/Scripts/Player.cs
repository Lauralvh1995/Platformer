using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    Controller2D controller;
    private void Start()
    {
        controller = GetComponent<Controller2D>();
    }
}
