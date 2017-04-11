using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Pojo : MonoBehaviour {
    public bool IsGhostBonus;
    public bool IsInvertedFlight;
    public float hunger = 100f;
    public int points = 0;
    public int gold = 0;
    GameObject camera;
    public List<GameObject> nestling;

    // Use this for initialization
    void Start()
    {
        camera = GameObject.Find("Main Camera Game");

        Debug.Log(camera.GetComponent<Game>().nestlingCount);
        IsGhostBonus = false;
        IsInvertedFlight = false;

        //Nestling information from "Game" object ( Saving System ).

        for (int i = 0; i < camera.GetComponent<Game>().nestlingCount; i++)
        {
            GameObject nestlings = Instantiate(nestling[i], new Vector3(transform.position.x + 3f, transform.position.y, -1), Quaternion.identity) as GameObject;
            nestlings.AddComponent<Nestling>();
            camera.GetComponent<Boid>().boids.Add(nestlings);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (FloatToInt(transform.position.x) >= 0)
        {
            camera.GetComponent<Points>().distance = FloatToInt(transform.position.x);
        }

        points = FloatToInt(transform.position.x);
	}

    private static int FloatToInt(float f)
    {
        try {
            return (int)Math.Floor(f + 0.5);
        }
        catch
        {
            return 0;
        }
    }
}
