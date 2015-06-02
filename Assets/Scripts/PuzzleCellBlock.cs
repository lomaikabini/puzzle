using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class PuzzleCellBlock : MonoBehaviour, IPointerClickHandler
{

	public List<PuzzleCellBlockType> cells = new List<PuzzleCellBlockType> ();

	public struct PuzzleCellBlockType
	{
		public PuzzleCell.CellType type;
		public Transform transform;

		public PuzzleCellBlockType (PuzzleCell.CellType t, Transform tr)
		{
			type = t;
			transform = tr;
		}
	}

	public void RotateBlock ()
	{
		Vector3 rot = transform.eulerAngles;
		rot.z += 60f;
		transform.eulerAngles = rot;

		for (int i = 0; i < cells.Count; i++) {
			PuzzleCell.CellType t;
			if (cells [i].type == PuzzleCell.CellType.top)
				t = PuzzleCell.CellType.bottom;
			else
				t = PuzzleCell.CellType.top;
			cells [i] = new PuzzleCellBlockType (t, cells [i].transform);
		}
	}

	public void OnPointerClick (PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
			PuzzleGame.instance.OnItmClick (this);
	}
}
