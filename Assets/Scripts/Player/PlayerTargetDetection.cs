using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTargetDetection : MonoBehaviour
{
    [SerializeField]
    private LayerMask enemyLayer;
    [SerializeField]
    private List<GameObject> enemies = new();
    [SerializeField]
    private Image crosshair;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private float detectionRange;
    [SerializeField]
    private float detectionAngle;

    private GameObject closestTarget = null;

    private void Start()
    {
        //Change this for enemies later
        List<Dummy> dummies = FindObjectsByType<Dummy>();
        foreach (Dummy dummy in dummies)
        {
            enemies.Add(dummy.gameObject);
        }
    }

    private List<T> FindObjectsByType<T>()
    {
        throw new NotImplementedException();
    }

    private void Update()
    {
        if (enemies.Count > 0)
        {
            if (closestTarget == null)
            {
                closestTarget = enemies[0];
            }

            if (closestTarget != null)
            {
                crosshair.transform.position = cam.WorldToScreenPoint(closestTarget.transform.position);
                if (crosshair.transform.position.x > 0 && crosshair.transform.position.x < Screen.currentResolution.width &&
                    crosshair.transform.position.y > 0 && crosshair.transform.position.y < Screen.currentResolution.height)
                {
                    crosshair.gameObject.SetActive(true);
                }
            }
            else
            {
                crosshair.gameObject.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (enemies.Count > 0)
        {
            if (closestTarget == null)
            {
                closestTarget = enemies[0];
            }

            float distanceToClosest = Vector3.Distance(gameObject.transform.position, closestTarget.transform.position);
            foreach (GameObject target in enemies)
            {
                float distanceToTarget = Vector3.Distance(gameObject.transform.position, target.transform.position);
                if (distanceToTarget < distanceToClosest)
                {
                    closestTarget = target;
                    distanceToClosest = distanceToTarget;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((enemyLayer.value & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            if (enemies.Count == 0)
            {
                closestTarget = other.gameObject;
            }

            enemies.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((enemyLayer.value & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
        {
            enemies.Remove(other.gameObject);

            if (enemies.Count == 0)
            {
                crosshair.gameObject.SetActive(false);
                closestTarget = null;
            }
        }
    }

    public void OnDisable()
    {
        crosshair.enabled = false;
    }

    public void ResetVariables(bool grabComplete, GameObject target)
    {
        if (grabComplete)
        {
            enemies.Remove(target);
        }
        crosshair.gameObject.SetActive(false);
        crosshair.enabled = true;
    }
}
