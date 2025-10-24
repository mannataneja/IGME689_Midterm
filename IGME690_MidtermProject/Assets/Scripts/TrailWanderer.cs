using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;

[RequireComponent(typeof(CharacterController))]
public class TrailWanderer : MonoBehaviour
{
    public string animalName;

    public Camera playerSnapCam;
    public bool isVisible;
    [Header("Waypoints")]
    public List<Transform> trailPoints = new List<Transform>();

    [Header("Movement Settings")]
    public float moveSpeed = 2.5f;
    public float turnSpeed = 5f;
    public float waitTimeAtPoint = 2f;
    public bool loopPath = true;

    public Animator animator;

    private CharacterController characterController;
    private int currentIndex = 0;
    private bool isWaiting = false;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerSnapCam = GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>();
        StartCoroutine(StartingUp());
    }
    private IEnumerator StartingUp()
    {
        GameObject.FindWithTag("Player").GetComponent<PlayerSnapCamController>().animals.Add(gameObject);
        isWaiting = true;
        yield return new WaitForSeconds(1);
        isWaiting = false;
        GetComponent<ArcGISLocationComponent>().enabled = false;
        float randomOffsetX = UnityEngine.Random.Range(10, 15);
        float randomOffsetZ = UnityEngine.Random.Range(10, 15);

        // Randomly flip sign (so offset can go in any direction)
        if (UnityEngine.Random.value > 0.5f) randomOffsetX *= -1;
        if (UnityEngine.Random.value > 0.5f) randomOffsetZ *= -1;

        // Create the offset vector
        Vector3 offset = new Vector3(randomOffsetX, 0f, randomOffsetZ);

        transform.position += offset;
    }

    private void Update()
    {
        Vector3 targetPosition = playerSnapCam.WorldToViewportPoint(gameObject.transform.position);
        isVisible = targetPosition.z > 0 && targetPosition.x > 0 && targetPosition.x < 1 && targetPosition.y > 0 && targetPosition.y < 1;

        if (trailPoints.Count == 0 || isWaiting)
            return;
        
        MoveAlongTrail();
    }

    private void MoveAlongTrail()
    {
        Transform targetPoint = trailPoints[currentIndex];
        Vector3 directionToTarget = targetPoint.position - transform.position;
        directionToTarget.y = 0;

        if (directionToTarget.magnitude < 0.5f)
        {
            StartCoroutine(WaitAtWaypoint());
            return;
        }

        Vector3 moveDirection = directionToTarget.normalized;
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

        if (moveDirection.sqrMagnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    private IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        animator.SetTrigger("idle");

        yield return new WaitForSeconds(waitTimeAtPoint);

        if (loopPath)
            currentIndex = (currentIndex + 1) % trailPoints.Count;
        else
            currentIndex = Mathf.Min(currentIndex + 1, trailPoints.Count - 1);

        isWaiting = false;
        animator.SetTrigger("walk");
    }
}
