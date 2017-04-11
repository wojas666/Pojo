using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class Boid : MonoBehaviour {
    public float angleOfView;
    public float distanceOfView;

    public float minimumDistanceX;
    public float minimumDistanceY;
    public float noise;

    public List<GameObject> boids;

    private const float bonusDistance = 2f;
    private const float standardOffset = 99.5f;
    public float weights;
    private float nestlingSpeedForward;
    private float nestlingSpeedUp;
    
    private GameObject pojo;

	// Use this for initialization
	void Start () {
        pojo = (GameObject.FindGameObjectsWithTag("Player") as GameObject[]).FirstOrDefault();
        nestlingSpeedForward = pojo.GetComponent<PlayerMovement>().forwardSpeed;

        for(int i = 1; i < boids.Count; i++)
        {
            boids[i].GetComponent<Nestling>().level = GetComponent<Game>().nestling[i - 1].level;
            boids[i].GetComponent<Nestling>().experience = GetComponent<Game>().nestling[i - 1].experience;
            boids[i].GetComponent<Nestling>().ghostBonusTime_ExtraPercent = GetComponent<Game>().nestling[i - 1].ghostBonusTime_ExtraPercent;
            boids[i].GetComponent<Nestling>().slowAndFastGUI_ExtraPercent = GetComponent<Game>().nestling[i - 1].slowAndFastGUI_ExtraPercent;
            boids[i].GetComponent<Nestling>().doubleGold = GetComponent<Game>().nestling[i - 1].doubleGold;
        }
	}

    // Update is called once per frame
    void Update()
    {
        if (/*boids[0].tag == "Player" &&*/ boids.Count > 1)
        {
            SetAvarangeDistance();

            minimumDistanceX = Random.Range(0.1f, 3f);

            for (int i = 1; i < boids.Count; i++)
            {
                boids[i].GetComponent<BoidSetting>().nestlingSpeedForward = ModifyForwardSpeedBoid(i);
                boids[i].GetComponent<BoidSetting>().nestlingSpeedUp = ModifyUpSpeedBoid(i);
            }


            for (int i = 1; i < boids.Count; i++)
            {
                boids[i].GetComponent<BoidSetting>().nestlingSpeedForward += (noise * ((Random.Range(0f, 100f) / standardOffset)) * 10f);
                boids[i].GetComponent<BoidSetting>().nestlingSpeedUp += (noise * ((Random.Range(0f, 100f) / standardOffset)) * 10f);

                for (int j = 0; j < boids.Count; j++)
                {
                    if (j == i)
                        continue;

                    if (Mathf.Abs(boids[j].transform.position.x - boids[i].transform.position.x) < minimumDistanceX)
                    {
                        boids[i].GetComponent<BoidSetting>().nestlingSpeedForward = GetNewForwardSpeed(i, j);
                        boids[i].GetComponent<BoidSetting>().nestlingSpeedUp = GetNewUpSpeed(i, j);
                    }
                    else if (Mathf.Abs(boids[j].transform.position.x - boids[i].transform.position.x) > minimumDistanceX)
                    {
                        boids[i].GetComponent<BoidSetting>().nestlingSpeedForward = ChangeForwardSpeedIncreaseTheDistances(i, j);
                        boids[i].GetComponent<BoidSetting>().nestlingSpeedUp = ChangeUpSpeedIncreaseTheDistances(i, j);
                    }
                }
            }
        }
        else
        {
            if (boids.Count > 1)
            {
                for (int i = 0; i < boids.Count; i++)
                {
                    boids[i].GetComponent<BoidSetting>().nestlingSpeedForward += 5f;
                    boids[i].GetComponent<BoidSetting>().nestlingSpeedUp += 5f;
                    boids[i].GetComponent<BoidSetting>().isPojoDeadh = true;
                }

                Destroy(GetComponent<Boid>());
            }
        }
	}

    #region Boids Alghoritm Helphers Method
    public float ForwardSpeedAvarange()
    {
        float avarangeSpeed = 0f;

        for (int i = 0; i < boids.Count; i++)
            avarangeSpeed += boids[i].GetComponent<BoidSetting>().nestlingSpeedForward;

        avarangeSpeed = avarangeSpeed / float.Parse(boids.Count.ToString());

        return avarangeSpeed;
    }
    public float UpSpeedAvarange()
    {
        float avarangeSpeed = 0f;

        for (int i = 0; i < boids.Count; i++)
            avarangeSpeed += boids[i].GetComponent<BoidSetting>().nestlingSpeedUp;

        avarangeSpeed = avarangeSpeed / float.Parse(boids.Count.ToString());

        return avarangeSpeed;
    }
    public float ModifyForwardSpeedBoid(int boidIndex)
    {
        float avarangeForwardSpeed = ForwardSpeedAvarange();

        return boids[boidIndex].GetComponent<BoidSetting>().nestlingSpeedForward + (weights * (avarangeForwardSpeed - boids[boidIndex].GetComponent<BoidSetting>().nestlingSpeedForward));
    }
    public float ModifyUpSpeedBoid(int boidIndex)
    {
        float avarangeUpSpeed = UpSpeedAvarange();

        return boids[boidIndex].GetComponent<BoidSetting>().nestlingSpeedUp + (weights * (avarangeUpSpeed - boids[boidIndex].GetComponent<BoidSetting>().nestlingSpeedUp));
    }
    public float GetBoidDistance(int boidIndex, int neighborIndex)
    {
        return Mathf.Sqrt(Mathf.Pow(boids[neighborIndex].transform.position.x /*+ bonusDistance*/ - boids[boidIndex].transform.position.x, 2) + Mathf.Pow(boids[neighborIndex].transform.position.y - boids[boidIndex].transform.position.y, 2));
    }
    public float GetNewForwardSpeed(int boidIndex, int neighborIndex)
    {
        float distance = GetBoidDistance(boidIndex, neighborIndex) + 0.02f;

        return boids[boidIndex].GetComponent<BoidSetting>().nestlingSpeedForward + (weights / float.Parse(boids.Count.ToString())) * (((boids[neighborIndex].transform.position.x - boids[boidIndex].transform.position.x) * (distance - boids[boidIndex].GetComponent<BoidSetting>().avarangeDistance)) / distance);
    }
    public float GetNewUpSpeed(int boidIndex, int neighborIndex)
    {
        float distance = GetBoidDistance(boidIndex, neighborIndex) + 0.01f;

        return boids[boidIndex].GetComponent<BoidSetting>().nestlingSpeedUp + (weights / float.Parse(boids.Count.ToString())) * (((boids[neighborIndex].transform.position.y - boids[boidIndex].transform.position.y) * (distance - boids[boidIndex].GetComponent<BoidSetting>().avarangeDistance)) / distance);
    }
    public float ChangeForwardSpeedIncreaseTheDistances(int boidIndex, int neighborIndex)
    {
        float distance = GetBoidDistance(boidIndex, neighborIndex) + 0.01f;

        return boids[boidIndex].GetComponent<BoidSetting>().nestlingSpeedForward - (weights / float.Parse(boids.Count.ToString())) * ((((boids[neighborIndex].transform.position.x - boids[boidIndex].transform.position.x) * (minimumDistanceX)) / distance) - (boids[neighborIndex].transform.position.x - boids[boidIndex].transform.position.x));
    }
    public float ChangeUpSpeedIncreaseTheDistances(int boidIndex, int neighborIndex)
    {
        float distance = GetBoidDistance(boidIndex, neighborIndex) + 0.01f;

        return boids[boidIndex].GetComponent<BoidSetting>().nestlingSpeedUp - (weights / float.Parse(boids.Count.ToString())) * ((((boids[neighborIndex].transform.position.y - boids[boidIndex].transform.position.y) * (minimumDistanceY)) / distance) - (boids[neighborIndex].transform.position.y - boids[boidIndex].transform.position.y));
    }
    public void SetAvarangeDistance()
    {
        for(int i = 0; i < boids.Count; i++)
        {
            boids[i].GetComponent<BoidSetting>().avarangeDistance = 0f;

            for(int j = 0; j < boids.Count; j++)
            {
                boids[i].GetComponent<BoidSetting>().avarangeDistance += GetBoidDistance(i, j);
            }

            boids[i].GetComponent<BoidSetting>().avarangeDistance = boids[i].GetComponent<BoidSetting>().avarangeDistance / float.Parse(boids.Count.ToString());
        }
    }
    #endregion
    
}
