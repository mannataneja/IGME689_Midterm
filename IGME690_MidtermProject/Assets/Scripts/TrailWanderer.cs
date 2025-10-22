using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class TrailWanderer : MonoBehaviour
{
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
    }

    private void Update()
    {
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
