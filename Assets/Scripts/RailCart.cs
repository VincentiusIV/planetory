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
    public Item Content { get; set; }
    public float speed = 1.0f, lookAheadDist = 0.3f;
    public bool IsHalted { get; private set; }
    public float headOnCollisionDot = -0.2f;
    public GameObject[] explosionPrefab;
    public MeshRenderer itemIconRenderer;

    private RailNode currentRail;
    private Rigidbody Rigidbody { get; set; }
    private List<RailCart> nearbyCarts = new List<RailCart>();

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }
    private void Start()
    {

        NodeSlot newSlot = NodeGrid.Instance.GetSlot(transform.position);
        currentRail = newSlot.HasNodes ? newSlot.nodes[Random.Range(0, newSlot.nodes.Count)] as RailNode : null;
        Debug.Assert(currentRail != null, "RailCart was not spawned on a rail node!");
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
            return;

        if(!currentRail.Contains(this))
        {
            RailNode nextRail = currentRail.Next(Forward);
            if(nextRail != null)
            {
                IsHalted = false;
                currentRail.OnCartExit(this);
                currentRail = nextRail;
                currentRail.OnCartEnter(this);
            }
            else
            {
                IsHalted = true;
                if(!currentRail.HasAwaitingCart)
                    currentRail.Push(this);
            }
        }

        if(!IsHalted)
        {
            IsHalted = false;
            Vector3 lastPosition = transform.position;
            Vector3 move = Forward * speed * Time.fixedDeltaTime;
            transform.position += move;
            transform.position = currentRail.Constrain(transform.position);
            Vector3 delta = (transform.position - lastPosition);
            //Vector3 projectedDelta = Vector3.ProjectOnPlane(delta, -Vector3.forward);
            if(delta.magnitude > 0.00001)
            {
                var angle = Mathf.Atan2(delta.normalized.y, delta.normalized.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5);
            }
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

            Vector3 dir = cart.transform.position - transform.position;
            if (Vector3.Dot(dir, Forward) > 0.0f)
                return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        RailCart otherCart = other.GetComponent<RailCart>();
        if(otherCart != null)
        {
            nearbyCarts.Add(otherCart);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        RailCart otherCart = other.GetComponent<RailCart>();
        if (otherCart != null)
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
