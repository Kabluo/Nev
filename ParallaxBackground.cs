using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private Vector3 deltaMovement;

    private Sprite sprite;
    private Texture2D texture;
    private float textureUnitSizeX;
    private float offsetPositionX;
    private float textureUnitSizeY;
    private float offsetPositionY;

    [SerializeField] Vector2 parallaxMultiplier;

    /*
    throws error when returning to main menu due to the camera object being destroyed, fix later
    */

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.SetParent(null); //remove from parent, helps keep things tidy in a prefab this way instead of making each object seperate
        cameraTransform = Camera.main.transform;
        gameObject.transform.position = new Vector3(cameraTransform.position.x, cameraTransform.position.y);
        lastCameraPosition = cameraTransform.position;

        sprite = GetComponent<SpriteRenderer>().sprite;
        texture = sprite.texture;
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
        textureUnitSizeY = texture.height / sprite.pixelsPerUnit;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxMultiplier.x, deltaMovement.y * parallaxMultiplier.y); //move object relative to camera, multiply by parallax speed
        lastCameraPosition = cameraTransform.position;

        if(Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX) //if camera moves too far ahead compared to object
        {
            offsetPositionX = cameraTransform.transform.position.x - transform.position.x % textureUnitSizeX; //get remainder, add to new transform to make transition appear seamless
            transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y);
        }

        if(Mathf.Abs(cameraTransform.position.y - transform.position.y) >= textureUnitSizeY) //if camera moves too far ahead compared to object
        {
            offsetPositionY = cameraTransform.transform.position.y - transform.position.y % textureUnitSizeY; //get remainder, add to new transform to make transition appear seamless
            transform.position = new Vector3(transform.position.x, cameraTransform.position.y + offsetPositionY);
        }
    }
}
