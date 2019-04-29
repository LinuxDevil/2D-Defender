using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {

	[SerializeField]
	private float timeBetweenAttacks;
	[SerializeField]
	private float attackRadius;
	[SerializeField]
	private Projectile projectile;
	private Enemy targetEnemy = null;
	private float attackCounter;
	private bool isAttacking = false;

	// Use this for initialization
	void Start () {
			
	}
	
	// Update is called once per frame
	void Update () {

		attackCounter -= Time.deltaTime;
		if (targetEnemy == null || targetEnemy.IsDead) {
			Enemy nearestEnemy = GetNearestEnemyInRage ();
			if (nearestEnemy != null && Vector2.Distance (transform.position, nearestEnemy.transform.localPosition) <= attackRadius) {
				targetEnemy = nearestEnemy;
			}
		} else {
			if (Vector2.Distance (transform.localPosition, targetEnemy.transform.localPosition) > attackRadius) {
				targetEnemy = null;
			}

			if (attackCounter <= 0) {
				isAttacking = true;
				//reset attack counter
				attackCounter = timeBetweenAttacks;
			} else {
				isAttacking = false;
			}
		}



	}

	void FixedUpdate(){
		if (isAttacking)
			Attack ();
	}

	public void Attack(){
		isAttacking = false;
		Projectile newPorjectTile = Instantiate (projectile) as Projectile;
		newPorjectTile.transform.localPosition = transform.localPosition;
		if (newPorjectTile.PropType == ProjectileType.arrow) {
			GameManager.Instance.AudioSource.PlayOneShot (SoundManager.Instance.Arrow);
		} else if (newPorjectTile.PropType == ProjectileType.fireball) {
			GameManager.Instance.AudioSource.PlayOneShot (SoundManager.Instance.Fireball);
		} else if(newPorjectTile.PropType == ProjectileType.rock) {
			GameManager.Instance.AudioSource.PlayOneShot (SoundManager.Instance.Rock);
		}
		if (targetEnemy == null) {
			Destroy (newPorjectTile.gameObject);
		} else {
			//Move the projectile
			StartCoroutine(LunchProjectile(newPorjectTile));
		}
	}

	IEnumerator LunchProjectile(Projectile projectile){
		while (getTargetDistance(targetEnemy) > 0.20f && projectile != null && targetEnemy != null) {
			var dir = targetEnemy.transform.localPosition - transform.localPosition;
			var angleDirection = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
			projectile.transform.rotation = Quaternion.AngleAxis (angleDirection, Vector3.forward);
			projectile.transform.localPosition = Vector2.MoveTowards (projectile.transform.localPosition, targetEnemy.transform.position, 5f * Time.deltaTime);
			yield return null;
		}
		if (projectile != null || targetEnemy == null) {
			Destroy (projectile.gameObject);
		}
	}

	public float getTargetDistance(Enemy enemy){
		if (enemy == null) {
			enemy = GetNearestEnemyInRage ();
			if (enemy == null) {
				return 0f;
			}
		}
			
		return Mathf.Abs (Vector2.Distance (transform.position, enemy.transform.position));
	}

	private List<Enemy> GetEnemiesInRage(){

		List<Enemy> enemiesInRage = new List<Enemy> ();

		foreach(Enemy enemy in GameManager.Instance.EnemyList){
			if (Vector2.Distance (transform.position, enemy.transform.position) <= attackRadius) {
				enemiesInRage.Add (enemy);
			}
		}

		return enemiesInRage;
	}

	private Enemy GetNearestEnemyInRage(){
		Enemy nearestEnemy = null;
		float smallestDistance = float.PositiveInfinity;

		foreach (Enemy enemey in GetEnemiesInRage()) {
			if (Vector2.Distance (transform.localPosition, enemey.transform.localPosition) < smallestDistance) {
				smallestDistance = Vector2.Distance (transform.localPosition, enemey.transform.localPosition);
				nearestEnemy = enemey;
			}
		}

		return nearestEnemy;

	}
}
