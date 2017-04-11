using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;
using Assets.Scripts;

public class PlayerMovement : MonoBehaviour {
    public float virtualWidth = 1920.0f;
    public float virtualHeight = 1080.0f;
    Matrix4x4 matrix;

    public bool isDeath = false;
    public bool paused;

    public Texture2D pausedButton;
    public Texture2D playButton;

    public AudioClip flappingClip;
    public AudioClip bonusClip;
    public AudioClip goldEggClip;

    bool playerIsOutOfGame = false;
    public float cameraTopBounds;
    public float cameraBottomBounds;

    Vector3 velocity = Vector3.zero;
    public Vector3 gravity;
    public Vector3 flapVelocity;
    public int MaxSpeed;
    public float forwardSpeed = 1f;
    public float bonusForwardSpeed = 0f;
    private float changeForwardSpeedTime = 15.0f;
    GameObject camera;


    private float centerPositionX;
    private float centerPositionY;  
    private const int standardGoldToAdd = 6;
    private const int doubleGoldToAdd = 12;
    private const float hungerPercentToChange = 25f;

    bool didFlap = false;

    // Use this for initialization
    void Start()
    {
        matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / virtualWidth, Screen.height / virtualHeight, 1.0f));
        GameObject cameras = GameObject.Find("Main Camera Game");
        Bounds bounds = cameras.GetComponent<CameraTracksPlayer>().OrthographicBounds(Camera.main);
        cameraTopBounds = cameras.transform.position.y + (bounds.size.y/2) - (bounds.size.y/10);
        cameraBottomBounds = cameras.transform.position.y - (bounds.size.y / 2) + (bounds.size.y/10);
        camera = cameras;
        camera.GetComponent<SaveGameManager>().LoadState(camera);
        GetComponent<Animation>().enabled = true;
        gameObject.AddComponent<Pojo>();
        GetComponent<Pojo>().nestling = new System.Collections.Generic.List<GameObject>();
        GetComponent<Pojo>().nestling.Add((GameObject)Resources.Load("Nestlings/Nestling_I", typeof(GameObject)));
        GetComponent<Pojo>().nestling.Add((GameObject)Resources.Load("Nestlings/Nestling_II", typeof(GameObject)));
        GetComponent<Pojo>().nestling.Add((GameObject)Resources.Load("Nestlings/Nestling_III", typeof(GameObject)));

	centerPositionX = virtualWidth / 2f;
	centerPositionY = virtualHeight / 2f;
        
        PlayMusic();
        paused = false;
    }

    public void PlayMusic()
    {
        AudioSource audio = GetComponent<AudioSource>();

        if (GameSettings.isMusicActive)
        {
            audio.enabled = true;
        }
        else
        {
            audio.enabled = false;
        }
    }

    public void Bounce()
    {
        didFlap = true;
        FixedUpdate();
    }

    void FixedUpdate()
    {
            if (GetComponent<Pojo>().IsGhostBonus && transform.position.y < cameraBottomBounds)
            {
                velocity.x = forwardSpeed + bonusForwardSpeed;
                velocity.y += flapVelocity.y;

                if (didFlap == true)
                {
                    didFlap = false;
                    velocity += flapVelocity;
                }

                velocity = Vector3.ClampMagnitude(velocity, MaxSpeed);

                transform.position += velocity * Time.deltaTime;
            }
            else if (transform.position.y < cameraTopBounds)
            {
                velocity += gravity * Time.deltaTime;
                velocity.x = forwardSpeed + bonusForwardSpeed;

                if (didFlap == true)
                {
                    didFlap = false;
                    velocity += flapVelocity;
                }
                velocity = Vector3.ClampMagnitude(velocity, MaxSpeed);

                transform.position += velocity * Time.deltaTime;
            }
            else if (!GetComponent<Pojo>().IsInvertedFlight && transform.position.y > cameraTopBounds)
            {
                velocity += gravity * Time.deltaTime;
                velocity.x = forwardSpeed + bonusForwardSpeed;

                velocity = Vector3.ClampMagnitude(velocity, MaxSpeed);

                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                velocity += flapVelocity * Time.deltaTime;
                velocity.x = forwardSpeed + bonusForwardSpeed;

                velocity = Vector3.ClampMagnitude(velocity, MaxSpeed);

                transform.position += velocity * Time.deltaTime;
            }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDeath)
        {
            changeForwardSpeedTime -= Time.deltaTime;

            if (Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.Space))
            {
                if (!playerIsOutOfGame)
                {
                    GetComponent<AudioSource>().PlayOneShot(flappingClip);

                    if (GetComponent<Animation>().isPlaying)
                        GetComponent<Animation>().Stop("Pojo_Flying");

                    GetComponent<Animation>().Play("Pojo_Flying");
                    didFlap = true;
                }
            }

            if(changeForwardSpeedTime < 0)
            {
                forwardSpeed += 0.2f;
                changeForwardSpeedTime = 9.0f;
            }
        }
    }

    void OnGUI()
    {
        GUI.matrix = matrix;

	float playButtonWidth = 300f;
	float playButtonHeight = 200f;

        if (!paused)
        {
            if (GUI.Button(new Rect(0, 0, 100f, 100f), pausedButton, GUIStyle.none))
            {
                Time.timeScale = 0;
                paused = true;
            }
        }
        else
        {
            if (GUI.Button(new Rect(centerPositionX - playButtonWidth/2f, centerPositionY - playButtonHeight/2f, playButtonWidth, playButtonHeight), playButton, GUIStyle.none))
            {
                Time.timeScale = 1;
                paused = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!GetComponent<Pojo>().IsGhostBonus)
        {
            if (other.tag == "Obstacles")
            {
                if (GetComponent<Animation>().isPlaying)
                    GetComponent<Animation>().Stop("Pojo_Flying");

                if (!isDeath)
                {
                    isDeath = true;
                    GetComponent<Animation>().Play("Pojo_Death");
                    GetComponent<Pojo>().IsInvertedFlight = false;
                    (GameObject.Find("Main Camera Game")).GetComponent<Boid>().boids.RemoveAt(0);
                    GetComponent<ResultDashboard>()._tempDistance = GetComponent<Pojo>().points;
                    isDeath = true;
                }
            }
        }
        if (other.tag == "DeathArea")
        {
            playerIsOutOfGame = true;
        }
        if (other.tag == "Gold")
        {
            GetComponent<AudioSource>().PlayOneShot(goldEggClip);

            if (GameSettings.isAdwordsView)
            {
                GetComponent<Pojo>().gold += doubleGoldToAdd;
            }
            else {
                GetComponent<Pojo>().gold += standardGoldToAdd;
            }

            other.gameObject.AddComponent<DestroyGold>();
            other.tag = "Untagged";
        }
        if (other.tag == "Bonuses")
        {
            GetComponent<AudioSource>().PlayOneShot(bonusClip);

            if (other.gameObject.name == "Flash")
            {
                forwardSpeed = forwardSpeed + (forwardSpeed * 0.10f);
                Destroy(other.gameObject);
            }
            if (other.gameObject.name == "Ghost")
            {
                gameObject.AddComponent<GhostBonus>();
                Destroy(other.gameObject);
            }
            if (other.gameObject.name == "InvertedFlight")
            {
                gameObject.AddComponent<InvertedFlightBonus>();
                Destroy(other.gameObject);
            }
            if (other.gameObject.name == "Slow")
            {
                forwardSpeed = forwardSpeed - (forwardSpeed * 0.10f);
                Destroy(other.gameObject);
            }
            if(other.gameObject.name == "Food")
            {
                GetComponent<Pojo>().hunger += hungerPercentToChange;
                Destroy(other.gameObject);
            }
        }   
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "DeathArea")
        {
            playerIsOutOfGame = false;
        }
    }
}
