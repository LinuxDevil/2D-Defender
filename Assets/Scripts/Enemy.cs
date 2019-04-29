using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {	

	[SerializeField]
	private Transform exitPoint;
	[SerializeField]
	private Transform[] waypoints;
	[SerializeField]
	private float navigationUpdate;
	[SerializeField]
	private int healthPoints;
	[SerializeField]
	private int rewardAmount;

	private int target = 0;
	private Transform enemy;
	private Collider2D enemyCollider;
	private Animator anim;
	private float navigationTime = 0;
	private bool isDead = false;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		enemy = GetComponent<Transform> ();
		enemyCollider = GetComponent<Collider2D> ();
		GameManager.Instance.RegisterEnemy (this);
	}
	
	// Update is called once per frame
	void Update () {
		if (waypoints != null && !isDead) {
			navigationTime += Time.deltaTime;
			if (navigationTime > navigationUpdate) {
				if (target < waypoints.Length) {
					enemy.position = Vector2.MoveTowards (enemy.position, waypoints [target].position, navigationTime);
				} else {
					enemy.position = Vector2.MoveTowards (enemy.position, exitPoint.position, navigationTime);
				}

				navigationTime = 0;
			}
		}
	}


	void OnTriggerEnter2D(Collider2D collider){
		if (collider.tag == "Checkpoint") {
			target++;
		} else if (collider.tag == "Finish") {
			GameManager.Instance.RoundEscaped += 1;
			GameManager.Instance.TotalEscaped += 1;
			GameManager.Instance.UnregisterEnemy (this);
			GameManager.Instance.isWaveOver ();
		} else if (collider.tag == "Projectile") {
			Projectile newP = collider.GetComponent<Projectile> ();
			enemyHit (newP.AttackStrength);
			Destroy (collider.gameObject);
		}
	}

	public void enemyHit(int hitpoints){
		if (healthPoints - hitpoints > 0){
			healthPoints -= hitpoints;
			GameManager.Instance.AudioSource.PlayOneShot (SoundManager.Instance.Hit);
			anim.Play ("Hurt");
		}else {
			anim.SetTrigger ("didDie");
			die();
		}
	}

	public void die(){
		isDead = true;
		enemyCollider.enabled = false;
		GameManager.Instance.TotalKilled += 1;
		GameManager.Instance.AudioSource.PlayOneShot (SoundManager.Instance.Death);
		GameManager.Instance.addMoney (rewardAmount);
		GameManager.Instance.isWaveOver ();
	}

	public bool IsDead{
		get{
			return isDead;
		}
	}

}