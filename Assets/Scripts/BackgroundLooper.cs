using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLooper : MonoBehaviour {
    [SerializeField] private Renderer myRenderer;
    [SerializeField] private float speed;

    void Update() {
        myRenderer.material.mainTextureOffset += new Vector2(speed * Time.deltaTime, 0f);
    }
}
