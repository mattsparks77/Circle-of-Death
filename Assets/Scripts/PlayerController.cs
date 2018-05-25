using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {
	
    Rigidbody rb;
	public NetworkStartPosition[] spawnPoints;
	public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;
	public float chargeTimer = 0;
	private Vector3 moveDirection = Vector3.zero;
    public float respawnTime = 2f;
    private float stunTimer = 0;
    public int playerLives = 3;
    //private CharacterController controller;
    private Rigidbody playerRB;

    [SerializeField] private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;
    [SerializeField] private GameObject[] disableGameObjectsOnDeath;
    private int jumpableLayer = 1 << 0;
    public bool hasBomb = false;

    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private ParticleSystem spawnParticles;

    private GameObject killBox;

    void Start() {

        //controller = GetComponent<CharacterController>();
        killBox = GameObject.FindGameObjectWithTag("KillBox");
        playerRB = GetComponent<Rigidbody> ();
        hasBomb = false;
        SetupPlayer();
        //Check if this player is the client's local player
       
	}
	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag == "DeathBarrier"){
            Die();
		}
	}


    public void SetupPlayer()
    {
        if (isLocalPlayer)
        {
            this.gameObject.SetActive(true);
            //Set the camera to follow this player if it's local player
            CameraThirdPerson.SetPlayerTarget(transform);
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
            //Add this player to the game manager
            CmdAddNewPlayer();
        }
        else
        {
            //Don't let other clients control this script if it's not a local player
            this.enabled = false;
        }

    }




    [Command]
    private void CmdAddNewPlayer() {
        GameManager.AddNewPlayer(this.gameObject);
    }

    //For dealing with code that are not physics related
    void Update() {
        decrementStunTimer();
        if (Input.GetKeyDown(KeyCode.E) && hasBomb && isLocalPlayer){
            CmdDropBomb();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "DeathBarrier"){
            Die();
        }
    }
    public void Die()
    {
        //isDead = true;
        playerLives--;
        Instantiate(deathParticles, transform.position, transform.rotation);
        this.gameObject.SetActive(false);
        StartCoroutine(Respawn());

    }
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        this.gameObject.SetActive(true);
        if (isLocalPlayer)
        {
            // move back to zero location
            Vector3 spawnPoint = Vector3.zero;
           


            if (spawnPoints != null && spawnPoints.Length != 0)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }

            // Set the player’s position to the chosen spawn point
            transform.position = spawnPoint;
            SetupPlayer();
        }
    }
    
    //For dealing with code that is physics-rigidbody related
    void FixedUpdate() {
        //Turn the y-axis to face where the camera is looking at
        faceDirectionOfCamera();

        if (isOutOfBounds()) {
            CmdKillPlayer();
            this.enabled = false;
        }

        //If the player is stunned, only take in account the gravity
        if (isStunned()) {
            Vector3 velocityWGravity = playerRB.velocity;
            velocityWGravity.y -= gravity * Time.fixedDeltaTime;
            playerRB.velocity = velocityWGravity;
            return;
        }

        //Take into account gravity amount here
        moveDirection.y -= gravity * Time.fixedDeltaTime;

        if (IsGrounded) {
            //Handle movement if the player is grounded and not stunned
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
            if (isStunned()) {
                moveDirection = Vector3.zero;
            } else if (Input.GetButton("Jump"))
				moveDirection.y = jumpSpeed;   
		}
        
        //Left click to charge up movement dash 
        if (Input.GetKey (KeyCode.Mouse0)) {
			chargeTimer += Time.deltaTime;
			Debug.Log (chargeTimer.ToString ());
		}

        //If player has movement dash charged, put moveDirection to take this into account
		if (Input.GetKeyUp (KeyCode.Mouse0) && chargeTimer > 2) {
			
            //			if (moveDirection.x <= 0) {
            //				transform.forward *= 2;
            //			}
            //				controller.Move(new Vector3(10f, -1*gravity * Time.deltaTime,0f));
            //			else {
            moveDirection = moveDirection * 6f;
            
		    chargeTimer = 0;

		}

        //Set player movement to be in the corresponding direction
        playerRB.velocity = moveDirection;
    }

    //Match the player's rotation with the camera's rotation using the rigidbody in the player
    private void faceDirectionOfCamera() {
        Transform camTransform = Camera.main.GetComponent < Transform >();
        float neededYRotation = camTransform.rotation.eulerAngles.y;
        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.y = neededYRotation;
        playerRB.MoveRotation (Quaternion.Euler(currentRotation));
    }

    //Returns if the player is stunned (can't move)
    private bool isStunned() {
        return stunTimer > 0;
    }

    //Decrements and checks the stun timer if the player is stunned
    private void decrementStunTimer() {
        if (stunTimer <= 0) {
            return;
        }
        stunTimer -= Time.deltaTime;
    }

    private bool isOutOfBounds() {
        Bounds killBounds = killBox.GetComponent<Collider>().bounds;
        return !killBounds.Contains(transform.position);
    }

    [Command]
    private void CmdKillPlayer() {
        GameManager.RemovePlayer(this.gameObject);
        RpcSpawnDeathParticles();
    }

    [ClientRpc]
    private void RpcSpawnDeathParticles() {
        Instantiate(deathParticles, transform.position, transform.rotation);

        //this.gameObject.active = false;
        Destroy(gameObject);
    }

    //Public variable for stunning the player for an amount of seconds
    public void StunPlayer(float stunTime) {
        stunTimer = stunTime;
    }

    //Checks if the player is grounded
    public bool IsGrounded { get {
            CapsuleCollider col = GetComponent<CapsuleCollider>();
            Collider[] collisions = Physics.OverlapSphere(transform.position - Vector3.up * (col.height / 2 - col.radius), col.radius, jumpableLayer);
            return collisions.Length > 1 || (collisions.Length != 0 && collisions[0] != GetComponent<Collider>());
            //return Physics.Raycast(transform.position, Vector3.down, col.height / 2);
        }
    }
    
    //Drawing Gizmos for debugging purposes
    private void OnDrawGizmos() {
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position - Vector3.up * (col.height / 2 - col.radius), col.radius);
    }

    public void SetBomb(bool flag){
        hasBomb = flag;
    }
    
    [Command]
    void CmdDropBomb(){
        GameObject bomb = transform.Find("Bomb(Clone)").gameObject;
        bomb.GetComponent<BombItem>().RpcDropBomb();
    }
}
