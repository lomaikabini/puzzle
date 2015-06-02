using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PuzzleCell : MonoBehaviour, IPointerClickHandler
{
	[HideInInspector]
	public RectTransform rectTransform;
	CellType _typeCell;
	int _posX;
	int _posY;
	bool _isActive = true;

	public enum CellType
	{
		top,
		bottom
	}

	public int PosX {
		get{ return _posX;}
		set{ _posX = value;}
	}

	public int PosY {
		get{ return _posY;}
		set{ _posY = value;}
	}

	public bool IsActive {
		get{ return _isActive;}
		set{ _isActive = value;}
	}
	
	public CellType TypeCell {
		get{ return _typeCell;}
		set {
			_typeCell = value;
			if (value == CellType.top)
				transform.eulerAngles = new Vector3 (0f, 0f, 0f);
			else
				transform.eulerAngles = new Vector3 (0f, 0f, 180f);
		}
	}

	void Start ()
	{
		rectTransform = gameObject.GetComponent<RectTransform> ();
	}

	public void OnPointerClick (PointerEventData eventData)
	{
		if (IsActive && eventData.button == PointerEventData.InputButton.Left) {
			PuzzleGame.instance.OnTargetClick ();
		}
	}
}
