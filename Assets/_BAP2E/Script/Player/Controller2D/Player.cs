using UnityEngine;
using System.Collections;
public enum PlayerState { Ground, Water, Jetpack }
public enum DoggeType { OverObject, HitObject }
[RequireComponent(typeof(Controller2D))]

public class Player : MonoBehaviour, ICanTakeDamage, IListener
{
    public int ID = 1;
    public Sprite headImage;
    [ReadOnly] public PlayerState PlayerState = PlayerState.Ground;        //set what state the player in
    public bool GodMode;        //active this then the player will not be hurt by anything
    float godTimer = 7;     //active the God timer in the given time
    float godmodeDamage = 50;

    GodmodeType godmodeType;
    //[Header("God setup")]
    [HideInInspector] public float godDamageRate = 0.5f;
    public AudioClip godSoundKeep;
    AudioSource godAudioSource;
    [HideInInspector] public Color godBlinkColor = new Color(0.2f, .2f, .2f, 1f);     //blink colour
    public SpriteRenderer imageCharacterSprite;     //the Image of the character
    [Header("Knock Back When Be Hit Option")]
    public bool knockBackBeHit = true;
    [Tooltip("If player take the damage >= this value, knock player back a lillte")]
    public float damageKnockBackBeHit = 30;
    public float knockbackForce = 10f;
   
    [Header("Setup Parameter")]
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    public float accGrounedOverride { get; set; }


    [Header("Setup parameter on ground")]
    public PlayerParameter GroundParameter;     //Ground parameters
    [Header("Setup parameter in water zone")]
    public PlayerParameter WaterZoneParameter;      //Water parameters

    private float moveSpeed;        //the moving speed, changed evey time the player on ground or in water
    private float maxJumpHeight;
    private float minJumpHeight;
    private float timeToJumpApex;

    public int numberOfJumpMax = 2;     //number jump allowed
    private int numberOfJumpLeft;
    public GameObject JumpEffect;       //spawn the object when jump if it is placed

    [Header("Health")]
    [Range(0, 1000)]
    public int maxHealth = 1000;     //limit the health by 100
    public int Health { get; private set; }
    public GameObject HurtEffect;       //spawn the effect object when get hit

    [HideInInspector]
    public bool isBlinking = false;
    public float blinking = 3;      //blinking time allowed
    public Color blinkColor = new Color(0.2f, .2f, .2f, 1f);        //blink colour

    [Header("Contact Enemy")]
    public int makeDamageOnEnemyHead = 30;
    public bool getDamageWhenContactEnemy = true;
    public bool useThisContactEnemyDamage = false;
    public int contactEnemyDamage = 10;
    public Transform feetPosition;

    [Header("Running and Sliding")]
    public float runningSpeed = 6;      //speed of running state
    [Range(0, 100)]
    public float runningEnegry = 100;       //full enegry is 100
    public float runningUseSpeed = 30;      //how fast is the enegry run out?
    public float runningRechargeSpeed = 10;     //how fast is the enegry fulfill?
    public bool isRunning { get; set; }
    private bool allowRechargeRunning = false;

    public float slidingTime = 0.5f;        //how long dose player can slide on the ground?
    public bool isSliding { get; set; }
    public ParticleSystem smokeParticleFX;

    [Header("Wall Slide")]
    public bool allowSlideWall;         //allow player do this action
    public bool allowJumpClimbSlideWall = false;
    public Vector2 wallJumpClimb = new Vector2(3, 10);      //Jump up force from the wall 
    public Vector2 wallJumpOff = new Vector2(3, 6);     //Jump out force from the wall 
    public Vector2 wallLeap = new Vector2(15, 9);

    public float wallSlideSpeedMax = 1.5f;      //the speed slide down from the wall 
    public float wallStickTime = .25f;
    float timeToWallUnstick;

    [Header("Jetpack")]
    [ReadOnly] public bool isJetPackAvailable = false;
    public float jetpackForce = 5;      //jet pack force
    [Range(0, 100)]
    public float jetpackEnegry = 0;       //the jetpack enegry bar
    public float jetpackUseSpeed = 25;      //how fast is the enegry run out?
    public bool useJetpackFireFX = false;
    public float jetpackRechargeSpeed = 7;  //how fast is the enegry fulfill?
    public GameObject jetpackEngine;        //the jetpack object, should be active when use the jetpack
    [HideInInspector]
    public bool isUsingJetpack = false;
    [ReadOnly] public bool allowRechargeJetpack = true;
    public AudioClip jetpackSoundEngine;
    AudioSource jetpackEngineAudio;

    [Header("Grenade")]
    public AudioClip throwGrenadeSound;
    public float angleThrow = 60;       //the angle to throw the bomb
    public float throwForce = 300;      //how strong?
    public Transform throwPosition;     //throw the bomb at this position
    public GameObject _Grenade;     //the bomb prefab object
    public float grenadeRate = 0.5f;
    float lastTimeThrowGrenade = 0;

    [Header("Sound")]
    public AudioClip showUpSound;
    public AudioClip finishSound;
    public AudioClip walkSound;
    public float walkSoundSpeed = 1.5f;
    [Range(0, 1)]
    public float walkSoundVolume = 0.5f;
    [Space]
    public AudioClip runSound;
    public float runSoundSpeed = 1.5f;
    [Range(0, 1)]
    public float runSoundVolume = 0.5f;
    AudioSource walkSoundSrc, runSoundSrc;
    [Space]
    public AudioClip jumpSound;
    [Range(0, 1)]
    public float jumpSoundVolume = 0.5f;
    public AudioClip landSound;
    [Range(0, 1)]
    public float landSoundVolume = 0.5f;
    public AudioClip wallSlideSound;
    [Range(0, 1)]
    public float wallSlideSoundVolume = 0.5f;
    public AudioClip hurtSound;
    [Range(0, 1)]
    public float hurtSoundVolume = 0.5f;
    public AudioClip blowSound;
    public AudioClip deadSoundSlowMotion;
    public AudioClip deadSound;
    [Range(0, 1)]
    public float deadSoundVolume = 0.5f;
    public AudioClip waterIN;
    public AudioClip waterOUT;
    public GameObject waterFX;
    public AudioClip SoundSlide;
    public AudioClip wallClimbSound;
    bool isPlayedLandSound;

    [Header("Boost")]
    public ParticleSystem BoostEffect;
    public AudioClip soundBoost;
    float boostYOffset = 1;
    float boostSpeed = 10;
    AudioSource boostASource;
    private bool isBoost = false;
    float targetBoostSpeed;
    public bool isBoostSpeed2 { get; set; }
    public bool needHoldForBoosting { get; set; }

    [Header("Option")]
    public bool bloodScreenFXHit = true;
     bool goThroughEnemy = false;
   [ReadOnly] public  bool allowHangingRopeShoot = false;      //allow player do this action
    [ReadOnly] public bool allowRopeType2Shoot = false;       //allow player do this action
    [ReadOnly] public bool allowWaterZoneShoot = true;
    [ReadOnly] public bool allowPipeShoot = true;
    bool allowHangingRopeGrenade = false;        //allow player do this action
    bool allowRopeType2Grenade = false;
    bool allowWaterZoneGrenade = false;
    bool allowPipeGrenade = true;

    public RangeAttack rangeAttack { get; set; }
    public MeleeAttack meleeAttack { get; set; }
    [ReadOnly] public PlayerCheckLadderZone playerCheckLadderZone;
    private AudioSource soundFx, wallClimbSoundSrc;

    float gravity;
    float originalGravity;
    [HideInInspector]
    public float maxJumpVelocity;
    float minJumpVelocity;
    [HideInInspector]
    public Vector3 velocity;
    float velocityXSmoothing;

    [HideInInspector]
    public bool isFacingRight
    {
        get
        {
            bool _faceRight = transform.localScale.x > 0;
            if (isHaningRope && transform.root.transform.localScale.x < 0)
                _faceRight = !_faceRight;

            return _faceRight;
        }
        set { }
    }
    [HideInInspector]
    public bool wallSliding;
    int wallDirX;

    bool allowClimpOnTopAgain;
    public bool isClimbOnTop { get; set; }

    [ReadOnly] public Vector2 input;

    [HideInInspector]
    public Controller2D controller;
    [HideInInspector]
    public Animator anim;

    public bool isPlaying { get; set; }
    public bool isInStayZone { get; set; }
    public bool isFinish { get; set; }
    public bool isInCannon { get; set; }
    public bool canJumpWhenHidingZone { get; set; }
    public bool canRunWhenHidingZone { get; set; }


    public GhostSprites ghostSprite { get; set; }

    public bool allowMoving { get; set; }
    public bool isFrozen { get; set; }  //player will be frozen

    [HideInInspector]
    public PushPullObject pushPullObj;
    public bool isDragging { get; set; }
    private GameObject currentDragObj;


    bool isBoostSpeed = false;
    bool forceShadowFX = false;

    public bool isInJumpPropArea { get; set; }
    public bool isJumpPropActive { get; set; }

    public bool isUsingPowerBombAction { get; set; }

    //check if Player is using: Shield or God or Fly or TimeStop or TimeSlow or Power or Partner
    public bool isUsingActions()
    {
        return (FindObjectOfType<Shield>() || GodMode || isUsingPowerBombAction) || GameManager.Instance.isInDialogue;
    }

    public bool isGrounded { get { return controller.collisions.below; } }

    void Awake()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        ghostSprite = GetComponent<GhostSprites>();
        Health = maxHealth;
        controller = GetComponent<Controller2D>();      //get the controller
        anim = GetComponent<Animator>();        //get the animator
        playerCheckLadderZone = GetComponent<PlayerCheckLadderZone>();
        SetupParameter();       //setup the parameters: speed, jump,...

        isPlaying = false;      //not allow the player woring at the beginning
        
        pushPullObj = GetComponentInChildren<PushPullObject>();
        allowMoving = true;

        walkSoundSrc = gameObject.AddComponent<AudioSource>();
        walkSoundSrc.clip = walkSound;
        walkSoundSrc.pitch = walkSoundSpeed;
        walkSoundSrc.Play();
        walkSoundSrc.loop = true;
        walkSoundSrc.volume = 0;

        runSoundSrc = gameObject.AddComponent<AudioSource>();
        runSoundSrc.clip = runSound;
        runSoundSrc.pitch = runSoundSpeed;
        runSoundSrc.Play();
        runSoundSrc.loop = true;
        runSoundSrc.volume = 0;

        jetpackEngineAudio = gameObject.AddComponent<AudioSource>();
        jetpackEngineAudio.clip = jetpackSoundEngine;
        jetpackEngineAudio.Play();
        jetpackEngineAudio.loop = true;
        jetpackEngineAudio.volume = 0;

        godAudioSource = gameObject.AddComponent<AudioSource>();
        godAudioSource.clip = godSoundKeep;
        godAudioSource.Play();
        godAudioSource.loop = true;
        godAudioSource.volume = 0;

        if (BoostEffect)
            BoostEffect.gameObject.SetActive(true);
        if (BoostEffect)
            SetEmissionRate(BoostEffect, 0);
        boostASource = gameObject.AddComponent<AudioSource>();
        boostASource.clip = soundBoost;
        boostASource.Play();
        boostASource.loop = true;
        boostASource.volume = 0;
    }

    void Start()
    {
        isFacingRight = transform.localScale.x > 0;     //check which direction the player are facing?

        numberOfJumpLeft = numberOfJumpMax;     //the number of jumping

        rangeAttack = GetComponent<RangeAttack>();      //get the RangeAttack and MeleeAttack scripts
        meleeAttack = GetComponent<MeleeAttack>();

        if (jetpackEngine)
            jetpackEngine.SetActive(false);     //unable the jetpack object

        soundFx = gameObject.AddComponent<AudioSource>();
        soundFx.loop = true;
        soundFx.playOnAwake = false;
        soundFx.clip = wallSlideSound;
        soundFx.volume = wallSlideSoundVolume;

        wallClimbSoundSrc = gameObject.AddComponent<AudioSource>();
        wallClimbSoundSrc.loop = true;
        wallClimbSoundSrc.playOnAwake = false;
        wallClimbSoundSrc.clip = wallClimbSound;
        wallClimbSoundSrc.volume = 0;
        wallClimbSoundSrc.Play();

        GameManager.Instance.lastJumpPos = transform.position;
        GameManager.Instance.AddListener(this);
        //if (GameManager.Instance.State == GameManager.GameState.Playing)
        //    ShowUpAnim();
    }

    public void InitGodmode(GodmodeType _type, float useTime, float damage)
    {
        if (GodMode)
            return;

        godmodeType = _type;
        godTimer = useTime;
        godmodeDamage = damage;

        if (mulSpeedc != 1)
        {
            StartCoroutine(SpeedBoostCo(true));
        }
        SlideOff();
        StartCoroutine(GodmodeCo());
    }

    Transform ropePoint;
    public Transform GrabRopePoint;
    public bool isGrabRopePoint { get; set; }
    float catchRopePointRate = 0.3f;
    float lastTimeGrabRopePoint;

    public void CatchRopePoint(Transform _ropePoint)
    {
        if (Time.time < lastTimeGrabRopePoint + catchRopePointRate)
            return;

        if (isUsingJetpack)
            return;

        ropePoint = _ropePoint;
        isGrabRopePoint = true;
    }

    public void SetupParameter()
    {
        switch (PlayerState)
        {
            case PlayerState.Ground:
                moveSpeed = GroundParameter.moveSpeed;
                maxJumpHeight = GroundParameter.maxJumpHeight;
                minJumpHeight = GroundParameter.minJumpHeight;
                timeToJumpApex = GroundParameter.timeToJumpApex;
                break;
            case PlayerState.Water:
                moveSpeed = WaterZoneParameter.moveSpeed;
                maxJumpHeight = WaterZoneParameter.maxJumpHeight;
                minJumpHeight = WaterZoneParameter.minJumpHeight;
                timeToJumpApex = WaterZoneParameter.timeToJumpApex;
                break;
            default:
                break;
        }

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        originalGravity = gravity;

        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        print("Gravity: " + gravity + "  Jump Velocity: " + maxJumpVelocity);

        if (PlayerState == PlayerState.Ground)
            velocity.y = minJumpVelocity;
        else
            velocity.y *= 0.1f;     //descrease the gravity

        if (!isPlaying)
            velocity.y = 0;
    }
    [HideInInspector]
    public float mulSpeedc = 1;

    float XXspeed, XXtime;
    public void SpeedBoost(float Xspeed, float time, bool allowEffect)
    {
        XXspeed = Xspeed;
        XXtime = time;
        StartCoroutine(SpeedBoostCo(allowEffect));
    }

    IEnumerator SpeedBoostCo(bool allowEffect)
    {
        mulSpeedc = XXspeed;
        isBoostSpeed = true;
        forceShadowFX = allowEffect;
        if (ghostSprite)
            ghostSprite.allowGhost = allowEffect;

        yield return new WaitForSeconds(XXtime);
        mulSpeedc = 1;
        isBoostSpeed = false;
    }

    bool isHoldJump = false;
    public bool forceGhostFX { get; set; }

    void Update()
    {
        if (isFrozen)
            return;

        if (isPlaying)
            imageCharacterSprite.enabled = (!isInCannon);

        if (BoostEffect)
            SetEmissionRate(BoostEffect, isBoostSpeed2 && isPlaying ? 200 : 0);

        if (smokeParticleFX)
            SetEmissionRate(smokeParticleFX, isSliding && isPlaying ? 20 : 0);

        if (playerCheckLadderZone.fallingOffFromLadder)
        {
            var hits = Physics2D.BoxCastAll(transform.position, controller.boxcollider.size, 0, Vector2.zero, 0, controller.collisionMask);
            if (hits.Length == 0)
            {
                playerCheckLadderZone.fallingOffFromLadder = false;
                controller.HandlePhysic = true;
            }
        }
        HandleAnimation();      //set the animation state for the Player

        if (gameObject.layer == LayerMask.NameToLayer("HidingZone"))
        {
            controller.collisionMask = controller.collisionMask & ~(1 << 10);
            controller.collisionMaskHorizontal = controller.collisionMaskHorizontal & ~(1 << LayerMask.NameToLayer("Enemy"));
            if (gameObject.layer == LayerMask.NameToLayer("HidingZone"))
            {
                imageCharacterSprite.sortingLayerName = "HidingZone";
                imageCharacterSprite.sortingOrder = 1;
            }
        }
        else
        {
            imageCharacterSprite.sortingLayerName = "Player";
            imageCharacterSprite.sortingOrder = 1;
            if (GodMode)
            {
                controller.collisionMaskHorizontal = controller.collisionMaskHorizontal & ~(1 << LayerMask.NameToLayer("Enemy"));
                controller.collisionMask = controller.collisionMask | (1 << LayerMask.NameToLayer("Enemy"));
                gameObject.layer = LayerMask.NameToLayer("Player");
            }
            else
            {
                if ((goThroughEnemy || isBoostSpeed2) || isBlinking || isDogging)
                {
                    if (isDogging && doggeType == DoggeType.HitObject)
                    {
                        ;
                    }
                    else
                    {
                        if (isBlinking)
                        {
                            controller.collisionMaskHorizontal = controller.collisionMaskHorizontal & ~(1 << LayerMask.NameToLayer("Enemy"));
                            if (controller.collisions.below)
                                controller.collisionMask = controller.collisionMask & ~(1 << LayerMask.NameToLayer("Enemy"));
                            else
                                controller.collisionMask = controller.collisionMask | (1 << LayerMask.NameToLayer("Enemy"));
                        }
                        else
                        {
                            controller.collisionMask = controller.collisionMask & ~(1 << LayerMask.NameToLayer("Enemy"));
                            controller.collisionMaskHorizontal = controller.collisionMaskHorizontal & ~(1 << LayerMask.NameToLayer("Enemy"));
                        }

                        if ((goThroughEnemy && isBoostSpeed2) || isBlinking || isDogging)
                            gameObject.layer = LayerMask.NameToLayer("IgnoreAll");
                        else
                            gameObject.layer = LayerMask.NameToLayer("Player");
                    }
                }
                else
                {
                    controller.collisionMask = controller.collisionMask | (1 << 10);
                    controller.collisionMaskHorizontal = controller.collisionMask;
                    gameObject.layer = LayerMask.NameToLayer("Player");
                }
            }
        }

        wallDirX = controller.collisions.faceDir;       //get the current direction

        float targetVelocityX = input.x * (isRunning ? runningSpeed : moveSpeed) * mulSpeedc;     //get the current moving state, Walk or Run?

        accGrounedOverride = 0;
        SurfaceModifier isUseSurfaceMod = null;
        bool useOverrideAcc = false;
        //Check ICE platform
        if (controller.collisions.ClosestHit)
        {
            isUseSurfaceMod = controller.collisions.ClosestHit.collider.gameObject.GetComponent<SurfaceModifier>();
            if (isUseSurfaceMod && isUseSurfaceMod.Friction > 0 && isUseSurfaceMod.Friction < 1)
            {
                accGrounedOverride = 1f / isUseSurfaceMod.Friction;
                useOverrideAcc = true;
            }
        }

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing,
            (controller.collisions.below) ? (useOverrideAcc ? accGrounedOverride : accelerationTimeGrounded) : accelerationTimeAirborne);       //make the smooth movement

        if (controller.collisions.ClosestHit && controller.collisions.below)
        {
            isUseSurfaceMod = controller.collisions.ClosestHit.collider.gameObject.GetComponent<SurfaceModifier>();
            if (isUseSurfaceMod && isUseSurfaceMod.Friction > 1)
            {
                velocity.x /= isUseSurfaceMod.Friction;
            }
        }

        if (isSliding)
            velocity.x = isFacingRight ? runningSpeed : -runningSpeed;

        if (isPlaying)
            CheckWall();



        if (isRunning && (controller.collisions.left || controller.collisions.right))
        {
            StopRunning();
        }


        if (playerCheckLadderZone.isClimbing)
        {
            velocity.x = 0;

            if (velocity.y > 0 && !playerCheckLadderZone.checkLadderWithPoint(controller.boxcollider.bounds.center))
            {
                playerCheckLadderZone.isClimbing = false;
                velocity.y = minJumpVelocity;

            }
        }

        if (!playerCheckLadderZone.isClimbing)
        {
            velocity.y += gravity * Time.deltaTime;

            if (wallSliding)
            {
                if (velocity.y < -wallSlideSpeedMax)
                {

                    velocity.y = -wallSlideSpeedMax;
                }

                if (controller.collisions.ClosestHit != null && controller.collisions.ClosestHit.collider.CompareTag("GrabWall"))
                {
                    velocity.y = 0;
                }
            }

            if (isClimbOnTop)
                velocity.y = 0;
        }

        if (controller.collisions.below && !isPlayedLandSound)
        {        //check to play land sound
            isPlayedLandSound = true;
            timeToWallUnstick = 0;
            playerCheckLadderZone.isClimbing = false;
            isJumpPropActive = false;
            SoundManager.PlaySfx(landSound, landSoundVolume);
        }
        else if (!controller.collisions.below && isPlayedLandSound)
        {
            if(velocity.y > 0)
            isPlayedLandSound = false;
        }

        if (isBoostSpeed || (isDogging && useDoggeGhostFX))
        {
            ghostSprite.allowGhost = forceShadowFX && !isInCannon;
        }
        else
        {
            ghostSprite.allowGhost = (isRunning) || forceGhostFX;
        }

wallClimbSoundSrc.volume = 0;

        walkSoundSrc.volume = GlobalValue.isSound ? ((Mathf.Abs(velocity.x) > 0.1f && !isRunning && controller.collisions.below) ? walkSoundVolume : 0) : 0;
        runSoundSrc.volume = GlobalValue.isSound ? ((Mathf.Abs(velocity.x) > 0.1f && isRunning && controller.collisions.below) ? runSoundVolume : 0) : 0;
    }


    void StopClimbLadderCo()
    {
        velocity.y = 0f;
    }

    public void Boost(float distance, float speedBoost, float yOffet, float delayHoldForBoost)
    {
        needHoldForBoosting = false;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Ground"), true);
        isBoostSpeed2 = true;
        boostYOffset = yOffet;
        boostSpeed = speedBoost;
        targetBoostSpeed = transform.position.x + distance * (isFacingRight ? 1 : -1);
        anim.SetBool("isBoostRotate", true);
        boostASource.volume = 1;

        Invoke("MustHoldToKeepBoosting", delayHoldForBoost);
    }

    public void StopBoost2()
    {
        velocity = Vector2.zero;
        isBoostSpeed2 = false;
        boostASource.volume = 0;
        anim.SetBool("isBoostRotate", false);

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Ground"), false);
    }

    void MustHoldToKeepBoosting()
    {
        needHoldForBoosting = true;
    }

    public void SetEmissionRate(ParticleSystem particleSystem, float emissionRate)
    {
        var emission = particleSystem.emission;
        var rate = emission.rate;
        rate.constantMax = emissionRate;
        emission.rate = rate;
    }

    bool allowGrabNextWall = true;

    bool allowCheckWall = true;

    void AllowCheckWall()
    {
        allowCheckWall = true;
    }

    void CheckWall()
    {
        wallSliding = false;

        if (!allowCheckWall)
            return;

        if (PlayerState == PlayerState.Water)
            return;

        if (!Physics2D.Raycast(controller.boxcollider.bounds.center + Vector3.up * controller.boxcollider.bounds.size.y * 0.3f, isFacingRight ? Vector2.right : Vector2.left, controller.boxcollider.bounds.size.x * 0.6f, controller.collisionMask))
            return;

        if (timeToWallUnstick > 0 || (allowSlideWall && ((controller.collisions.left && (wallDirX == -1 || isInJumpWallZone))
            || (controller.collisions.right && (wallDirX == 1 || isInJumpWallZone))) && !controller.collisions.below && velocity.y < 0) && (input.x == wallDirX || (isInJumpWallZone && allowGrabNextWall)))
        {
            if (controller.collisions.ClosestHit.collider != null && controller.collisions.ClosestHit.collider.gameObject.layer == LayerMask.NameToLayer("Ground") && !controller.collisions.ClosestHit.collider.gameObject.CompareTag("Through"))
            {
                wallSliding = true;

                if (allowGrabNextWall)
                {
                    timeToWallUnstick = wallStickTime * 2;
                    allowGrabNextWall = false;
                }
                if (!soundFx.isPlaying)
                    soundFx.Play();     //play the sliding sound

                if (timeToWallUnstick > 0)
                {
                    velocityXSmoothing = 0;
                    velocity.x = 0;

                    if (input.x != wallDirX)
                    {
                        timeToWallUnstick -= Time.deltaTime;
                    }
                    else
                    {
                        timeToWallUnstick = wallStickTime;
                    }
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
        }
        else
        {
            if (soundFx.isPlaying)
                soundFx.Stop();
        }

        if ((!controller.collisions.left && controller.collisions.faceDir == -1) || (!controller.collisions.right && controller.collisions.faceDir == 1))
        {
            wallSliding = false;
            allowCheckWall = true;
        }
    }

    public bool isRoping { get; set; }
    public bool isHaningRope { get; set; }

    [HideInInspector]
    public bool inverseGravity = false;

    void LateUpdate()
    {
        if (isFrozen)
            return;

        if (GameManager.Instance.State == GameManager.GameState.GameOver)
            return;

        if (isBoostSpeed2)
        {
            transform.Translate(boostSpeed * Time.deltaTime * (isFacingRight ? 1 : -1), boostYOffset * Time.deltaTime, 0, Space.Self);

            RaycastHit2D hit = (Physics2D.Raycast(transform.position + Vector3.up * 0.5f, isFacingRight ? Vector2.right : Vector2.left, 0.8f, doggeHitLayer));
            RaycastHit2D hitUp = (Physics2D.Raycast(transform.position + Vector3.up * 0.5f, Vector2.up, 0.8f, doggeHitLayer));
            if (isBoostSpeed2 && ((isFacingRight && transform.position.x >= targetBoostSpeed
                || (!isFacingRight && transform.position.x <= targetBoostSpeed)
                || (needHoldForBoosting && !isHoldJump)
                || hit || hitUp)))
            {
                if (hit)
                {
                    transform.position = hit.point + Vector2.one * (isFacingRight ? -0.6f : 0.6f);
                }

                if (hitUp)
                {
                    transform.position = hitUp.point + Vector2.up * -0.8f;
                }

                StopBoost2();
            }
            return;
        }

        CheckClimbOnTop();
        //CheckLedge();

        if (!isPlaying || GameManager.Instance.State != GameManager.GameState.Playing)
            velocity.x = 0;     //stop right here if the game is not in playing mode

        if (isInCannon)
        {
            velocity = Vector2.zero;
        }

        if (isUsingJetpack && jetpackEnegry > 0)
        {      //check if the player is using the jetpack and the jetpack enegry > 0
            velocity.y = jetpackForce;      //push the player with the jetpack force
            jetpackEnegry -= jetpackUseSpeed * Time.deltaTime;      //reduce the jetpack enegry overtime
            jetpackEngineAudio.volume = GlobalValue.isSound ? 0.85f : 0;
            if (jetpackEnegry <= 0)
                StopUseJetpack();       //if the enegry reach to zero then stop the jetpack
        }
        else if (isJetPackAvailable && allowRechargeJetpack)
        {
            jetpackEnegry += jetpackRechargeSpeed * Time.deltaTime;     //only recharge the jetpack again if allowed
        }
        jetpackEnegry = Mathf.Clamp(jetpackEnegry, 0, 100);

        //RUNNING
        if (isRunning && runningEnegry > 0)
        {
            runningEnegry -= runningUseSpeed * Time.deltaTime;
            if (runningEnegry <= 0 || Mathf.Abs(velocity.x) < 0.1)
                StopRunning();
        }
        else if (allowRechargeRunning)
        {
            runningEnegry += runningRechargeSpeed * Time.deltaTime;
        }
        runningEnegry = Mathf.Clamp(runningEnegry, 0, 100);     //limited the running enegry by 100

        if (!isDogging)
        {
            doggeStatus += Time.deltaTime / doggeEnegryRechargeTime;
        }

        doggeStatus = Mathf.Clamp(doggeStatus, 0, 1);

        if (isRoping || isHaningRope || isGrabRopePoint)
            velocity = Vector2.zero;

        if (!allowMoving || wallSliding)
            velocity.x = 0;

        if (isDogging)
            velocity.y = 0;

        if ((controller.raycastOrigins.bottomLeft.x < CameraFollow.Instance._min.x && velocity.x < 0) || (controller.raycastOrigins.bottomRight.x > CameraFollow.Instance._max.x && velocity.x > 0))
            velocity.x = 0;

        if (controller.raycastOrigins.topRight.y > CameraFollow.Instance._max.y && velocity.y > 0)
            velocity.y = 0;

        if (controller.raycastOrigins.topRight.y < CameraFollow.Instance._min.y && velocity.y < 0)
            LevelManager.Instance.KillPlayer();

        Vector3 finalVelocity = velocity;
        Vector3 finalInput = input;

        if (inverseGravity)
        {
            finalVelocity.y *= -1;
            finalInput.y *= -1;
        }

        if (isDragging)
        {
            if ((currentDragObj.GetComponent<BoxSetup>().BoxHitRight() && input.x == 1) || (currentDragObj.GetComponent<BoxSetup>().BoxHitLeft() && input.x == -1))
            {
                velocity.x = 0;
                finalVelocity.x = 0;
            }

            if ((isFacingRight && input.x == 1) || (!isFacingRight && input.x == -1))
                ;
            else
            {
                RaycastHit2D checkHitWall = Physics2D.Raycast(transform.position, (isFacingRight ? Vector2.left : Vector2.right), 0.7f, controller.collisionMask);
                if (checkHitWall)
                {
                    velocity.x = 0;
                    finalVelocity.x = 0;
                }
            }


            currentDragObj.GetComponent<BoxSetup>().MoveBox(finalVelocity * 0.5f, finalInput);
        }

        controller.Move(finalVelocity * Time.deltaTime * (isDragging ? 0.5f : 1), finalInput, false, true);      //move the player

        if (isGrabRopePoint)
        {
            transform.position = ropePoint.position - (GrabRopePoint.position - transform.position);
        }

        //if (isGrabbingLedge)
        //{
        //    transform.position += (Vector3)checkLedge.offset;
        //}

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;     //stop the gravity when the player colide with the Top/Bottom 
        }

        if (controller.collisions.above)
        {
            CheckBox();     //check if the brick on the head
        }

        if (controller.collisions.below && velocity.y <= 0)
        {
            allowClimpOnTopAgain = true;     //allow the player h		old the pipe or not depend on the allowClimbOnTop value
            allowRechargeJetpack = true;        //allow recharge the jetpack's enegry

            if (isPlaying)
            {
                CheckSpring();      //check the Spring on the feet 
                CheckJumpOnEnemyHead();
                CheckBridge();
            }
        }

        if (wallSliding && (controller.collisions.left || controller.collisions.right))
            CheckBridge();

        if (isDogging)
        {
            //if (doggePowerBar <= 0)
            //{
            //    StopDogge();
            //    return;
            //}

            velocity.x = speedDogge * doggeDirection.x;
            velocity.y = speedDogge * doggeDirection.y;
        }
    }

    private void CheckBridge()
    {
        if (controller.collisions.ClosestHit.collider == null)
            return;
        var bridge = controller.collisions.ClosestHit.collider.gameObject.GetComponent<Bridge>();
        if (bridge)
        {
            bridge.Work();
        }
    }
    private void CheckJumpOnEnemyHead()
    {
        if (isDogging || !isPlaying)
            return;

        if (meleeAttack.DetectEnemies.activeInHierarchy)
            return;

        if (gameObject.layer == LayerMask.NameToLayer("HidingZone"))
            return;

        if (controller.collisions.ClosestHit != null && controller.collisions.ClosestHit.collider != null)
        {
            if (isBlinking && controller.collisions.ClosestHit.collider.gameObject.GetComponent<GiveDamageToPlayerX>())
            {
                var pushForceX = controller.collisions.ClosestHit.collider.gameObject.GetComponent<GiveDamageToPlayerX>().pushPlayer;
                pushForceX.x = Mathf.Abs(pushForceX.x) * (isFacingRight ? 1 : -1);
                pushForceX.y = Mathf.Abs(pushForceX.y);

                velocity = pushForceX;
            }

            if (GodMode && controller.collisions.ClosestHit.collider.gameObject.GetComponent<GiveDamageToPlayerX>())
            {
                var canTakeDamage = (ICanTakeDamage)controller.collisions.ClosestHit.collider.gameObject.GetComponent<GiveDamageToPlayerX>().gameObject.GetComponent(typeof(ICanTakeDamage));
                if (canTakeDamage != null)
                    canTakeDamage.TakeDamage(float.MaxValue, Vector2.zero, gameObject, transform.position);
            }

            if (controller.collisions.ClosestHit != null && controller.collisions.ClosestHit.collider.gameObject.GetComponent<CanBeJumpOn>() == null)
                return;

            if (feetPosition.transform.position.y < controller.collisions.ClosestHit.transform.position.y)
                return;

            var pushForce = controller.collisions.ClosestHit.collider.gameObject.GetComponent<CanBeJumpOn>().pushForce;
            pushForce.x = Mathf.Abs(pushForce.x) * (isFacingRight ? 1 : -1);
            pushForce.y = Mathf.Abs(pushForce.y);

            velocity = pushForce;
            SoundManager.PlaySfx(jumpSound, jumpSoundVolume);

            var damage = (ICanTakeDamage)controller.collisions.ClosestHit.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
            if (damage != null)
                damage.TakeDamage(makeDamageOnEnemyHead, Vector2.zero, gameObject, controller.collisions.ClosestHit.collider.transform.position);     //kill the enemy right away while in godmode
        }
    }

    //check the brick when the player jump, if there are the brick then break it
    private void CheckBox()
    {
        CheckBlockBrick();

        if (controller.collisions.ClosestHit.collider.gameObject.GetComponent<CanBeHitByPlayerHead>())
        {
            var takeDamage = (ICanTakeDamage)controller.collisions.ClosestHit.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
            if (takeDamage != null)
            {
                takeDamage.TakeDamage(int.MaxValue, Vector2.zero, gameObject, transform.position);
            }
        }
    }

    void CheckBlockBrick()
    {
        Block isBlock;
        var bound = controller.boxcollider.bounds;

        //check middle
        var hit = Physics2D.Raycast(new Vector2((bound.min.x + bound.max.x) / 2f, bound.max.y), Vector2.up, 0.5f, 1 << LayerMask.NameToLayer("Platform"));

        if (hit)
        {
            isBlock = hit.collider.gameObject.GetComponent<Block>();
            if (isBlock)
            {
                isBlock.BoxHit();
            }
        }

        //check left
        hit = Physics2D.Raycast(new Vector2(bound.min.x, bound.max.y), Vector2.up, 0.5f, 1 << LayerMask.NameToLayer("Platform"));
        if (hit)
        {
            isBlock = hit.collider.gameObject.GetComponent<Block>();
            if (isBlock)
            {
                isBlock.BoxHit();
            }
        }

        hit = Physics2D.Raycast(new Vector2(bound.max.x, bound.max.y), Vector2.up, 0.5f, 1 << LayerMask.NameToLayer("Platform"));
        if (hit)
        {
            isBlock = hit.collider.gameObject.GetComponent<Block>();
            if (isBlock)
            {
                isBlock.BoxHit();
            }
        }
    }

    //check if the top of player collide with the Pipe, if yes then allow it hold the pipe and moving
    private void CheckClimbOnTop()
    {
        if (isUsingJetpack)
            return;

        if (isClimbOnTop)
        {
            var hit = Physics2D.Raycast(transform.position, Vector2.up, 2, controller.collisionMask);
            if (hit)
                isClimbOnTop = hit.collider.CompareTag("ClimbOnTop");
            else
                isClimbOnTop = false;
        }

        if (controller.collisions.above && allowClimpOnTopAgain)
        {
            var hit = Physics2D.Raycast(transform.position, Vector2.up, 2, controller.collisionMask);
            if (hit)
            {

                if (hit.collider.CompareTag("ClimbOnTop"))
                {
                    isClimbOnTop = true;
                    allowClimpOnTopAgain = false;
                }
            }
        }
    }

    //check if the player stand on the Spring, if yes then push the player up with the given value height from the Spring script
    private void CheckSpring()
    {
        if (controller.collisions.ClosestHit)
        {
            var spring = controller.collisions.ClosestHit.collider.GetComponent<Spring>();
            if (spring)
            {
                velocity.y = Mathf.Abs((2 * spring.pushHeight) / Mathf.Pow(timeToJumpApex, 2)) * timeToJumpApex;
                spring.Push();
            }
        }
    }

    //Flip the play to facing Right or Left base on its direction and the X velocity
    public void Flip()
    {
        if (wallSliding)
            return;

        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void DragBegin()
    {
        isDragging = true;
        Debug.Log("CC");
        //move player back a little bit
        if (pushPullObj.hit)
            currentDragObj = pushPullObj.hit.transform.gameObject;
    }

    public void DragStop()
    {
        isDragging = false;
        if (currentDragObj)
            currentDragObj.transform.parent = null;
    }

    //This action is called by the Input/ControllerInput
    public void MoveLeft()
    {
        //if (isGrabbingLedge)
        //    return;

        if (isPlaying)
        {
            if ((playerCheckLadderZone.isClimbing && playerCheckLadderZone.currentLadder && !playerCheckLadderZone.currentLadder.GetComponent<Ladder8DirsZone>()) || playerCheckLadderZone.fallingOffFromLadder)
            {
                var hits = Physics2D.BoxCastAll(transform.position, controller.boxcollider.size, 0, Vector2.zero, 0, controller.collisionMask);
                if (hits.Length > 0)
                    return;
            }
            else if (playerCheckLadderZone.isClimbing && playerCheckLadderZone.currentLadder && playerCheckLadderZone.currentLadder.GetComponent<Ladder8DirsZone>())
            {
                velocity.x = -playerCheckLadderZone.climbSpeed;
            }

            input = new Vector2(-1, 0);

            //don't allow flip character if dragging
            if (isDragging)
                return;

            if (isHaningRope)
            {
                if (transform.root.transform.localScale.x < 0 && transform.localScale.x < 0)
                    Flip();
                else if (transform.root.transform.localScale.x > 0 && isFacingRight)
                    Flip();
            }

            else
            if (isFacingRight)
            {
                Flip();
            }
        }
    }

    //This action is called by the Input/ControllerInput
    public void MoveRight()
    {
        //if (isGrabbingLedge)
        //    return;

        if (isPlaying)
        {
            if ((playerCheckLadderZone.isClimbing && playerCheckLadderZone.currentLadder && !playerCheckLadderZone.currentLadder.GetComponent<Ladder8DirsZone>()) || playerCheckLadderZone.fallingOffFromLadder)
            {
                var hits = Physics2D.BoxCastAll(transform.position, controller.boxcollider.size, 0, Vector2.zero, 0, controller.collisionMask);
                if (hits.Length > 0)
                    return;
            }
            else if (playerCheckLadderZone.isClimbing && playerCheckLadderZone.currentLadder && playerCheckLadderZone.currentLadder.GetComponent<Ladder8DirsZone>())
            {
                velocity.x = playerCheckLadderZone.climbSpeed;
            }
            input = new Vector2(1, 0);

            //don't allow flip character if dragging
            if (isDragging)
                return;

            if (isHaningRope)
            {
                if (transform.root.transform.localScale.x < 0 && transform.localScale.x > 0)
                    Flip();
                else if (transform.root.transform.localScale.x > 0 && !isFacingRight)
                    Flip();
            }

            else if (!isFacingRight)
            {
                Flip();
            }
        }
    }

    //This action is called by the Input/ControllerInput
    public void MoveUp()
    {
        if (isPlaying && playerCheckLadderZone.isInLadderZone)
        {
            if (isGrounded && !playerCheckLadderZone.isLadderAbove)
                return;

            transform.position = new Vector2(playerCheckLadderZone.currentLadder.transform.position.x, transform.position.y);
            playerCheckLadderZone.isClimbing = true;

            velocity.y = playerCheckLadderZone.climbSpeed;
        }

        isClimbOnTop = false;

        input = new Vector2(0, 1);
    }

    //This action is called by the Input/ControllerInput
    public void MoveDown()
    {
        //if (GameManager.Instance.Player.isGrabbingLedge)
        //{
        //    isGrabbingLedge = false;
        //    ledgeJumpTime = Time.time;
        //    return;
        //}

        if (isPlaying && playerCheckLadderZone.isInLadderZone)
        {
            if (isGrounded && !playerCheckLadderZone.isLadderBelow)
            {
                input = new Vector2(0, -1);
                return;
            }

            transform.position = new Vector2(playerCheckLadderZone.currentLadder.transform.position.x, transform.position.y);
            playerCheckLadderZone.isClimbing = true;
            velocity.y = -playerCheckLadderZone.climbSpeed;
        }

        isClimbOnTop = false;

        input = new Vector2(0, -1);
    }

    //This action is called by the Input/ControllerInput
    public void StopLadder()
    {
        if (!isPlaying)
            return;

        if (!isGrounded)
        {
            velocity.y = 0;
            velocity.x = 0;
        }
    }

    //This action is called by the Input/ControllerInput
    public void StopMove()
    {
        input = Vector2.zero;

        if (!isPlaying)
            return;

        if (playerCheckLadderZone.isClimbing)
        {
            velocity.y = 0;
            velocity.x = 0;
        }

        if (isRunning)
        {
            StopRunning();
        }
    }

    public void Run()
    {
        if (isDragging)
            return;

        if (!isRunning && PlayerState == PlayerState.Ground && controller.collisions.below && runningEnegry >= 10)
        {
            allowRechargeRunning = false;
            isRunning = true;
        }
    }

    public void Forzen(bool is_enable)
    {
        isFrozen = is_enable;
        anim.enabled = !is_enable;
    }

    public void StopRunning()
    {
        if (isRunning)
            StartCoroutine(RunningBreak());
    }

    //allow recharge the running enegry after 1s
    IEnumerator RunningBreak()
    {
        isRunning = false;
        yield return new WaitForSeconds(1);
        allowRechargeRunning = true;
    }

    Vector2 oriSize;
    Vector2 oriOffet;
    public bool isSlidingInTurnel { get; set; }     //mean have a platform on player's head when he sliding
    //This action is called by the Input/ControllerInput, slide the player with the parameters can be set in the Hierarchy

    public void Slide()
    {
        if (isRunning)
        {
            if (!isSliding)
                StartCoroutine(SlideCo());
        }
    }

    private void SlideOff()
    {
        if (!isSliding)
            return;

        isSlidingInTurnel = false;
        isSliding = false;
        controller.boxcollider.size = oriSize;
        controller.boxcollider.offset = oriOffet;
    }

    IEnumerator SlideCo()
    {
        isSliding = true;
        SoundManager.PlaySfx(SoundSlide);
        oriSize = controller.boxcollider.size;
        oriOffet = controller.boxcollider.offset;
        controller.boxcollider.size = new Vector2(controller.boxcollider.size.x, controller.boxcollider.size.y * 0.5f);
        controller.boxcollider.offset = new Vector2(controller.boxcollider.offset.x, -controller.boxcollider.size.y * 0.5f);
        yield return new WaitForSeconds(slidingTime);

        while (HitWallOnHeadWhenSlide())
        {
            isSlidingInTurnel = true;
            yield return null;
        }

        SlideOff();
    }

    bool HitWallOnHeadWhenSlide()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 1, controller.collisionMask);
        return hit ? true : false;
    }

    //This action is called by the Input/ControllerInput
    public void UseJetpack()
    {
        if (!isPlaying || jetpackEnegry <= 0 || playerCheckLadderZone.isInLadderZone || playerCheckLadderZone.fallingOffFromLadder)
            return;

        isUsingJetpack = true;
        allowRechargeJetpack = false;
        if (jetpackEngine)
            jetpackEngine.SetActive(true && useJetpackFireFX);

        isClimbOnTop = false;
    }

    //This action is called by the Input/ControllerInput
    public void StopUseJetpack()
    {
        if (!isPlaying)
            return;

        isUsingJetpack = false;
        if (jetpackEngine)
            jetpackEngine.SetActive(false);

        jetpackEngineAudio.volume = 0;
    }

    public JumpProp currentJumpProp { get; set; }
    public void Jump()
    {
        //Debug.LogError(input);
        isHoldJump = true;
        allowGrabNextWall = true;

        Debug.Log("Jump");
        if (isInStayZone)
        {
            Flip();
            input.x *= -1;      //change the direction
            isPlaying = true;
            isInStayZone = false;
            return;
        }

        if (!isPlaying)
            return;

        if (isSlidingInTurnel)
            return;

        if (isJumpPropActive)
            return;

        if (isInJumpPropArea)
        {
            isJumpPropActive = true;
            velocity.y = minJumpVelocity;
            currentJumpProp.MakeJump();
            return;
        }

        if (isBoostSpeed2)
        {
            return;
        }

        if (isRoping || isHaningRope)
        {
            ExitTheRope();
            RopeUI.instance.ExitRope();
        }

        if (isGrabRopePoint)
        {
            isGrabRopePoint = false;
            numberOfJumpLeft = numberOfJumpMax;
            lastTimeGrabRopePoint = Time.time;
        }

        //do not allow the player jump when the player are climbing the ladder and are in the the ground
        if (playerCheckLadderZone.isClimbing)
        {
            //check if can jump off from Ladder
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, 1.25f, Vector2.zero, 0, 1 << LayerMask.NameToLayer("Ground"));

            if (!hit && GameManager.Instance.controllerInput.isHoldingRight)
            {
                playerCheckLadderZone.fallingOffFromLadder = true;
                playerCheckLadderZone.isInLadderZone = false;
                playerCheckLadderZone.currentLadder = null;
                playerCheckLadderZone.isClimbing = false;
                input.x = 1;

                if (!isFacingRight)
                    Flip();

                velocity.y = maxJumpVelocity;
                velocity.x = moveSpeed;
                SoundManager.PlaySfx(jumpSound, jumpSoundVolume);
                numberOfJumpLeft = numberOfJumpMax;
                GameManager.Instance.lastJumpPos = transform.position;
            }
            else if (!hit && GameManager.Instance.controllerInput.isHoldingLeft)
            {
                playerCheckLadderZone.fallingOffFromLadder = true;
                playerCheckLadderZone.isInLadderZone = false;
                playerCheckLadderZone.currentLadder = null;
                playerCheckLadderZone.isClimbing = false;
                input.x = -1;

                if (isFacingRight)
                    Flip();

                velocity.y = maxJumpVelocity;
                velocity.x = -moveSpeed;
                SoundManager.PlaySfx(jumpSound, jumpSoundVolume);
                numberOfJumpLeft = numberOfJumpMax;
                GameManager.Instance.lastJumpPos = transform.position;
            }
            else
            {
                JumpOffFromLadder();
                //			var hits = Physics2D.BoxCastAll(transform.position,controller.boxcollider.size,0,Vector2.zero,0,controller.collisionMask);
                //			if(hits.Length>0)
                return;
            }
        }
        if (wallSliding)
        {
            if (wallDirX == input.x && !allowJumpClimbSlideWall && !isInJumpWallZone)
                return;

            allowCheckWall = false;
            Invoke("AllowCheckWall", 0.1f);

            timeToWallUnstick = 0;
            SoundManager.PlaySfx(jumpSound, jumpSoundVolume);

            if (isInJumpWallZone && input.x == 0)
            {
                velocity.x = -wallDirX * wallLeap.x * 1.5f;
                velocity.y = wallLeap.y;
                Flip();


            }
            else if (wallDirX == input.x)
            {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            else if (input.x == 0)
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
                Flip();
            }
            else
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
                Flip();
            }
        }
        else if (controller.collisions.below && PlayerState == PlayerState.Ground)
        {
            velocity.y = maxJumpVelocity;

            if (JumpEffect != null)
                Instantiate(JumpEffect, transform.position, transform.rotation);
            SoundManager.PlaySfx(jumpSound, jumpSoundVolume);
            numberOfJumpLeft = numberOfJumpMax;
            GameManager.Instance.lastJumpPos = transform.position;
        }
        else if (PlayerState == PlayerState.Water)
        {
            velocity.y = minJumpVelocity;
            SoundManager.PlaySfx(waterIN, 0.25f);
        }
        else
        {
            numberOfJumpLeft--;
            if (numberOfJumpLeft > 0)
            {
                velocity.y = minJumpVelocity;

                anim.SetTrigger("doubleJump");

                if (JumpEffect != null)
                    Instantiate(JumpEffect, transform.position, transform.rotation);
                SoundManager.PlaySfx(jumpSound, jumpSoundVolume);
            }
        }
        isClimbOnTop = false;
    }

    //This action is called by the Input/ControllerInput
    public void JumpOff()
    {
        isHoldJump = false;

        if (!isPlaying || isJumpPropActive)
            return;
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    //This action is called by the Input/ControllerInput
    public void MeleeAttack()
    {
        if (!isPlaying || isClimbOnTop || isRoping || isHaningRope || isGrabRopePoint || isSlidingInTurnel)
            return;

        if (meleeAttack != null)
        {
            if (meleeAttack.CanAttack())
            {
                if (wallSliding)
                {
                    anim.SetTrigger("wallAttack");
                }
                else
                {
                    anim.SetTrigger("meleeAttack");
                }
            }
            //if (controller.collisions.below || PlayerState == PlayerState.Water)
            //    if (meleeAttack.CanAttack())
            //    {
            //        if (wallSliding)
            //        {
            //            anim.SetTrigger("wallAttack");
            //        }
            //        else
            //        {
            //            anim.SetTrigger("meleeAttack");
            //        }
            //    }
        }
    }

    //This action is called by the Input/ControllerInput
    //buttonUp means allow fire
    public void RangeAttack(bool powerBullet)
    {
        if (GameManager.Instance.Player.isInCannon)
            return;
        if (GameManager.Instance.Player.playerCheckLadderZone.isClimbing)
            return;
        if (GameManager.Instance.Player.isSliding)
            return;
        if (wallSliding)
            return;
        if (PlayerState == PlayerState.Water && !allowWaterZoneShoot)
            return;
        if ((isHaningRope) && !allowHangingRopeShoot)
            return;
        if ((isGrabRopePoint) && !allowRopeType2Shoot)
            return;
        if (isClimbOnTop && !allowPipeShoot)
            return;
        //if (isGrabbingLedge)
        //    return;
        if (!isPlaying)
            return;

        if (rangeAttack != null)
        {
            if (rangeAttack.Fire(false))
            {
                SoundManager.PlaySfx(rangeAttack.shootSound);
            }
        }
    }

    public void CancelRangeHolding()
    {
        if (rangeAttack)
            rangeAttack.CancleHolding();
    }

    //This action is called by the Input/ControllerInput
    public void ThrowGrenade()
    {
        if (!isPlaying || Time.time < lastTimeThrowGrenade + grenadeRate)
            return;
        if (_Grenade == null || GlobalValue.grenade <= 0)
            return;
        if (wallSliding)
            return;
        if (PlayerState == PlayerState.Water && !allowWaterZoneGrenade)
            return;
        if ((isHaningRope) && !allowHangingRopeGrenade)
            return;
        if ((isGrabRopePoint) && !allowRopeType2Grenade)
            return;
        if (isClimbOnTop && !allowPipeGrenade)
            return;
    

        GlobalValue.grenade--;
        lastTimeThrowGrenade = Time.time;

        Vector3 throwPos = throwPosition.position;
        if (wallSliding)
        {
            float throwX = transform.position.x;
            float offsetThrow = throwPosition.localPosition.x;

            throwX += transform.position.x > throwPosition.position.x ? offsetThrow : -offsetThrow;
            throwPos.x = throwX;
        }

        GameObject obj = Instantiate(_Grenade, throwPos, Quaternion.identity) as GameObject;

        float angle;
        angle = transform.localScale.x > 0 ? angleThrow : 180 - angleThrow;

        if (inverseGravity)
            angle *= -1;

        if (wallSliding)
            angle = 180 - angle;

        Debug.Log("Wallsliding" + wallSliding);

        obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        Debug.Log(obj.transform.eulerAngles.z);
        if (inverseGravity)
            obj.GetComponent<Rigidbody2D>().gravityScale = -1;
        obj.GetComponent<Rigidbody2D>().AddForce(obj.transform.right * new Vector2(throwForce * (isRunning ? 1.35f : 1), 0));

        anim.SetTrigger("throw");
        SoundManager.PlaySfx(throwGrenadeSound);
    }

    public void SetForce(Vector2 force)
    {
        if (meleeAttack.DetectEnemies.activeInHierarchy)
            return;

        velocity = (Vector3)force;
    }

    public void AddForce(Vector2 force)
    {
        Debug.Log("AddForce");
        velocity += (Vector3)force;
    }

    public void AddHorizontalForce(float _speed)
    {
        if (controller.collisions.ClosestHit.collider.gameObject.GetComponent<SurfaceModifier>())
            transform.Translate(_speed * Time.deltaTime, 0, 0, Space.Self);
    }

    //Called by Gamemanager script if the player is dead and there are lives
    public void RespawnAt(Vector2 pos, int checkpointDir)
    {
        Health = maxHealth;
        transform.position = new Vector3(pos.x, pos.y + 1, transform.position.z);
        gameObject.SetActive(true);

        isClimbOnTop = false;

        ResetAnimation();

        StartCoroutine(BlinkEffecrCo());

        transform.localScale = new Vector3(checkpointDir, transform.localScale.y, transform.localScale.z);
        isFacingRight = transform.localScale.x > 0;


        StartCoroutine(RestartGameCo());
    }

    //only allow the player playing after 1s
    IEnumerator RestartGameCo()
    {

        yield return new WaitForSeconds(1);
        isPlaying = true;
    }

    //public void ShowUpAnim()
    //{
    //    AnimSetTrigger("showUp");
    //    SoundManager.PlaySfx(showUpSound);
    //}

    public void SetFinishAnim()
    {
        AnimSetTrigger("showUp");
    }

    public void AnimSetTrigger(string name)
    {
        anim.SetTrigger(name);
    }

    //Called every frame in the Update()
    void HandleAnimation()
    {
        anim.SetFloat("speed", Mathf.Abs(velocity.x));
        anim.SetFloat("height_speed", velocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWall", wallSliding);
        anim.SetBool("isInWater", PlayerState == PlayerState.Water);
        anim.SetBool("isClimbLadder", playerCheckLadderZone.isClimbing && playerCheckLadderZone.currentLadder && !playerCheckLadderZone.currentLadder.GetComponent<Ladder8DirsZone>());
        playerCheckLadderZone.isClimbingLadder8Dir = playerCheckLadderZone.isClimbing && playerCheckLadderZone.currentLadder && playerCheckLadderZone.currentLadder.GetComponent<Ladder8DirsZone>();
        anim.SetBool("isClimbing8Ladder", playerCheckLadderZone.isClimbingLadder8Dir);
        anim.SetBool("isUsingJetpack", isUsingJetpack);
        anim.SetBool("running", isRunning);
        anim.SetBool("isSliding", isSliding);
        anim.SetBool("isClimbOnTop", isClimbOnTop);
        anim.SetBool("hangingRope", isRoping || isHaningRope || isGrabRopePoint);
        //anim.SetBool("isGrabLedge", isGrabbingLedge);
        anim.SetBool("isDogging", isDogging);
        anim.SetBool("isDragging", isDragging);
        anim.SetInteger("inputX", (int)input.x);
        anim.SetInteger("inputY", (int)input.y);
    }

    //public void AnimLedgeClimbFinish()
    //{
    //    transform.position = checkLedge.newLedgePos.position;
    //    isGrabbingLedge = false;
    //}

    //To make the player change to the Climb on top state, then it must be disactive the Animator after the moment
    IEnumerator DisableAnimatorCo()
    {
        yield return new WaitForSeconds(0.1f);
        anim.enabled = input.x != 0 || anim.GetBool("range_attack") || anim.GetBool("throw") || anim.GetBool("specialPower");
    }

    //Reset the animation state when the player is respawned
    void ResetAnimation()
    {
        if (anim != null)
        {
            anim.ResetTrigger("idleFront");
            anim.ResetTrigger("idleBack");
            anim.SetFloat("speed", 0);
            anim.SetFloat("height_speed", 0);
            anim.SetBool("isGrounded", true);
            anim.SetBool("isWall", false);
            anim.SetTrigger("reset");
            anim.SetBool("hangingRope", false);
        }
    }

    public void TakeDamageFromContactEnemy(float damage, Vector2 force, GameObject instigator, bool ignorePlayerThroughEnemy = false)
    {
        if (!getDamageWhenContactEnemy)
            return;

        if (goThroughEnemy && !ignorePlayerThroughEnemy)
            return;

        if (useThisContactEnemyDamage)
            damage = contactEnemyDamage;

        TakeDamage(damage, force, instigator, transform.position);

        SetForce(force);
    }


    public void TakeDamage(float damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
    {
        //		return;
        bool ignoreBlinking = false;
        //if (instigator && instigator.GetComponent<SuperBossClone>() && instigator.GetComponent<SuperBossClone>().meleeAttack.isFastAttacking)
        //    ignoreBlinking = true;

        if (!isPlaying || (isBlinking && !ignoreBlinking) || GodMode || (isDogging && doggeType == DoggeType.OverObject) || isInCannon)
            return;

        if (isBoostSpeed2)
            return;

        if (GameManager.Instance.isUsingShield)
        {
            FindObjectOfType<Shield>().Hit(instigator);
            StartCoroutine(BlinkEffecrCo());
            return;
        }

        SoundManager.PlaySfx(hurtSound, hurtSoundVolume);
        if (HurtEffect != null)
            Instantiate(HurtEffect, hitPoint, Quaternion.identity);

        Health -= (int)damage;

        if (Health <= 0)
        {
            LevelManager.Instance.KillPlayer();

            return;
        }

        if (!playerCheckLadderZone.isClimbing)
        {
            anim.SetTrigger("hurt");        //set the animation to hurt state
            //set force to player, push the player back with the current direction
            if (knockBackBeHit && damage >= damageKnockBackBeHit)
            {
                if (instigator != null)
                {
                    int dirKnockBack = (instigator.transform.position.x > transform.position.x) ? -1 : 1;
                    SetForce(new Vector2(knockbackForce * dirKnockBack, 0));
                }
            }

            if (!ignoreBlinking)
                StartCoroutine(BlinkEffecrCo());        //begin the blink effect
        }
        else
            JumpOffFromLadder();

        if (bloodScreenFXHit)
            BloodScreenUI.instance.Work();
    }

    //Do the blink effect with the blinking timer
    IEnumerator BlinkEffecrCo()
    {
        isBlinking = true;
        int blink = (int)(blinking * 0.5f / 0.2f);
        for (int i = 0; i < blink; i++)
        {
            imageCharacterSprite.color = blinkColor;
            yield return new WaitForSeconds(0.2f);
            imageCharacterSprite.color = Color.white;
            yield return new WaitForSeconds(0.2f);
        }

        imageCharacterSprite.color = Color.white;
        isBlinking = false;
    }

    //Do the God mode effect with the godTimer amount, it will be blinking while the godmode is actived
    IEnumerator GodmodeCo()
    {
        GodMode = true;
        isBlinking = false;
        godAudioSource.volume = GlobalValue.isSound ? 1 : 0;
        int blink = (int)(godTimer * 0.5f / 0.1f);
        for (int i = 0; i < blink; i++)
        {
            imageCharacterSprite.color = godBlinkColor;
            yield return new WaitForSeconds(0.1f);
            imageCharacterSprite.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }

        imageCharacterSprite.color = Color.white;

        godAudioSource.volume = 0;
        GodMode = false;
    }

    //Called by GiveHealth script item after collect it
    public void GiveHealth(int hearthToGive, GameObject instigator)
    {
        Health = Mathf.Min(Health + hearthToGive, maxHealth);
    }
    public GameObject deadFX;
    //Call by Level Manager
    public void Kill()
    {
        if (isPlaying)
        {
            anim.enabled = true;
            isPlaying = false;
            StopMove();
            soundFx.Stop();    //stop the sliding wall sound if it's playing
            anim.SetTrigger("dead");
            anim.SetBool("doorHideInOut", false);
            Health = 0;
            StopUseJetpack();
            Debug.Log("DEAD");
            isRoping = false;
            isHaningRope = false;
            isClimbOnTop = false;
            isGrabRopePoint = false;
            forceGhostFX = false;
            isRunning = false;
            ghostSprite.allowGhost = false;
            isUsingJetpack = false;
            velocity = Vector2.zero;
            if (jetpackEngine)
                jetpackEngine.SetActive(false);

            if (GameManager.Instance.isUsingShield)
            {
                FindObjectOfType<Shield>().StopShield();
            }

            SoundManager.PlaySfx(deadSound, deadSoundVolume);
        }
        else if (isUsingPowerBombAction)
        {
            anim.SetTrigger("dead");
        }
    }

    void WaterIn()
    {
        if (waterFX)
            Instantiate(waterFX, transform.position, Quaternion.identity);

        SoundManager.PlaySfx(waterIN);
        PlayerState = PlayerState.Water;
        StopRunning();
        SetupParameter();
    }

    bool isWiding = false;
    bool isInJumpWallZone = false;

    /// <summary>
    /// JUMP FALL OFF FROM LADDER
    /// </summary>
    void JumpOffFromLadder()
    {
        playerCheckLadderZone.fallingOffFromLadder = true;
        controller.HandlePhysic = false;
        playerCheckLadderZone.isInLadderZone = false;
        playerCheckLadderZone.currentLadder = null;
        playerCheckLadderZone.isClimbing = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            if (PlayerState != PlayerState.Water)
            {
                WaterIn();
            }
        }

        var isTrigger = (ITriggerPlayer)other.GetComponent(typeof(ITriggerPlayer));
        if (isTrigger != null)
            isTrigger.OnTrigger();

        if (!isPlaying)
            return;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<WindPhysic2D>())
        {
            gravity = other.GetComponent<WindPhysic2D>().Gravity;
            isWiding = true;
        }
        else if (other.CompareTag("Water"))
        {
            if (PlayerState != PlayerState.Water)
            {
                WaterIn();
            }
        }
        else if (other.CompareTag("KillZone"))
        {
            LevelManager.Instance.KillPlayer();
        }
        if (other.CompareTag("ContinueJumpWall"))
        {
            isInJumpWallZone = true;
        }
    }

    //Detect when the player move out of the water
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            if (waterFX && isPlaying)
            {
                Instantiate(waterFX, transform.position, Quaternion.identity);
                SoundManager.PlaySfx(waterOUT);
            }
            PlayerState = PlayerState.Ground;
            SetupParameter();
        }

        if (other.GetComponent<WindPhysic2D>())
        {
            gravity = originalGravity;
            isWiding = false;
        }

        if (other.CompareTag("ContinueJumpWall"))
        {
            isInJumpWallZone = false;
        }
    }

    /// <summary>
    /// Teleport Effect between 2 points
    /// Called by the Teleport script
    /// </summary>

    public void Teleport(Transform newPos, float timer, float smoothTranparent)
    {
        if (isUsingJetpack)
            StopUseJetpack();

        StartCoroutine(TeleportCo(newPos, timer, smoothTranparent));
    }


    IEnumerator TeleportCo(Transform newPos, float timer, float smoothTranparent)
    {
        StopMove();
        isPlaying = false;
        Color color = imageCharacterSprite.color;

        float delay = timer / smoothTranparent;
        float smooth = 1f / smoothTranparent;
        for (int j = 0; j < smoothTranparent; j++)
        {
            color.a = Mathf.Clamp01(color.a - smooth);
            imageCharacterSprite.color = color;
            yield return new WaitForSeconds(delay);
        }

        transform.position = newPos.position;

        for (int j = 0; j < smoothTranparent; j++)
        {
            color.a = Mathf.Clamp01(color.a + smooth);
            imageCharacterSprite.color = color;
            yield return new WaitForSeconds(delay);
        }

        color.a = 1;
        imageCharacterSprite.color = Color.white;

        isPlaying = true;
    }

    public void GoDoor(Transform newPos, float timer)
    {
        StartCoroutine(GoDoorCo(newPos, timer));
    }


    IEnumerator GoDoorCo(Transform newPos, float timer)
    {
        StopMove();
        BlackScreenUI.instance.Show(timer);
        MenuManager.Instance.HideController();

        yield return new WaitForSeconds(timer);

        anim.SetTrigger("idleFront");
        transform.position = newPos.position;
        BlackScreenUI.instance.Hide(timer);
        yield return new WaitForSeconds(timer);
        MenuManager.Instance.ShowController();
    }

    void OnBecameInvisible()
    {
    }

    public void ActiveMagnet(int time)
    {
        if (mulSpeedc != 1)
        {
            StartCoroutine(SpeedBoostCo(true));
        }
        SlideOff();
        StartCoroutine(ActiveMagnetCo(time));

        imageCharacterSprite.color = Color.white;
    }


    IEnumerator ActiveMagnetCo(int timer)
    {
        //magnet.SetActive(true);
        yield return new WaitForSeconds(timer);
        //magnet.SetActive(false);
    }

    //Use the IListener script to get the state of the game via GameManager
    #region IListener implementation

    public void IPlay()
    {

    }

    public void ISuccess()
    {
        StopMove();		//
        if(walkSoundSrc)
        walkSoundSrc.Stop();
        isPlaying = false;
        if (anim)
            anim.SetTrigger("finish");
        SoundManager.PlaySfx(finishSound);
        isUsingJetpack = false;
        if (jetpackEngine)
            jetpackEngine.SetActive(false);
    }

    public void IPause()
    {
        StopMove();
    }

    public void IUnPause()
    {

    }

    public void IGameOver()
    {
        if (this)
        {
            jetpackEngineAudio.volume = 0;
            mulSpeedc = 1;
            isBoostSpeed = false;
            allowCheckWall = true;

        }
    }

    public void IOnRespawn()
    {
        if (this)
        {
            gameObject.SetActive(true);
            imageCharacterSprite.enabled = true;
            isRoping = false;
            isGrabRopePoint = false;
            isHaningRope = false;
            transform.parent = null;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            anim.speed = 1;
            transform.position = new Vector3(transform.position.x, transform.position.y, -1);
            jetpackEnegry = 100;
            doggeStatus = 1;
            imageCharacterSprite.color = Color.white;
            GameManager.Instance.Player.gameObject.layer = LayerMask.NameToLayer("Player");
            if (isFrozen)
            {
                Forzen(false);
            }
        }
    }

    public void IOnStopMovingOn()
    {
    }

    public void IOnStopMovingOff()
    {
    }
    #endregion

    public void ExitTheRope()
    {
        isRoping = false;
        isHaningRope = false;
        transform.parent = null;
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    /// <summary>
    /// Dogge this instance.
    /// </summary>
    /// 
    bool enterGravityZoneTemp = false;
    public void EnterGravityZone()
    {
        if (isDogging)
            return;

        enterGravityZoneTemp = true;
        GameManager.Instance.Player.inverseGravity = true;
        GameManager.Instance.Player.controller.inverseGravity = true;
        GameManager.Instance.Player.transform.localScale
        = new Vector3(GameManager.Instance.Player.transform.localScale.x, -Mathf.Abs(GameManager.Instance.Player.transform.localScale.y), GameManager.Instance.Player.transform.localScale.z);
    }


    public void ExitGravityZone()
    {
        GameManager.Instance.Player.inverseGravity = false;
        GameManager.Instance.Player.controller.inverseGravity = false;

        if (!isDogging)
        {
            GameManager.Instance.Player.transform.localScale
        = new Vector3(GameManager.Instance.Player.transform.localScale.x, Mathf.Abs(GameManager.Instance.Player.transform.localScale.y), GameManager.Instance.Player.transform.localScale.z);
        }
    }

    [Header("Dogge")]
    public bool useDoggeGhostFX = true;
    public bool unlimitedDogge = false;
    public bool doggeCanMakeDamage = true;
    public GameObject doggeDamageTrigger;
    public int doggeDamage = 100;
    public float speedDogge = 20;
    public float timeDogge = 0.25f;
    //public bool allowRechargeDoggeEnegry;
    public float doggeEnegryRechargeTime = 6;

    public AudioClip soundDogge;

    [ReadOnly]
    public float doggeStatus = 1;
    public bool isDogging { get; set; }
    Vector2 doggeDirection;
    public DoggeType doggeType;
    public LayerMask doggeHitLayer;

    public bool canDoggle()
    {
        return unlimitedDogge || (!unlimitedDogge && (doggeStatus == 1));
    }

    public void Dogge()
    {
        if (isDogging)
            return;

        if (wallSliding)
            return;

        if (GameManager.Instance.Player.isSliding)
            return;
        if (GameManager.Instance.Player.isGrabRopePoint || GameManager.Instance.Player.isHaningRope || GameManager.Instance.Player.isRoping)
            return;
        if (input.y == -1)
            return;

        if (!canDoggle())
        {
            return;
        }

        if (!unlimitedDogge)
            doggeStatus = 0;

        doggeDirection = isFacingRight ? Vector2.right : Vector2.left;

        if (inverseGravity && input.y == -1)
            return;
        wallSliding = false;
        allowCheckWall = true;

        if (isDogging || doggeDirection == Vector2.zero)
            return;

        if ((doggeDirection.x > 0 && controller.collisions.right) || (doggeDirection.x < 0 && controller.collisions.left))
            StopDogge();

        if (playerCheckLadderZone.isClimbing)
            return;

        SoundManager.PlaySfx(soundDogge);
        isDogging = true;
        if (doggeCanMakeDamage && doggeDamageTrigger != null)
        {
            doggeDamageTrigger.GetComponent<GiveDamage>().Damage = doggeDamage;
            doggeDamageTrigger.SetActive(true);
        }

        enterGravityZoneTemp = false;

        if (gameObject.layer == LayerMask.NameToLayer("HidingZone"))
        {
            GameManager.Instance.Player.gameObject.layer = LayerMask.NameToLayer("Player");
        }

        Debug.Log("DoggeCo: " + doggeDirection);

        forceShadowFX = true;
        if (ghostSprite)
            ghostSprite.allowGhost = true;

        Invoke("StopDogge", timeDogge);
    }

    public void StopDogge()
    {
        if (PlayerState == PlayerState.Water)
        {
            PlayerState = PlayerState.Ground;
            SetupParameter();
        }
        isDogging = false;
        if (doggeCanMakeDamage && doggeDamageTrigger != null)
            doggeDamageTrigger.SetActive(false);

        if (isWiding)
        {
            isWiding = false;
            gravity = originalGravity;
        }

        numberOfJumpLeft = numberOfJumpMax;
        Invoke("CheckGravityZoneAfterDogge", 0.1f);
    }

    void CheckGravityZoneAfterDogge()
    {
        if (!enterGravityZoneTemp)
            ExitGravityZone();
    }

    public void PausePlayer(bool pause)
    {
        StopMove();
        isPlaying = !pause;
    }
}

[System.Serializable]
public class PlayerParameter
{
    public float moveSpeed = 3;
    public float maxJumpHeight = 3;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
}