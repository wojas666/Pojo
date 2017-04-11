using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Scripts;

public class BoidSetting : MonoBehaviour {
    public AudioClip goldClip;

    public GameObject bonusGameObject;
    GameObject cameras;
    GameObject pojo;

    public bool isPojoDeadh;

    public float timeToNextBonuses = 3f;

    public float minimumDistance;
    public float avarangeDistance;
    public float weights;
    public float nestlingSpeedForward;
    public float nestlingSpeedUp;

    public Vector3 angleOfView;
    public float distanceOfView;
    public bool isViewGizmosDistance;

    public float timeToNestlingsDelete = 2f;

    private const int standardGoldToAdd = 6;
    private const int doubleGoldToAdd = 12;

    // Use this for initialization
    void Start () {
        isPojoDeadh = false;
        cameras = (GameObject.FindGameObjectsWithTag("MainCamera") as GameObject[]).FirstOrDefault();
        pojo = (GameObject.FindGameObjectsWithTag("Player") as GameObject[]).FirstOrDefault();

        if (tag == "Player")
        {
            nestlingSpeedUp = 0f;
            nestlingSpeedForward = GetComponent<PlayerMovement>().forwardSpeed;
        }
        else
        {
            nestlingSpeedForward = 0f;
            nestlingSpeedUp = 0f;
        }
	}
	
	// Update is called once per frame
	void Update () {
        timeToNextBonuses -= Time.deltaTime;

        if (tag != "Player" && bonusGameObject == null)
            transform.position += new Vector3(nestlingSpeedForward + 6f, nestlingSpeedUp, 0) * Time.deltaTime;
        else if(bonusGameObject != null && tag != "Player")
        {
            float speed = pojo.GetComponent<PlayerMovement>().forwardSpeed * 5f;
            transform.position = Vector3.MoveTowards(transform.position, bonusGameObject.transform.position, speed * Time.deltaTime);
        }

        if (isPojoDeadh)
        {
            timeToNestlingsDelete -= Time.deltaTime;

            if(timeToNestlingsDelete < 0f)
            {
                Destroy(gameObject);
            }
        }

        GameObject[] findGameObject = GameObject.FindGameObjectsWithTag("Gold") as GameObject[];

        if (bonusGameObject == null)
        {
            for (int i = 0; i < findGameObject.Length; i++)
            {
                if (PointInTriangle(new Vector2(findGameObject[i].transform.position.x, findGameObject[i].transform.position.y), new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x + angleOfView.x, transform.position.y + angleOfView.y), new Vector2(transform.position.x + angleOfView.x, transform.position.y - angleOfView.y)))
                {
                    bonusGameObject = findGameObject[i];
                }
            }
        }
	}
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Gold")
        {
            GetComponent<AudioSource>().PlayOneShot(goldClip);

            if (GameSettings.isAdwordsView)
            {
                pojo.GetComponent<Pojo>().gold += doubleGoldToAdd;
            }
            else
            {
                pojo.GetComponent<Pojo>().gold += standardGoldToAdd;
            }
            
            other.gameObject.AddComponent<DestroyGold>();
            other.tag = "Untagged";
            bonusGameObject = null;
        }
    }

    void OnDrawGizmos()
    {
        if (isViewGizmosDistance)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + angleOfView.x, transform.position.y + angleOfView.y, 0));
            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + angleOfView.x, transform.position.y - angleOfView.y, 0));
        }
    }

    #region point checking inside triangle helphersMethod
    float sign(Vector2 point1, Vector2 point2, Vector2 point3)
    {
        return (point1.x - point3.x) * (point2.y - point3.y) - (point2.x - point3.x) * (point1.y - point3.y);
    }

    bool PointInTriangle(Vector2 pointForChecking, Vector2 point1, Vector2 point2, Vector2 point3)
    {
        bool b1, b2, b3;

        b1 = sign(pointForChecking, point1, point2) < 0.0f;
        b2 = sign(pointForChecking, point2, point3) < 0.0f;
        b3 = sign(pointForChecking, point3, point1) < 0.0f;

        return ((b1 == b2) && (b2 == b3));
    }
    #endregion
}
