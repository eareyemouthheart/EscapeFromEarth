using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultValueKeyboard : MonoBehaviour {
	public static DefaultValueKeyboard Instance;

	[Header("Keyboard")]
	public KeyCode Left = KeyCode.LeftArrow;
	public KeyCode Right = KeyCode.RightArrow;
	public KeyCode Up = KeyCode.UpArrow;
	public KeyCode Down = KeyCode.DownArrow;
	public KeyCode Jump = KeyCode.Space;
	public KeyCode Shooting = KeyCode.K;
    public KeyCode Melee = KeyCode.L;
    public KeyCode Throw = KeyCode.Semicolon;
	public KeyCode Jetpack = KeyCode.N;
	public KeyCode Run;
	public KeyCode Slide = KeyCode.M;
	public KeyCode Dogge = KeyCode.Quote;
	public KeyCode PushandPull = KeyCode.Comma;
	public KeyCode ElevatorUp;
	public KeyCode ElevatorDown;
	public KeyCode Pause = KeyCode.Escape;
    public KeyCode ShieldItem;
    public KeyCode Action;

    void Awake(){
		Instance = this;
	}
}
