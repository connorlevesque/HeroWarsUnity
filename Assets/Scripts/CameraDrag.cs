using UnityEngine;
 
public class CameraDrag : MonoBehaviour
{
    public float dragSpeed = 8;
    public float mapBuffer;
    private Vector3 dragOrigin;

    private float leftBound;
    private float rightBound;
    private float upperBound;
    private float lowerBound;
 
    void Start()
    {
    	Camera camera = GetComponent<Camera>();
    	float camSize = camera.orthographicSize;
    	GridManager gM = GameObject.FindWithTag("GridManager").GetComponent<GridManager>();
    	leftBound = camSize * 1.78f - mapBuffer - 0.5f;
    	rightBound = gM.width + mapBuffer - 0.5f - camSize * 1.78f;
    	lowerBound = camSize - mapBuffer - 0.5f;
    	upperBound = gM.height + mapBuffer - 0.5f - camSize;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }
 
        if (!Input.GetMouseButton(0)) return;
 
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(pos.x * dragSpeed * -1, pos.y * dragSpeed * -1, 0);
 
        transform.position += move;
        dragOrigin = Input.mousePosition;

        SnapToBoundaries();
    }

    void SnapToBoundaries()
    {
    	// check left and right bounds
    	if (transform.position.x < leftBound)
    	{
    		transform.position += new Vector3(leftBound - transform.position.x, 0, 0);
    	} else if (transform.position.x > rightBound) {
    		transform.position += new Vector3(rightBound - transform.position.x, 0, 0);
    	}
    	// check upper and lower bounds
    	if (transform.position.y < lowerBound)
    	{
    		transform.position += new Vector3(0, lowerBound - transform.position.y, 0);
    	} else if (transform.position.y > upperBound) {
    		transform.position += new Vector3(0, upperBound - transform.position.y, 0);
    	}
    }

}
