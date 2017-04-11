using UnityEngine;
using System.Collections;

public class CameraTracksPlayer : MonoBehaviour {
    Transform player;
    Bounds cameraBounds;
    float offsetX;
    
	// Use this for initialization
	void Start () {
        GameObject player_go = GameObject.FindGameObjectsWithTag("Player")[0];
        cameraBounds = OrthographicBounds(Camera.main);

        if (player_go == null)
        {
            Debug.LogError("Couldn't find an object with tag 'Player'!");
            return;
        }

        player = player_go.transform;

        player_go.transform.position = new Vector2(transform.position.x - (cameraBounds.size.x/2), player_go.transform.position.y);

        offsetX = transform.position.x - player.position.x;
	}
	
	// Update is called once per frame
	void Update () {
        if(player != null)
        {
            Vector3 pos = transform.position;
            pos.x = player.position.x + offsetX;
            transform.position = pos;
        }
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            Vector3 pos = transform.position;
            pos.x = player.position.x + offsetX;
            transform.position = pos;
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 pos = transform.position;
            pos.x = player.position.x + offsetX;
            transform.position = pos;
        }
    }

    public Bounds OrthographicBounds(Camera camera)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        Bounds bounds = new Bounds(
            camera.transform.position,
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }
}
