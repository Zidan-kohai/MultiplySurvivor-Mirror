using Cinemachine;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour, IPlayer, IHumanoid, ISee, IMove
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 7;
    [SerializeField] private float runSpeed = 12;
    [SerializeField] private float fallspeed = 2;
    [SerializeField] private float maxSpeedOnPlayerFall = 3;
    [SerializeField] private float currrentSpeed = 5;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Transform groundChekcRaycastOrigin;
    [SerializeField] private float groundChekcRaycastHeight;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isOnGround;
    [SerializeField] private Vector3 velocity;
    //[SerializeField] private GameObject visualHandler;

    [Header("State")]
    [SerializeField] private PlayerState state;
    [SerializeField] private float timeToUpFromFall;
    [SerializeField] private float timeToDeathFromFall;
    [SerializeField] private float passedTimeFromFallToUp;
    [SerializeField] private float passedTimeFromFallToDeath;

    [Header("Components")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerAnimationController animationController;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private CharacterController characterController;

    [Header("Humanoids")]
    private List<IPlayer> players = new List<IPlayer>();

    [Header("Interactive")]
    [SerializeField] private float distanceToHelp;


    [SerializeField] ParticleSystem fire;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animationController.SetIMove(this);

        inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<InputManager>();

        if (isLocalPlayer)
        {
            virtualCamera.Priority = 5;
        }

    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        #region Movement

        Rotate();

        Move();

        CheckGround();

        Jump();

        ApplyGravity();

        characterController.Move(velocity * Time.deltaTime);

        #endregion

        if (state != PlayerState.Fall && state != PlayerState.Death)
            Help();

        if(inputManager.GetIsFireButtonDown)
        {
            fire.Play();
            RaycastHit hit;
            if(Physics.Raycast(fire.transform.position, fire.transform.forward, out hit))
            {
                Debug.Log("Hit: " + hit.transform.gameObject.name);
                if(hit.transform.TryGetComponent(out EnemyController enemy))
                {
                    enemy.Hit(5);
                }
            }
        }    
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public bool IsFallOrDeath()
    {
        return state == PlayerState.Fall || state == PlayerState.Death;
    }

    public bool IsFall()
    {
        return state == PlayerState.Fall;
    }

    public bool IsDeath()
    {
        return state == PlayerState.Death;
    }

    public void Fall()
    {
        NetworkServer.Destroy(gameObject);
        ChangeState(PlayerState.Fall);
    }

    public float Raising()
    {
        passedTimeFromFallToUp -= Time.deltaTime;

        float raisingPercent = GetPercentOfRaising();

        if (passedTimeFromFallToUp <= 0)
        {
            ChangeState(PlayerState.Idle);
        }

        return Mathf.Abs(passedTimeFromFallToUp / timeToUpFromFall - 1);
    }

    public float GetPercentOfRaising()
    {
        return Mathf.Abs(passedTimeFromFallToUp / timeToUpFromFall - 1);
    }

    public void AddHumanoid(IHumanoid IHumanoid)
    {
        if (IHumanoid.gameObject.TryGetComponent(out IPlayer player) && player != this)
        {
            players.Add(player);
        }
    }

    public void RemoveHumanoid(IHumanoid IHumanoid)
    {
        if (IHumanoid.gameObject.TryGetComponent(out IPlayer player))
        {
            players.Remove(player);
        }
    }

    public float MoveSpeed()
    {
        Vector3 move = new Vector3(velocity.x, 0, velocity.z);

        return move.magnitude;
    }

    public bool IsJump()
    {
        return !isOnGround && velocity.y != 0;
    }

    private void Rotate()
    {
        //First Person Controller
        float mouseX = inputManager.GetMouseDeltaX * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        //Third Person Conteroller

        //Vector3 velocityXY = new Vector3(velocity.x, 0, velocity.z);

        //if (velocityXY.magnitude < 0.01f) return;

        //Quaternion targetRotation = Quaternion.LookRotation(velocityXY.normalized, Vector3.up);
        //visualHandler.transform.rotation = Quaternion.Lerp(visualHandler.transform.rotation, targetRotation, Time.deltaTime * mouseSensitivity);
    }

    private void Move()
    {
        float moveHorizontal = inputManager.GetMoveHorizontal;
        float moveVertical = inputManager.GetMoveVertical;


        Vector3 move = Camera.main.transform.right * moveHorizontal + Camera.main.transform.forward * moveVertical;

        velocity.x = move.x * currrentSpeed;
        velocity.z = move.z * currrentSpeed;

        if (moveHorizontal == 0 && moveVertical == 0)
        {
            ChangeState(PlayerState.Idle);
        }
        else if(inputManager.GetIsLeftShift)
        {
            ChangeState(PlayerState.Run);
        }
        else
        {
            ChangeState(PlayerState.Walk);
        }
    }

    private void Jump()
    {
        if (inputManager.GetSpace && isOnGround)
        {
            float jumpVelocity = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
            velocity.y += jumpVelocity;
            isOnGround = false;
        }
    }

    private void ApplyGravity()
    {
        if (!isOnGround)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = 0;
        }
    }

    private void CheckGround()
    {
        isOnGround = Physics.Raycast(groundChekcRaycastOrigin.position, Vector3.down, groundChekcRaycastHeight, groundLayer);
    }

    private void ChangeState(PlayerState newState)
    {
        if (state == newState ||
            ((state == PlayerState.Fall) && (passedTimeFromFallToUp > 0))) return;

        state = newState;

        switch (state)
        {
            case PlayerState.Idle:
                
                animationController.Up();
                break;
            case PlayerState.Walk:
                currrentSpeed = walkSpeed;
                break;
            case PlayerState.Run:
                currrentSpeed = runSpeed;
                break;
            case PlayerState.Fall:
                currrentSpeed = fallspeed;
                animationController.Fall();
                passedTimeFromFallToUp = timeToUpFromFall;
                passedTimeFromFallToDeath = timeToDeathFromFall;
                break;
            case PlayerState.Death:
                break;
        }
    }

    private void Help()
    {
        if (CanHelp(out IPlayer player))
        {
            if (inputManager.GetIsE)
            {
                float helpPercent = player.Raising();
            }
        }
        else
        {
        }
    }

    private bool CanHelp(out IPlayer player)
    {
        for (int i = 0; i < players.Count; i++)
        {
            float distanceToPlayer = (players[i].GetTransform().position - transform.position).magnitude;
            if (players[i].IsFall() && distanceToPlayer < distanceToHelp)
            {
                player = players[i];
                return true;
            }
        }

        player = null;
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(groundChekcRaycastOrigin.position, Vector3.down * groundChekcRaycastHeight);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(fire.transform.position, fire.transform.forward * 100);
    }

}