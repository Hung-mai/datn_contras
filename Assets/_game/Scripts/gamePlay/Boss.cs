using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Boss : MonoBehaviour
{
    public EnemyManager LeftCannon;
    public EnemyManager RightCannon;
    public GameObject Target;
    public GameObject Projectile;
    public float ShotDelay;
    float counterLeft;
    float counterRight;
    public bool isActive;


    private void Start()
    {
        counterLeft = ShotDelay;
        counterRight = 0;
    }

    private void OnBecameVisible()
    {
        isActive = true;
    }

    private void Update()
    {
        if (!isActive) return;
        if (Target == null)
        {
            if (LeftCannon != null) LeftCannon.Die();
            if (RightCannon != null) RightCannon.Die();
            PhotonNetwork.Destroy(gameObject);

            // dến đây là thắng, hiển thị win game, win xong thì back ra home
            SoundManager.PlayEfxSound(SoundManager.ins.bossDefeat);
            IngameManager.ins.win = true;
            Timer.Schedule(IngameManager.ins, 2, () => {
                IngameManager.ins.WinGame();
            });
        }

        if (LeftCannon != null)
        {
            if(counterLeft <= 0)
            {
                PhotonNetwork.Instantiate("Enemy/" + Projectile.name, LeftCannon.transform.position, LeftCannon.transform.rotation);
                counterLeft = ShotDelay + Random.Range(-1, 0);
            }
            else
            {
                counterLeft -= Time.deltaTime;
            }
        }
        if (RightCannon != null)
        {
            if (counterRight <= 0)
            {
                PhotonNetwork.Instantiate("Enemy/" + Projectile.name, RightCannon.transform.position, RightCannon.transform.rotation);
                counterRight = ShotDelay + Random.Range(-1, 0);
            }
            else
            {
                counterRight -= Time.deltaTime;
            }
        }

    }
}
