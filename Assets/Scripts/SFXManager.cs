using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour {

    [SerializeField] private AudioSource planetExplosion, planetSwap;
    [SerializeField] private AudioSource[] levelMusics;


    //For each level picks and plays a random song
    private void Start() {
        levelMusics[Random.Range(0, levelMusics.Length - 1)].Play();
    }

    //Plays planet explosion sound effect
    public void PlayPlanetExplosion() {

        planetExplosion.Play();
    }

    //Plays planet swap sound effect with a little pitch variation
    public void PlayPlanetSwap() {

        planetSwap.pitch = Random.Range(0.9f, 1.3f);
        planetSwap.Play();
    }
}

