using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class RailCart : MonoBehaviour
{
    public Vector3 Forward => transform.right;
    public Vector3 Right => transform.up;
    public Vector3 FrontPosition => transform.position + Forward * forwardLookDistance;
    public Item Content { get; set; }
    public float maxSpeed = 1.0f, lookAheadDist = 0.3f;
    public float acceleration = 1.0f;
    public bool IsHalted { get; private set; }
    public float headOnCollisionDot = -0.2f;
    public float forwardCollisionDot = 0.5f;
    public float rightCollisionDot = 0;
    public float forwardLookDistance = 0.55f;

    public float Speed { get; private set; }

    public GameObject[] explosionPrefab;
    public MeshRenderer itemIconRenderer;

    private RailNode currentRail;
    private Rigidbody Rigidbody { get; set; }
    private List<RailCart> nearbyCarts = new List<RailCart>();

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>(); 
        gameObject.SetActive(false);
    }
    private void Start()
    {
    }

    public void Setup(RailNode currentRail)
    {
        this.currentRail = currentRail;
        this.currentRail.OnCartEnter(this);
        gameObject.SetActive(true);
    }

    public void SetItem(Item item)
    {
        Debug.Assert(Content == null);
        Content = item;
        itemIconRenderer.material.SetTexture("_BaseMap", item.Icon);
    }

    private void Update()
    {
        itemIconRenderer.transform.rotation = Quaternion.identity;
    }

    private void FixedUpdate()
    {
        if (AreNearbyCartsInFront())
        {
            Speed = 0.0f;
            return;
        }

        if(!currentRail.Contains(this, FrontPosition))
        {
            RailNode nextRail = currentRail.Next(Forward);
            if(nextRail != null)
            {
                IsHalted = nextRail.HasCart;
                if (!currentRail.Contains(this, transform.position))
                {
                    currentRail.OnCartExit(this);
                    currentRail = nextRail;
                    currentRail.OnCartEnter(this);
                }
            }
            else
            {
                IsHalted = true;
                if(!currentRail.HasAwaitingCart)
                    currentRail.Push(this);
            }
        }

        if (!IsHalted)
        {
            Speed += acceleration * Time.fixedDeltaTime;
            Speed = Mathf.Min(Speed, maxSpeed);

            IsHalted = false;
            Vector3 lastPosition = transform.position;
            Vector3 move = Forward * Speed * Time.fixedDeltaTime;
            transform.position += move;
            transform.position = currentRail.Constrain(transform.position);

            Vector3 constrainedFront = currentRail.Constrain(FrontPosition);
            Vector3 lookdir = (constrainedFront - transform.position).normalized;
            if (lookdir.magnitude > 0.00001)
            {
                var angle = Mathf.Atan2(lookdir.y, lookdir.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5);
            }
        }
        else
        {
            Speed = 0.0f;
        }
    }

    private bool AreNearbyCartsInFront()
    {
        for (int i = 0; i < nearbyCarts.Count; i++)
        {
            RailCart cart = nearbyCarts[i];
            if(cart == null)
            {
                nearbyCarts.RemoveAt(i--);
                continue;
            }

            // check head-on collision
            if (Vector3.Dot(cart.Forward, Forward) < headOnCollisionDot)
            {
                Explode();
                return true;
            }

            Vector3 dir = (cart.transform.position - transform.position).normalized;

            if (Vector3.Dot(dir, Forward) > forwardCollisionDot)
                return true;

            if (Vector3.Dot(Right, dir) > rightCollisionDot) 
                return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        RailCart otherCart = other.GetComponent<RailCart>();
        if(otherCart != null && otherCart != this)
        {
            nearbyCarts.Add(otherCart);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
            return;

        RailCart otherCart = other.GetComponent<RailCart>();
        if (otherCart != null && otherCart != this)
        {
            if (nearbyCarts.Contains(otherCart))
                nearbyCarts.Remove(otherCart);
        }
    }

    public void Kill()
    {
        currentRail?.OnCartExit(this);
        Destroy(gameObject);
    }

    public void Explode()
    {
        int randIdx = Random.Range(0, explosionPrefab.Length);
        GameObject explosion = Instantiate(explosionPrefab[randIdx]);
        explosion.transform.position = transform.position;
        Kill();
    }
}
