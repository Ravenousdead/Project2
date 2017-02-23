using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Sprites;

public class EnemyTool : EditorWindow {

	public List<Enemies> enemyList = new List<Enemies>();
	public List<string> enemyNameList = new List<string>();
	public string[] enemyNameArray;

	string enemyName = "";
	Sprite enemySprite;
	int enemyHealth = 0;
	int enemyAttack = 0;
	int enemyDefense = 0;
	int enemyAgility = 0;
	float attackTime = 0f;
	bool isMagicUser = false;
	int enemyMana = 0;

	bool isntSprited = false;
	bool isntNamed = false;
	bool alreadyExists = false;
	int currentChoice = 0;
	int lastChoice;



	[MenuItem("Custom Tool/Enemy Tools")]
	private static void CanBeNamedAnything()
	{
		EditorWindow.GetWindow<EnemyTool> ();
	}


	// calls when the window is opened
	void Awake() {
		getEnemies();
	}



	void OnGUI() {

		// moves the focus to the top of the tool when called
		GUI.SetNextControlName ("Top");

		currentChoice = EditorGUILayout.Popup (currentChoice, enemyNameArray);

		// if there is a texture, make it a texture and display as a label
		if (enemySprite != null) {
			Texture2D spriteTexture = SpriteUtility.GetSpriteTexture (enemySprite, false);
			// center sprite
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			// dispaly the sprite texture
			GUILayout.Label (spriteTexture);
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
		}

		// Display enemy attributes
		enemySprite  = EditorGUILayout.ObjectField (enemySprite, typeof(Sprite), false) as Sprite;
		enemyName    = EditorGUILayout.TextField (new GUIContent("Name: ","The name will also be used to name the file."), enemyName);
		enemyHealth  = EditorGUILayout.IntSlider ("Health: ", enemyHealth, 2, 300);
		enemyAttack  = EditorGUILayout.IntSlider ("Attack: ", enemyAttack, 2, 100);
		enemyDefense = EditorGUILayout.IntSlider ("Defense: ", enemyDefense, 2, 100);
		enemyAgility = EditorGUILayout.IntSlider ("Agility: ", enemyAgility, 2, 100);
		attackTime   = EditorGUILayout.Slider ("Attack Time: ", attackTime, 1, 20);
		isMagicUser  = EditorGUILayout.BeginToggleGroup ("Magic User: ", isMagicUser);
		enemyMana    = EditorGUILayout.IntSlider ("Mana Pool: ", enemyMana, 0, 100);
		EditorGUILayout.EndToggleGroup ();

		// Create / Save / Save As..  Buttons
		if (currentChoice == 0) {
			if (GUILayout.Button ("Create")) {
				createEnemy ();
			}
		} else if(enemyName != enemyList [currentChoice - 1].emname) {
			if (GUILayout.Button ("Save As..")) {
				createEnemy ();
			}
		} else {
			if (GUILayout.Button ("Save")) {
				saveCurrentEnemy ();
			}
		}

		// calls newEnemy or currentEnemy when a new enemy is selected
		if (currentChoice != lastChoice) {
			if (currentChoice == 0) {
				newEnemy ();
			} else {
				currentEnemy ();
			}
			lastChoice = currentChoice;
		}

		// send a message if there isn't a sprite
		if (isntSprited && enemySprite == null) {
			EditorGUILayout.HelpBox ("A sprite must be selected!", MessageType.Error);
		} else {
			isntSprited = false;
		}

		// send a message if the name field isn't filled out
		if (isntNamed && enemyName == "") {
			EditorGUILayout.HelpBox ("Name can not be blank!", MessageType.Error);
		} else {
			isntNamed = false;
		}

		// send a message if the name is already being used
		if (alreadyExists) {
			EditorGUILayout.HelpBox ("Enemy by that name already exists.", MessageType.Warning);
		}
	}



	// clear and refill the enemy lists
	private void getEnemies ()
	{
		enemyList.Clear ();
		enemyNameList.Clear ();

		string[] guids = AssetDatabase.FindAssets ("t:Enemies");
		foreach (string guid in guids) {
			string pathString = AssetDatabase.GUIDToAssetPath (guid);
			Enemies enemyInst = AssetDatabase.LoadAssetAtPath (pathString, typeof(Enemies)) as Enemies;
			enemyNameList.Add (enemyInst.emname);
			enemyList.Add (enemyInst);
		}

		enemyNameList.Insert(0, "New");
		enemyNameArray = enemyNameList.ToArray ();
	}


	// set the attributes to starting values
	private void newEnemy()
	{
		enemySprite = null;
		enemyName = "";
		enemyHealth = 2;
		enemyAttack = 2;
		enemyDefense = 2;
		enemyAgility = 2;
		attackTime = 1f;
		isMagicUser = false;
		enemyMana = 0;

		alreadyExists = false;
		isntSprited = false;
		isntNamed = false;
	}


	// load the attributes from the current enemy into the gui variables
	private void currentEnemy() {
		enemySprite  = enemyList [currentChoice - 1].mySprite;
		enemyName    = enemyList [currentChoice - 1].emname;
		enemyHealth  = enemyList [currentChoice - 1].health;
		enemyAttack  = enemyList [currentChoice - 1].atk;
		enemyDefense = enemyList [currentChoice - 1].def;
		enemyAgility = enemyList [currentChoice - 1].agi;
		attackTime   = enemyList [currentChoice - 1].atkTime;
		isMagicUser  = enemyList [currentChoice - 1].isMagic;
		enemyMana    = enemyList [currentChoice - 1].manaPool;
	}



	// save the current attributes to a predefined enemy
	private void saveCurrentEnemy () {
		enemyList [currentChoice - 1].mySprite = enemySprite;
		enemyList [currentChoice - 1].emname   = enemyName;
		enemyList [currentChoice - 1].health   = enemyHealth;
		enemyList [currentChoice - 1].atk      = enemyAttack;
		enemyList [currentChoice - 1].def      = enemyDefense;
		enemyList [currentChoice - 1].agi      = enemyAgility;
		enemyList [currentChoice - 1].atkTime  = attackTime;
		enemyList [currentChoice - 1].isMagic  = isMagicUser;
		if (isMagicUser) {
			enemyList [currentChoice - 1].manaPool = enemyMana;
		} else {
			enemyList [currentChoice - 1].manaPool = 0;
		}
	}


	// build a new enemy asset and save the attributes to it
	private void createEnemy() {

		// make sure there is a sprite and name
		if (enemySprite == null) {
			isntSprited = true;
		} else if (enemyName == "") {
			isntNamed = true;
		} else {

			// make sure an enemy isn't already saved with the same name
			string[] assetString = AssetDatabase.FindAssets (enemyName.Replace(" ","_"));
			if (assetString.Length > 0) {
				alreadyExists = true;
				return;
			}

			// assign the attributes to a new enemy scriptable object
			Enemies myEnemy = ScriptableObject.CreateInstance<Enemies> ();
			myEnemy.mySprite = enemySprite;
			myEnemy.emname   = enemyName;
			myEnemy.health   = enemyHealth;
			myEnemy.atk      = enemyAttack;
			myEnemy.def      = enemyDefense;
			myEnemy.agi      = enemyAgility;
			myEnemy.atkTime  = attackTime;
			myEnemy.isMagic  = isMagicUser;
			if (isMagicUser) {
				myEnemy.manaPool = enemyMana;
			} else {
				myEnemy.manaPool = 0;
			}

			// create a new enemy asset
			AssetDatabase.CreateAsset (myEnemy, "Assets/Resources/Data/EnemyData/" + myEnemy.emname.Replace(" ","_") + ".asset");

			// reset stuff back to starting values
			isntSprited = false;
			isntNamed = false;
			newEnemy ();
			getEnemies ();
			currentChoice = 0;
			GUI.FocusControl ("Top");
		}
	}



}
