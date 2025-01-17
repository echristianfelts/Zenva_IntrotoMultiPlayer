﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks , IPunObservable
{
    [HideInInspector]
    public int id;

    [Header ("Info")]
    public float moveSpeed;
    public float jumpForce;
    public GameObject hatObject;

    [HideInInspector]
    public float curHatTime;

    [Header("Components")]
    public Rigidbody rig;
    public Player photonPlayer;


    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;

        GameManager.instance.players[id - 1] = this;

        //give the first player the hat
        if (id == 1)
        {
            Debug.Log("Give Player one the hat...");
            GameManager.instance.GiveHat(id, true);
        }

        //if this is not our local player, disable physics as that is
        //controlled by the player and synched to all other clients.
        if(!photonView.IsMine)
        {
            rig.isKinematic = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            if(curHatTime>= GameManager.instance.timeToWin && !GameManager.instance.gameEnded)
            {
                GameManager.instance.gameEnded = true;
                GameManager.instance.photonView.RPC("WinGame", RpcTarget.All, id);

            }
        }

        // make sure no other player can control my guy but me.
        if (photonView.IsMine)
        {
            Move();

            if (Input.GetKeyDown(KeyCode.Space))
                TryJump();

            //track the amount of time we are wearing the hat.

            if(hatObject.activeInHierarchy)
            {
                curHatTime += Time.deltaTime;
            }
        }
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal") * moveSpeed;
        float z = Input.GetAxis("Vertical") * moveSpeed;

        rig.velocity = new Vector3(x, rig.velocity.y, z);

    }

    void TryJump()
    {
        Ray ray = new Ray(transform.position, Vector3.down);


        if (Physics.Raycast(ray, 0.7f))
        {
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }


    }

    public void SetHat(bool hasHat)
    {
        hatObject.SetActive(hasHat);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter");
        if(!photonView.IsMine)
        {
            return;
        }
        
        //did we hit anothre player..?

        if (collision.gameObject.CompareTag("Player") )
        {
            // do they have the hat..?
            if (GameManager.instance.GetPlayer(collision.gameObject).id == GameManager.instance.playerWithHat)
            {
                // can we get the hat?
                if (GameManager.instance.CanGetHat())
                {
                    // give us the hat
                    GameManager.instance.photonView.RPC("GiveHat", RpcTarget.All, id, false);
                }
            }
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(curHatTime);
        }
        else if(stream.IsReading)
        {
            curHatTime = (float)stream.ReceiveNext();
        }
    }

}
