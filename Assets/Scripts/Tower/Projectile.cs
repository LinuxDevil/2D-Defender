using UnityEngine;

public enum ProjectileType{
	rock,arrow,fireball
};

public class Projectile : MonoBehaviour {
	[SerializeField]
	private int attackStrength;

	[SerializeField]
	private ProjectileType projectileType;

	public ProjectileType PropType{
		get{
			return projectileType;
		}
	}

	public int AttackStrength{
		get{
			return attackStrength;
		}
	}
}
