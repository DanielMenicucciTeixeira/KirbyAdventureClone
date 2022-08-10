using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    //Reference to the main camera
    Camera GameCamera;

    //Camer's ViewporSize (Original game has close to 1.1696, default Unity Camera is close to 0.8)
    public float ViewportSize = 1.1696f;

    //Camera's Aspect Ratio (original game is 1:0,95)
    public Vector2 AspectRatio = new Vector2(1.0f,0.95f);

    //Camera's starting height
    float StartHeight;

    //By how much the camera height must change between Area transitions
    float TransitionHeightDisplacemet;

    //Maximum and minimum X value the camera can assume to avoid going out fo bounds
    Vector2 PositionXBounds;

    //Reference to the main character GameObject
    public Transform Player;

    //Reference to the Stages Sprite
    public Sprite StageSprite;

    //Reference to the HUD
    public HUD GameHUD;
    Sprite HUDSprite;

    //Before the game starts
    void Awake()
    {
        GameCamera = GetComponent<Camera>();
        HUDSprite = GameHUD.GetComponent<SpriteRenderer>().sprite;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Ajust the camera's aspect ratio
        GameCamera.aspect = AspectRatio.x / AspectRatio.y;

        //Set the Camera's viewport size
        GameCamera.orthographicSize = ViewportSize;

        //Set PositonXBounds
        PositionXBounds.x = GameCamera.orthographicSize * GameCamera.aspect;
        PositionXBounds.y = (StageSprite.bounds.size.x - GameCamera.orthographicSize*GameCamera.aspect);

        //Set Height depende variables
        StartHeight = (StageSprite.bounds.size.y * (2.0f / 3.0f)) + GameCamera.orthographicSize - HUDSprite.bounds.size.y;
        TransitionHeightDisplacemet = StageSprite.bounds.size.y / 3.0f;

        //Set the Camera's starting position
        transform.position = new Vector3(Player.position.x, StartHeight, transform.position.z);

        //Set the HUD's starting position
        GameHUD.transform.position = new Vector3(transform.position.x - ViewportSize * GameCamera.aspect, transform.position.y - ViewportSize, -GameCamera.nearClipPlane);


        if (!Player)
        {
            Debug.LogWarning("No Player assigned to Camera Manager");
            gameObject.SetActive(false);
        }
        else if (!StageSprite)
        {
            Debug.LogWarning("No Stage assigned to Camera Manager");
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Player.position.x < PositionXBounds.x)
        {
            transform.position = new Vector3(PositionXBounds.x, transform.position.y, transform.position.z);
        }
        else if (Player.position.x > PositionXBounds.y)
        {
            transform.position = new Vector3(PositionXBounds.y, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(Player.position.x, transform.position.y, transform.position.z);
        }
    }
}
