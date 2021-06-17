using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private float azimuth;

    public enum State
    {
        Standing = 0,
        Walking = 1,
        Attacking = 2
    }

    public State CharacterState;
    public Waypoint MatrixWaypoint;

    public Collider Sword;
    public float Damage = 5.0f;
    public bool HasBeenHit;

    private Vector3 target;
    private Animator animator;
    
    void Start()
    {
        animator = this.GetComponent<Animator>();
        azimuth = transform.rotation.eulerAngles.y;
        target = transform.position;

        IsAlive = true;
    }

    public float RotationSpeed = 360.0f;
    public float WalkSpeed = 10.0f;
    public float MinDistance = 0.1f;

    public bool IsAlive;

    public void EndAttack()
    { 
        CharacterState = State.Standing;
        HasBeenHit = false;
    }

    void Update()
    {
        animator.SetBool("IsAlive", IsAlive);

        Sword.enabled = IsAlive && CharacterState == State.Attacking;

        if (IsAlive)
        {
            animator.SetInteger("State", (int) CharacterState);

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //var hits = Physics.RaycastAll(ray);

            if (Input.GetKeyDown(KeyCode.F))
                IsAlive = false;

            if (Input.GetMouseButton(0) && CharacterState != State.Attacking)
            {

                if (Physics.Raycast(ray, out var hit))
                {
                    if(Input.GetMouseButtonDown(0))
                    {
                        var waypoint = Instantiate(MatrixWaypoint, hit.point, Quaternion.identity);
                        waypoint.SetColor(Color.green);
                    }
                    target = hit.point;
                }
            }

            var distance = target - this.transform.position;

            if(Input.GetMouseButtonDown(1))
            {
                HasBeenHit = false;
            }

            if(Input.GetMouseButton(1) || CharacterState == State.Attacking)
            {
                //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if(Physics.Raycast(ray, out var hit))
                {
                    target = transform.position;
                    CharacterState = State.Attacking;

                    var direction = hit.point - this.transform.position;
                    var desiredAzimuth = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                    azimuth = Mathf.MoveTowardsAngle(azimuth, desiredAzimuth, RotationSpeed * Time.deltaTime);
                }
            }
            else if (distance.sqrMagnitude > MinDistance * MinDistance)
            {
                var desiredAzimuth = Mathf.Atan2(distance.x, distance.z) * Mathf.Rad2Deg;

                azimuth = Mathf.MoveTowardsAngle(azimuth, desiredAzimuth, RotationSpeed * Time.deltaTime);

                CharacterState = State.Walking;

                transform.position += transform.forward * WalkSpeed * Time.deltaTime;
            }
            else
            {
                CharacterState = State.Standing;
            }



            transform.rotation = Quaternion.Euler(0, azimuth, 0);
        }
    }
}
