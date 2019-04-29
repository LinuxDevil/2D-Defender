﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerManager : Singleton<TowerManager> {

	public TowerButton towerButtonPressed{ get; set;}

	private SpriteRenderer spriteRenderer;

	private List<Tower> TowerList = new List<Tower> ();
	private List<Collider2D> BuildList = new List<Collider2D> ();
	private Collider2D buildTile;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer> ();		
		buildTile = GetComponent<Collider2D> ();
		spriteRenderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (0)) {
			Vector2 worldPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast (worldPoint, Vector2.zero);

			if (hit.collider.tag == "BuildSite") {
				buildTile = hit.collider;
				buildTile.tag = "BuildSiteFull";
				RegisterBuildSite (buildTile);
				placeTower (hit);
			}
				
		}

		if (spriteRenderer.enabled) {
			followMouse ();
		}
		 
	}

	public void RegisterBuildSite(Collider2D buildTag){
		BuildList.Add (buildTag);
	}

	public void RegisterTower(Tower tower){
		TowerList.Add (tower);
	}
		
	public void RenameTagsBuildSites(){
		foreach (Collider2D col in BuildList) {
			col.tag = "BuildSite";
		}

		BuildList.Clear ();
	}

	public void DestroyAllTowers(){
		foreach (Tower tower in TowerList) {
			Destroy (tower.gameObject);
		}

		TowerList.Clear ();
	}

	public void placeTower(RaycastHit2D hit){
		if (!EventSystem.current.IsPointerOverGameObject() && towerButtonPressed != null) {
			Tower newTower = Instantiate (towerButtonPressed.TowerObject);
			newTower.transform.position = hit.transform.position;
			buyTower (towerButtonPressed.TowerPrice);
			GameManager.Instance.AudioSource.PlayOneShot (SoundManager.Instance.TowerBuilt);
			RegisterTower (newTower);
			disableDragSprite ();
		}
	}

	public void buyTower(int price){
		GameManager.Instance.subtractMoney (price);
	}

	public void selectedTower(TowerButton towerSelected){
		if (towerSelected.TowerPrice <= GameManager.Instance.TotalMoney) {
			towerButtonPressed = towerSelected;
			enableDragSprite (towerButtonPressed.DragSprite);
		}
	}

	public void followMouse(){
		transform.position = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		transform.position = new Vector2 (transform.position.x, transform.position.y);
	}

	public void enableDragSprite(Sprite sprite){
		spriteRenderer.enabled = true;
		spriteRenderer.sprite = sprite;
	}

	public void disableDragSprite(){
		spriteRenderer.enabled = false;
	}

}
	