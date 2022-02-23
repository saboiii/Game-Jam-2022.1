using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{

	public GameObject followTarget;
	private Vector3 targetPos;
	public float moveSpeed;
	private static bool cameraExists;
	public GameObject trueHugo;
	//public GameObject canvasMain;
	public Canvas canvas;
	public Camera thisOne;


	// Start is called before the first frame update
	void Start()
	{
		DontDestroyOnLoad(transform.gameObject);
		if (!cameraExists)
		{
			cameraExists = true;
			DontDestroyOnLoad(transform.gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
		canvas = GameObject.FindWithTag("Main Canvas").GetComponent<Canvas>();
	}

	// Update is called once per frame
	void Update()
	{
		if (followTarget != null)
		{
			targetPos = new Vector3(followTarget.transform.position.x, followTarget.transform.position.y, transform.position.z);
			transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
		}
		else
		{
			followTarget = GameObject.FindWithTag("Player");
			if (canvas != null)
			{
				canvas.worldCamera = thisOne;
			}
			else
			{
				canvas = GameObject.FindWithTag("Main Canvas").GetComponent<Canvas>();
				canvas.worldCamera = thisOne;
			}
		}
	}
}
