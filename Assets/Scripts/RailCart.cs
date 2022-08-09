using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class RailCart : MonoBehaviour
{
    public Item Content { get; set; }
    public float speed = 1.0f;
    public bool IsHalted { get; private set; }

    private RailNode currentRail;
    private Rigidbody Rigidbody { get; set; }
    private List<RailCart> nearbyCarts = new List<RailCart>();

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    public void SetItem(Item item)
    {
        Debug.Assert(Content == null);
        Content = item;
    }

    private void FixedUpdate()
    {
        NodeSlot nextSlot = NodeGrid.Instance.GetSlot(transform.position + transform.forward * 0.5f);
        RailNode nextRail = nextSlot.node as RailNode;
        if (nextRail == null)
        {
            if (currentRail && !IsHalted)
                currentRail.Push(this);
            IsHalted = true;
        }
        else
        {
            IsHalted = false;
            currentRail = NodeGrid.Instance.GetSlot(transform.position).node as RailNode;
            Vector3 lastPosition = transform.position;
            Vector3 move = transform.forward * speed * Time.fixedDeltaTime;
            transform.position += move;
            transform.position = currentRail.Constrain(transform.position);
            Vector3 delta = (transform.position - lastPosition);
            Vector3 projectedDelta = Vector3.ProjectOnPlane(delta, -Vector3.forward);
            Quaternion targetRotation = Quaternion.LookRotation(projectedDelta.normalized, -Vector3.forward);
            if (targetRotation != Quaternion.identity)
                transform.rotation = targetRotation;

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
