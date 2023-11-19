using UnityEngine;
using System.Collections;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    public Joystick _joystick;
    public TMPro.TMP_Text txt_name;
    public PhotonView photonView;
    private GameObject currentProjectile;
    public GameObject basicProjectile;
    public GameObject ProjectileM;
    public GameObject ProjectileF;
    public GameObject ProjectileS;
    public GameObject ProjectileL;


    public BoxCollider2D myColl;

    public static int rapidsPicked = 0;
    public static float projectileSpeedKoeff = 2;

    public float moveSpeed;
    public float jumpHeight;
    public float gravity;
    public float shootDelay;

    private float originColliderSize;
    private float originColliderOffset;
    public float duckColliderSize;
    public float duckColliderOffset;

    public float pixelSize;

    public float[] shootAngles;
    private Quaternion rot;

    private Transform currentShootPoint;
    public Transform[] shootPoints;


    public int direction;

    public LayerMask solid;
    public LayerMask oneway;
    public LayerMask water;

    private float hsp;
    private float vsp;
    private float shootDelayCounter;

    private bool KeyLeft;
    private bool KeyRight;
    private bool KeyUp;
    private bool KeyDown;
    public bool KeyJump;
    private bool KeyAction;
    private bool KeyJumpOff;

    private bool onGround;
    private bool onWater;
    private bool jumped;
    private bool moving;
    private bool onPlatform;
    private bool obsticleOnRight;
    private bool obsticleOnLeft;

    private Vector2 botLeft;
    private Vector2 botRight;
    private Vector2 topLeft;
    private Vector2 topRight;

    public Animator[] animators;

    private bool isActive;
    private bool isDead;
    public float invincibilityTime;
    public float inactivityTime;
    public float invincCounter;
    private float inactCounter;
    public GameObject SpawnPoint;
    private int flashing = 0;
    public GameObject DeathEffect;
    public SpriteRenderer[] sprites;


    private void Start()
    {
        SpawnPoint = IngameManager.ins.spawnPoint.gameObject;
        transform.position = SpawnPoint.transform.position + Random.Range(-1f, 1f) * Vector3.right;
        currentProjectile = basicProjectile;
        shootDelayCounter = 0;
        rot = new Quaternion(0, 0, 0, 0);
        originColliderSize = myColl.size.y;
        originColliderOffset = myColl.offset.y;

        IngameManager.ins.players.Add(this);

        // if(PhotonNetwork.IsMasterClient)
        // {
            // Debug.LogError("PhotonNetwork.IsMasterClient: " + PhotonNetwork.IsMasterClient);
        // }
    }


    // Спавн
    private void Spawn()
    {
        invincCounter = invincibilityTime;
        rapidsPicked = 0;
        currentProjectile = basicProjectile;
        jumped = true;
        vsp = 0.1f;
    }

    // Смерть
    public void Death()
    {
        if(IngameManager.ins.win) return;
        
        if(photonView.IsMine)
        {
            if (KeyDown && onWater) return;
            if (invincCounter > 0) return;
            PhotonNetwork.Instantiate("Game/" + DeathEffect.name, transform.position, transform.rotation);
            transform.position = SpawnPoint.transform.position;        
            isDead = true;
            isActive = false;
            inactCounter = inactivityTime;
        }
    }

    private void Update()
    {
        if (!isActive)
        {
            inactCounter -= Time.deltaTime;
            if (inactCounter < 0)
            {
                isActive = true;
                Spawn();
            }
            return;
        }
        CalculateBounds();

        if(photonView.IsMine)
        {
            onGround =
                CheckCollision(botLeft, Vector2.down, pixelSize, solid) ||
                CheckCollision(botRight, Vector2.down, pixelSize, solid) ||
                CheckCollision(botLeft, Vector2.down, pixelSize, oneway) ||
                CheckCollision(botRight, Vector2.down, pixelSize, oneway);
            onPlatform =
                CheckCollision(botLeft, Vector2.down, pixelSize, oneway) ||
                CheckCollision(botRight, Vector2.down, pixelSize, oneway);

        }
        onWater =
            CheckCollision(botLeft, Vector2.down, pixelSize, water) ||
            CheckCollision(botRight, Vector2.down, pixelSize, water);

        if (invincCounter > 0)
        {
            invincCounter -= Time.deltaTime;
            flashing++;
            if (flashing > 15)
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    sprites[i].enabled = !sprites[i].enabled;
                    if (i == 2 && onWater) sprites[2].enabled = false;
                    if (i == 3 && !onWater) sprites[3].enabled = false;
                    if (i == 3 && onWater) sprites[3].enabled = enabled;
                }
                flashing = 0;
            }
        }
        else
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i].enabled = true;
                if (i == 2 && onWater) sprites[2].enabled = false;
                if (i == 2 && !onWater) sprites[2].enabled = true;
                if (i == 3 && !onWater) sprites[3].enabled = false;
                if (i == 3 && onWater) sprites[3].enabled = enabled;
            }
            flashing = 0;
        }


        if(photonView.IsMine)
        {
            obsticleOnRight = CheckCollision(topRight, Vector2.right, pixelSize, solid) || CheckCollision(botRight, Vector2.right, pixelSize, solid);
            obsticleOnLeft = CheckCollision(topLeft, Vector2.left, pixelSize, solid) || CheckCollision(botLeft, Vector2.left, pixelSize, solid);
            GetInput();
            if (onGround && KeyDown)
            {
                myColl.size = new Vector2(myColl.size.x, duckColliderSize);
                myColl.offset = new Vector2(myColl.offset.x, duckColliderOffset);
            }
            else
            {
                myColl.size = new Vector2(myColl.size.x, originColliderSize);
                myColl.offset = new Vector2(myColl.offset.x, originColliderOffset);

            }
        
            CalculateDirection();
            CalculateShootAngles();
            CalculateShootPoint();
            Move();
            Shoot();
            Animate();
        }
    }

    private void GetInput()
    {
        #if UNITY_EDITOR
            KeyLeft = Input.GetKey(KeyCode.LeftArrow);
            KeyRight = Input.GetKey(KeyCode.RightArrow);
            KeyUp = Input.GetKey(KeyCode.UpArrow);
            KeyDown = Input.GetKey(KeyCode.DownArrow);
            KeyJump = Input.GetKeyDown(KeyCode.Space);
            KeyAction = Input.GetKey(KeyCode.E);
            KeyJumpOff = KeyDown && KeyJump;


        #else
            KeyLeft = _joystick.Horizontal < 0 && (Mathf.Abs(_joystick.Horizontal) > 0.5f * Mathf.Abs(_joystick.Vertical));
            KeyRight = _joystick.Horizontal > 0 && (Mathf.Abs(_joystick.Horizontal) > 0.5f * Mathf.Abs(_joystick.Vertical));
            KeyUp = _joystick.Vertical > 0 && (Mathf.Abs(_joystick.Vertical) > 0.5f * Mathf.Abs(_joystick.Horizontal));
            KeyDown = _joystick.Vertical < 0 && (Mathf.Abs(_joystick.Vertical) > 0.5f * Mathf.Abs(_joystick.Horizontal));
            // KeyJump = Input.GetKeyDown(KeyCode.Space);
            KeyAction = true;
            KeyJumpOff = KeyDown && KeyJump;
            
        #endif
        
        
    }

    private void Move()
    {
        if (KeyLeft && !obsticleOnLeft)
        {
            hsp = -moveSpeed * Time.deltaTime;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (KeyRight && !obsticleOnRight)
        {
            hsp = moveSpeed * Time.deltaTime;
            transform.localScale = new Vector3(1, 1, 1);
        }
        if (KeyRight || KeyLeft) moving = true;
        if ((!KeyLeft && !KeyRight) || (KeyLeft && KeyRight))
        {
            moving = false;
            hsp = 0;
        }


        // спрыгиваем с платформы
        if (onPlatform && KeyJumpOff)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y - pixelSize);
            onGround = false;
        }

        if (KeyJump && onGround)
        {
            jumped = true;
            if(!onWater) vsp = jumpHeight * 0.6f;
            else vsp = jumpHeight* 0.6f;
            onGround = false;
        }

        if (!onGround) vsp -= gravity * Time.deltaTime;

        // проверяем пол под ногами
        if ((vsp < 0) && (CheckCollision(botLeft, Vector2.down, Mathf.Abs(vsp), solid) || CheckCollision(botRight, Vector2.down, Mathf.Abs(vsp), solid)))
        {
            float dist1 = CheckCollisionDistance(botLeft, Vector2.down, Mathf.Abs(vsp), solid);
            float dist2 = CheckCollisionDistance(botRight, Vector2.down, Mathf.Abs(vsp), solid);
            if (dist1 <= dist2) vsp = -dist1;
            else vsp = -dist2;
            transform.position = new Vector2(transform.position.x, transform.position.y + vsp + pixelSize / 2);
            vsp = 0;
        }
        // проверяем платформу под ногами
        if ((vsp < 0) && (CheckCollision(botLeft, Vector2.down, Mathf.Abs(vsp), oneway) || CheckCollision(botRight, Vector2.down, Mathf.Abs(vsp), oneway)))
        {
            float dist1 = CheckCollisionDistance(botLeft, Vector2.down, Mathf.Abs(vsp), oneway);
            float dist2 = CheckCollisionDistance(botRight, Vector2.down, Mathf.Abs(vsp), oneway);
            if (dist1 <= dist2) vsp = -dist1;
            else vsp = -dist2;
            transform.position = new Vector2(transform.position.x, transform.position.y + vsp + pixelSize / 2);
            vsp = 0;
        }



        // проверяем потолок
        if ((vsp > 0) && (CheckCollision(topLeft, Vector2.up, vsp, solid) || CheckCollision(topRight, Vector2.up, vsp, solid)))
        {
            float dist1 = CheckCollisionDistance(topLeft, Vector2.up, vsp, solid);
            float dist2 = CheckCollisionDistance(topRight, Vector2.up, vsp, solid);
            if (dist1 <= dist2) vsp = dist1;
            else vsp = dist2;
            transform.position = new Vector2(transform.position.x, transform.position.y + vsp + pixelSize / 2);
            vsp = 0;
        }

        // проверяем стену справа
        if ((hsp > 0) && (CheckCollision(topRight, Vector2.right, hsp, solid) || CheckCollision(botRight, Vector2.right, hsp, solid)))
        {
            float dist1 = CheckCollisionDistance(topRight, Vector2.right, hsp, solid);
            float dist2 = CheckCollisionDistance(botRight, Vector2.right, hsp, solid);
            if (dist1 <= dist2) hsp = dist1;
            else hsp = dist2;
            transform.position = new Vector2(transform.position.x + hsp, transform.position.y);
            hsp = 0;
        }

        // проверяем стену слева
        if ((hsp < 0) && (CheckCollision(topLeft, Vector2.left, Mathf.Abs(hsp), solid) || CheckCollision(botLeft, Vector2.left, Mathf.Abs(hsp), solid)))
        {
            float dist1 = CheckCollisionDistance(topLeft, Vector2.left, Mathf.Abs(hsp), solid);
            float dist2 = CheckCollisionDistance(botLeft, Vector2.left, Mathf.Abs(hsp), solid);
            if (dist1 <= dist2) hsp = -dist1;
            else hsp = -dist2;
            transform.position = new Vector2(transform.position.x + hsp, transform.position.y);
            hsp = 0;
        }


        if (vsp == 0) jumped = false;

        transform.position = new Vector2(transform.position.x + hsp, transform.position.y + vsp);
    }

    // public void PlayerJump()
    // {
    //     if (KeyJump && onGround)
    //     {
    //         jumped = true;
    //         if(!onWater) vsp = jumpHeight * 0.6f;
    //         else vsp = jumpHeight* 0.6f;
    //         onGround = false;
    //     }
    // }

    // Стрельба
    private void Shoot()
    {
        if (KeyDown && onWater) return;
        if (KeyAction && shootDelayCounter <= 0)
        {
            if ((currentProjectile == basicProjectile) )
            {
                SpawnBullet();
                shootDelayCounter = shootDelay;
            }
            else if (currentProjectile == ProjectileM)
            {
                SpawnBullet();
                shootDelayCounter = shootDelay;
            }
            else if ((currentProjectile == ProjectileF) )
            {
                SpawnBullet();
                shootDelayCounter = shootDelay;
            }
            else if ((currentProjectile == ProjectileS) )
            {
                SpawnBullet();
                shootDelayCounter = shootDelay;
            }
            else if (currentProjectile == ProjectileL)
            {
                SpawnBullet();
                shootDelayCounter = shootDelay;
            }

        }
        shootDelayCounter -= Time.deltaTime;
    }

    private void SpawnBullet()
    {
        PhotonNetwork.Instantiate("Game/" + basicProjectile.name, currentShootPoint.position, rot);
    }

    // Рассчитать направление
    private void CalculateDirection()
    {
        if (KeyUp && !KeyRight && !KeyLeft && !KeyDown)
        {
            direction = 8;
        }
        else if (jumped && KeyDown && !KeyRight && !KeyLeft) direction = 2;
        else if (transform.localScale.x > 0)
        {
            if (KeyUp && KeyRight) direction = 9;
            else if (KeyDown && KeyRight) direction = 3;
            else if (KeyDown && !KeyRight) direction = 6;
            else direction = 6;
        }
        else if (transform.localScale.x < 0)
        {
            if (KeyUp && KeyLeft) direction = 7;
            else if (KeyDown && KeyLeft) direction = 1;
            else if (KeyDown && !KeyLeft) direction = 4;
            else direction = 4;
        }

    }

    // Вычислить точку для стрельбы
    private void CalculateShootPoint()
    {
        if (onGround && direction == 8) currentShootPoint = shootPoints[0];
        if (!jumped && (direction == 9 || direction == 7)) currentShootPoint = shootPoints[1];
        if (!jumped && (direction == 4 || direction == 6)) currentShootPoint = shootPoints[2];
        if (!jumped && (direction == 1 || direction == 3)) currentShootPoint = shootPoints[3];
        if (!jumped && KeyDown) currentShootPoint = shootPoints[4];
        if (!onGround && KeyDown) currentShootPoint = shootPoints[2];
        if (jumped) currentShootPoint = transform;
    }

    // Рассчитать углы стрельбы
    private void CalculateShootAngles()
    {
        if (direction == 8) rot = Quaternion.Euler(transform.rotation.x, transform.rotation.y, shootAngles[0]);
        if (direction == 9) rot = Quaternion.Euler(transform.rotation.x, transform.rotation.y, -shootAngles[1]);
        if (direction == 6) rot = Quaternion.Euler(transform.rotation.x, transform.rotation.y, -shootAngles[2]);
        if (direction == 3) rot = Quaternion.Euler(transform.rotation.x, transform.rotation.y, -shootAngles[3]);
        if (direction == 2) rot = Quaternion.Euler(transform.rotation.x, transform.rotation.y, shootAngles[4]);
        if (direction == 7) rot = Quaternion.Euler(transform.rotation.x, transform.rotation.y, shootAngles[1]);
        if (direction == 4) rot = Quaternion.Euler(transform.rotation.x, transform.rotation.y, shootAngles[2]);
        if (direction == 1) rot = Quaternion.Euler(transform.rotation.x, transform.rotation.y, shootAngles[3]);
    }

    // Проверка столкновения
    private bool CheckCollision(Vector2 raycastOrigin, Vector2 direction, float distance, LayerMask layer)
    {
        return Physics2D.Raycast(raycastOrigin, direction, distance, layer);
    }

    // Проверка дистанции до столкновения
    private float CheckCollisionDistance(Vector2 raycastOrigin, Vector2 direction, float distance, LayerMask layer)
    {
        int i = 0;
        while (Physics2D.Raycast(raycastOrigin, direction, distance, layer))
        {
            i++;

            if (distance > pixelSize) distance -= pixelSize;
            else distance = pixelSize; /// может забаговаться

            if (i > 1000) return 0;
        }
        return distance;
    }

    // пересчитать углы
    Bounds b;
    private void CalculateBounds()
    {
        b = myColl.bounds;
        topLeft = new Vector2(b.min.x, b.max.y);
        botLeft = new Vector2(b.min.x, b.min.y);
        topRight = new Vector2(b.max.x, b.max.y);
        botRight = new Vector2(b.max.x, b.min.y);
    }

    // Анимация
    private void Animate()
    {
        for (int i = 0; i < 2; i++)
        {
            animators[i].SetBool(Constant.anim_OnGround, onGround);
            animators[i].SetBool(Constant.anim_Jumped, jumped);
            animators[i].SetBool(Constant.anim_Moving, moving);
            animators[i].SetBool(Constant.anim_Shooting, KeyAction);
            animators[i].SetBool(Constant.anim_KeyDown, KeyDown);
            animators[i].SetFloat(Constant.anim_VSP, vsp);
            animators[i].SetInteger(Constant.anim_Direction, direction);
        }
    }

    // Сменить оружие
    public void ChangeWeapon(int type)
    {
        /*
        * 0 - R
        * 1 - M
        * 2 - F
        * 3 - S
        * 4 - L
        */
        switch (type)
        {
            case 0:
                break;
            case 1:
                currentProjectile = ProjectileM;
                break;
            case 2:
                currentProjectile = ProjectileF;
                break;
            case 3:
                currentProjectile = ProjectileS;
                break;
            case 4:
                currentProjectile = ProjectileL;
                break;
        }

    }
}
