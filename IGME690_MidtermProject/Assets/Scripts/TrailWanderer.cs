using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;

[RequireComponent(typeof(CharacterController))]
public class TrailWanderer : MonoBehaviour
{
    public Camera playerSnapCam;
    public bool visible;
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
        isWaiting = true;
        yield return new WaitForSeconds(1);
        isWaiting = false;
        GetComponent<ArcGISLocationComponent>().enabled = false;
    }

    private void Update()
    {
        Vector3 targetPosition = playerSnapCam.WorldToViewportPoint(gameObject.transform.position);
        visible = targetPosition.z > 0 && targetPosition.x > 0 && targetPosition.x < 1 && targetPosition.y > 0 && targetPosition.y < 1;

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
        transform.position = new Vector3(transform.position.x, -2.5f, transform.position.z);
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
