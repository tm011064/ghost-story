using System;
using System.Linq;
using UnityEngine;

public partial class PlayerController : BaseCharacterController
{
  private const string TRACE_TAG = "PlayerController";

  public WallJumpSettings WallJumpSettings = new WallJumpSettings();

  public JumpSettings JumpSettings = new JumpSettings();

  public RunSettings RunSettings = new RunSettings();

  public ClimbSettings ClimbSettings = new ClimbSettings(); // TODO (old): make those behaviors that can be attached to the controller

  public SlideSettings SlideSettings = new SlideSettings();

  public CrouchSettings CrouchSettings = new CrouchSettings();

  public Vector2 BoxColliderOffsetWallAttached = Vector2.zero;

  public Vector2 BoxColliderSizeWallAttached = Vector2.zero;

  [HideInInspector]
  public float AdjustedGravity;

  [HideInInspector]
  public Animator Animator;

  [HideInInspector]
  public GameObject Sprite;

  [HideInInspector]
  public SpriteRenderer SpriteRenderer;

  [HideInInspector]
  public BoxCollider2D EnvironmentBoxCollider;

  [HideInInspector]
  public BoxCollider2D EnemyBoxCollider;

  [HideInInspector]
  public Vector3 SpawnLocation;

  [HideInInspector]
  public GameObject CurrentPlatform = null;

  [HideInInspector]
  public PlayerState PlayerState;

  [HideInInspector]
  public AbstractWeaponBehaviour[] Weapons = new AbstractWeaponBehaviour[0];

  [HideInInspector]
  public Vector2 StandIdleEnvironmentBoxColliderSize;

  [HideInInspector]
  public PlayerHealthBehaviour Health;

  private RaycastHit2D _lastControllerColliderHit;

  private Vector3 _velocity;

  private WallJumpControlHandler _reusableWallJumpControlHandler;

  private WallJumpEvaluationControlHandler _reusableWallJumpEvaluationControlHandler;

  public event Action<GroundedPlatformChangedInfo> GroundedPlatformChanged;

  public event Action JumpedThisFrame;

  void Awake()
  {
    Health = this.GetComponentOrThrow<PlayerHealthBehaviour>();

    InitializeBoxCollider();

    InitializeSpriteAndAnimator();

    InitializeCharacterPhysicsManager();

    InitializeWeapons();

    _reusableWallJumpControlHandler = new WallJumpControlHandler(this);

    _reusableWallJumpEvaluationControlHandler = new WallJumpEvaluationControlHandler(this);

    AdjustedGravity = JumpSettings.Gravity;

    OnAwake();

    PushControlHandler(CreateDefaultPlayerControlHandler());
  }

  protected virtual void OnAwake()
  {
  }

  private void InitializeWeapons()
  {
    Weapons = GetComponentsInChildren<AbstractWeaponBehaviour>(true);
  }

  private void InitializeCharacterPhysicsManager()
  {
    CharacterPhysicsManager = GetComponent<CharacterPhysicsManager>();

    CharacterPhysicsManager.BoxCollider = EnvironmentBoxCollider;

    CharacterPhysicsManager.ControllerCollided += OnControllerCollided;

    CharacterPhysicsManager.ControllerLostGround += OnControllerLostGround;
  }

  private void InitializeBoxCollider()
  {
    EnvironmentBoxCollider = GetEnvironmentCollider();

    StandIdleEnvironmentBoxColliderSize = EnvironmentBoxCollider.size;

    EnemyBoxCollider = GetEnemyCollider();
  }

  private void InitializeSpriteAndAnimator()
  {
    var childTransform = this.GetChildGameObject("SpriteAndAnimator");

    Sprite = childTransform.gameObject;

    SpriteRenderer = childTransform.gameObject.GetComponent<SpriteRenderer>();

    Animator = childTransform.gameObject.GetComponent<Animator>();

    AnimationHashLookup.Register(
      "Idle",
      "Run Start",
      "Run",
      "Freeze",
      "Jump",
      "Fall",
      "Enemy Contact Knockback");
  }

  private BoxCollider2D GetEnvironmentCollider()
  {
    var spriteAndAnimator = transform.GetChildGameObject("SpriteAndAnimator");

    var colliderObject = spriteAndAnimator.transform.GetChildGameObject("Environment Collider");

    var collider = colliderObject.GetComponentOrThrow<BoxCollider2D>();

    return collider;
  }

  private BoxCollider2D GetEnemyCollider()
  {
    var spriteAndAnimator = transform.GetChildGameObject("SpriteAndAnimator");

    var colliderObject = spriteAndAnimator.transform.GetChildGameObject("Enemy Collider");

    var collider = colliderObject.GetComponentOrThrow<BoxCollider2D>();

    return collider;
  }

  public float CalculateMaxJumpHeight()
  {
    return RunSettings.EnableRunning
      ? JumpSettings.RunJumpHeight
      : Math.Max(JumpSettings.StandJumpHeight, JumpSettings.WalkJumpHeight);
  }

  public bool IsFacingRight()
  {
    return Sprite.transform.localScale.x > 0f;
  }

  public bool IsGrounded()
  {
    return CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below == true;
  }

  public bool IsAirborne()
  {
    return CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below == false;
  }

  public void OnFellFromClimb()
  {
    if (ClimbSettings.MinFallDuration <= 0f)
    {
      return;
    }

    ClimbSettings.EnableLadderClimbing = false;

    Invoke("EnableClimbing", ClimbSettings.MinFallDuration);
  }

  public void OnJumpedThisFrame()
  {
    Logger.Info("Ground Jump executed.");

    var handler = JumpedThisFrame;

    if (handler != null)
    {
      handler.Invoke();
    }
  }

  private void SetCurrentPlatform(GameObject gameObject)
  {
    var handler = GroundedPlatformChanged;

    if (handler != null)
    {
      var previousGameObject = CurrentPlatform;

      CurrentPlatform = gameObject;

      GroundedPlatformChanged(new GroundedPlatformChangedInfo(previousGameObject, CurrentPlatform));
    }
    else
    {
      CurrentPlatform = gameObject;
    }
  }

  void OnControllerLostGround()
  {
    if (CurrentPlatform != null)
    {
      SetCurrentPlatform(null);
    }
  }

  void OnControllerCollided(RaycastHit2D hit)
  {
    // bail out on plain old ground hits cause they arent very interesting
    if (hit.normal.y == 1f)
    {
      if (CurrentPlatform != hit.collider.gameObject)
      {
        SetCurrentPlatform(hit.collider.gameObject);
      }

      return;
    }

    // TODO (old): these methods should be optimized and put into constant field...
    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Platforms"))
    {
      if (WallJumpSettings.EnableWallJumps
        && !CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Below
        && CharacterPhysicsManager.LastMoveCalculationResult.DeltaMovement.y < 0f
        && (CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.CharacterWallState & CharacterWallState.OnWall) != 0)
      {
        // wall jumps work like this: if the player makes contact with a wall, we want to keep track how long he moves towards the
        // wall (based on input axis). If a certain threshold is reached, we are "attached to the wall" which will result in a reduced "slide down"
        // gravity. When a player is on a wall, he can not detach by pressing the opposite direction - the only way to detach is to jump.
        if (ActiveControlHandler != _reusableWallJumpControlHandler
          && ActiveControlHandler != _reusableWallJumpEvaluationControlHandler)
        {
          var wallJumpEnabledTime =
            WallJumpSettings.WallJumpEnabledTime >= 0f
              ? WallJumpSettings.WallJumpWallEvaluationDuration + WallJumpSettings.WallJumpEnabledTime
              : -1f;

          // new event, start evaluation
          if (CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Left)
          {
            _reusableWallJumpControlHandler.Reset(wallJumpEnabledTime, Direction.Left, WallJumpSettings);

            _reusableWallJumpEvaluationControlHandler.Reset(WallJumpSettings.WallJumpWallEvaluationDuration, Direction.Left, WallJumpSettings);

            PushControlHandlers(_reusableWallJumpControlHandler, _reusableWallJumpEvaluationControlHandler);
          }
          else if (CharacterPhysicsManager.LastMoveCalculationResult.CollisionState.Right)
          {
            _reusableWallJumpControlHandler.Reset(wallJumpEnabledTime, Direction.Right, WallJumpSettings);

            _reusableWallJumpEvaluationControlHandler.Reset(WallJumpSettings.WallJumpWallEvaluationDuration, Direction.Right, WallJumpSettings);

            PushControlHandlers(_reusableWallJumpControlHandler, _reusableWallJumpEvaluationControlHandler);
          }
        }
      }
    }
  }

  private void EnableClimbing()
  {
    ClimbSettings.EnableLadderClimbing = true;
  }

  public bool IsAttacking()
  {
    return Weapons.Any(wh => wh.IsAttacking());
  }

  protected virtual PlayerControlHandler CreateDefaultPlayerControlHandler()
  {
    return new DefaultPlayerControlHandler(this);
  }

  public void FlipHorizontalSpriteScale()
  {
    Sprite.transform.localScale = Sprite.transform.localScale.SetX(Sprite.transform.localScale.x * -1);
  }
}