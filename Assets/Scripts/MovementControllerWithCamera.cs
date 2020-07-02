﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementControllerWithCamera : MonoBehaviour
{
    public GameObject mainCamera;
    public AudioClip jumpSound;

    public float verticalVelocity =  30;

    public int speedFactor = 50;
    public int maxVelocity = 20;
    public int speedFactorIncreaseOnSprint = 40;
    public int maxVelocityIncreaseOnSprint = 20;

    public float deltaSinceJump =  0.5f;

    Rigidbody rigidBody;
    
    int collidingJumpPlatformsCount = 0;
    
    float deltaTimeJump = 0;

    bool isSprinting = false;

    void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        ProccessUserInput();
        deltaTimeJump += Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Config.JUMPABLE_TAG))
        {
            collidingJumpPlatformsCount++;

            if(gameObject.transform.parent != null)
            {
                GameObject temp = gameObject.transform.parent.gameObject;
                gameObject.transform.parent = null;
                Destroy(temp);
            }

            var emptyObject = new GameObject();
            emptyObject.name = Config.PLAYER_PARENT_OBJECT_NAME;
            emptyObject.transform.rotation = collision.transform.rotation;
            emptyObject.transform.parent = collision.gameObject.transform;
            gameObject.transform.parent = emptyObject.transform;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(Config.JUMPABLE_TAG))
        {
            collidingJumpPlatformsCount--;

            if (gameObject.transform.parent != null)
            {
                GameObject temp = gameObject.transform.parent.gameObject;
                gameObject.transform.parent = null;
                Destroy(temp);
            }
        }
    }

    void ProccessUserInput()
    {

        float posX = transform.position.x - mainCamera.transform.position.x;
        float posZ = transform.position.z - mainCamera.transform.position.z;

        if (Input.GetKey(Config.jumpKeyCode) && collidingJumpPlatformsCount > 0 && deltaTimeJump > deltaSinceJump)
        {
            deltaTimeJump = 0f;
            rigidBody.velocity += new Vector3(0, verticalVelocity, 0);
            gameObject.GetComponent<AudioSource>().PlayOneShot(jumpSound);
        }

        if (Input.GetKey(Config.leftMovementKeyCode))
        {
            rigidBody.velocity += Quaternion.Euler(0, -90, 0) * new Vector3(posX, 0, posZ).normalized * Time.deltaTime * speedFactor;
        }

        if (Input.GetKey(Config.forwardMovementKeyCode))
        {
            rigidBody.velocity += new Vector3(posX, 0, posZ).normalized * Time.deltaTime * speedFactor;
        }

        if (Input.GetKey(Config.rightMovementKeyCode))
        {
            rigidBody.velocity += Quaternion.Euler(0, -90, 0) * -new Vector3(posX, 0, posZ).normalized * Time.deltaTime * speedFactor;
        }

        if (Input.GetKey(Config.backwardMovementKeyCode))
        {
            rigidBody.velocity -= new Vector3(posX, 0, posZ).normalized * Time.deltaTime * speedFactor;
        }

        if (Input.GetKeyDown(Config.sprintKeyCode))
        {
            isSprinting = true;
            maxVelocity += maxVelocityIncreaseOnSprint;
            speedFactor += speedFactorIncreaseOnSprint;
        }

        else if (Input.GetKeyUp(Config.sprintKeyCode))
        {
            isSprinting = false;
            maxVelocity -= maxVelocityIncreaseOnSprint;
            speedFactor -= speedFactorIncreaseOnSprint;
        }

        LimitVelocity();
    }

    public void PrepareForDisabling()
    {
        if(isSprinting)
        {
            maxVelocity -= maxVelocityIncreaseOnSprint;
            speedFactor -= speedFactorIncreaseOnSprint;
        }
    }

    public void PrepareForEnabling()
    {
        if(Input.GetKey(Config.sprintKeyCode))
        {
            maxVelocity += maxVelocityIncreaseOnSprint;
            speedFactor += speedFactorIncreaseOnSprint;
        }
    }

    void LimitVelocity()
    {
        if (rigidBody.velocity.x > maxVelocity)
        {
            rigidBody.velocity = new Vector3(maxVelocity, rigidBody.velocity.y, rigidBody.velocity.z);
        }

        if (rigidBody.velocity.z < -maxVelocity)
        {
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y, -maxVelocity);
        }

        if (rigidBody.velocity.z > maxVelocity)
        {
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y, maxVelocity);
        }

        if (rigidBody.velocity.x < -maxVelocity)
        {
            rigidBody.velocity = new Vector3(-maxVelocity, rigidBody.velocity.y, rigidBody.velocity.z);
        }
    }
}
