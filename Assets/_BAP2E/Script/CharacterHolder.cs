using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterHolder : MonoBehaviour, IListener
{
	public static CharacterHolder Instance;
	[HideInInspector]
	public GameObject CharacterPicked;

	public List<GameObject> Characters;

	//	List<GameObject> listChaTemp;

	public List<int> CharacterUnlocked { get; set; }
	int originalPlayerID = int.MaxValue;    //mean if Player eat item can switch player, then eat another item switch player, 
											//keep the original player ID, when out of time use another player -> Back to the original player

	void Awake()
	{
		if (CharacterHolder.Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);

		GetPickedCharacter();

		UpdateUnlockCharacter();
	}

	void Update()
	{
		GetPickedCharacter();
	}

	public void GetPickedCharacter()
	{
		CharacterPicked = Characters[0];    //default character is 0
		var characterIDChoosen = PlayerPrefs.GetInt(GlobalValue.ChoosenCharacterInstanceID, 0);
		foreach (var character in Characters)
		{
			var ID = character.GetInstanceID();
			if (ID == characterIDChoosen)
			{
				CharacterPicked = character;
				return;
			}
		}
	}

	public void UpdateUnlockCharacter()
	{
		CharacterUnlocked = new List<int>();

		for (int i = 0; i < Characters.Count; i++)
		{
			if (GlobalValue.CheckUnlockCharacter(i + 1) || (i == 0))
				CharacterUnlocked.Add(Characters[i].GetInstanceID());
		}

		Debug.Log("Totol Player Available: " + CharacterUnlocked.Count);
	}

	public bool isThisCharIdUnlock(int id)
	{
		for (int i = 0; i < CharacterUnlocked.Count; i++)
		{
			if (CharacterUnlocked[i] == Characters[id].GetInstanceID())
			{
				return true;
			}
		}
		return false;
	}

	#region IListener implementation

	public void IPlay()
	{

	}

	public void ISuccess()
	{
	}

	public void IPause()
	{

	}

	public void IUnPause()
	{

	}

	public void IGameOver()
	{
	}

	public void IOnRespawn()
	{

	}

	public void IOnStopMovingOn()
	{

	}

	public void IOnStopMovingOff()
	{

	}

	#endregion
}