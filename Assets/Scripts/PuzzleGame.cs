using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

public class PuzzleGame : MonoBehaviour
{

	public static PuzzleGame instance;
	public GameObject cellPrefab;
	public GameObject cellBlockPrefab;
	public RectTransform canvas;
	public RectTransform cellBlockContainer;
	public RectTransform targetContainer;
	public float cellSize;
	List<PuzzleCell> cells = new List<PuzzleCell> ();
	Color32 cellColor = new Color32 (232,102,82,255);
	RectTransform currentBlock;

	void Awake ()
	{
		instance = this;
	}

	void Start ()
	{
		parseLevelFile ();
	}

	void Update ()
	{
		if (currentBlock != null) {
			Vector3 p = Camera.main.ScreenToViewportPoint (Input.mousePosition);
			p.x *= canvas.rect.width;
			p.y *= canvas.rect.height;
			currentBlock.anchoredPosition = p;
		}
		if (currentBlock != null && Input.GetMouseButtonDown (1)) {
			currentBlock.GetComponent<PuzzleCellBlock> ().RotateBlock ();
		}
	}

	void parseLevelFile ()
	{
		StreamReader reader = File.OpenText (Application.dataPath + "/Data/cat.txt");
		string line;
		float xCof = 2f;
		float yCof = 1.23f;
		float posX = 0;
		float posY = 0;
		float xOffset = 0f;
		float yOffset = 0f;
		float blockOffset = 0f;
		int i = 0;
		int emptyStrings = 0;
		bool isTarget = true;
		PuzzleCellBlock currentBlock = null;
		PuzzleCell.CellType preCellType = PuzzleCell.CellType.bottom;
		while ((line = reader.ReadLine()) != null) {

			char[] chars = line.ToCharArray ();
			if (chars.Length == 0) {
				if (isTarget && chars.Length == 0)
					yOffset = i * cellSize / 2 / yCof;
				emptyStrings ++;
				if (emptyStrings == 2 && isTarget) {
					emptyStrings = 1;
					isTarget = false;
				}
				continue;
			}

			if (i == 0) {
				xOffset = chars.Length * cellSize / 2 / xCof;
			}

			if (isTarget) {
				for (int j = 0; j < chars.Length; j++) {
					
					if (preCellType == PuzzleCell.CellType.top)
						preCellType = PuzzleCell.CellType.bottom;
					else
						preCellType = PuzzleCell.CellType.top;

					if (chars [j].Equals ('1')) {
						GameObject obj = Instantiate (cellPrefab, Vector3.zero, Quaternion.identity) as GameObject;
						obj.transform.SetParent (targetContainer);
						obj.transform.localScale = Vector3.one;
						obj.transform.localPosition = new Vector3 (posX, posY, 0f);
						obj.GetComponent<RectTransform> ().sizeDelta = new Vector2 (cellSize, cellSize);
						PuzzleCell pc = obj.GetComponent<PuzzleCell> ();
						pc.PosX = j;
						pc.PosY = i;
						cells.Add (pc);
						pc.TypeCell = preCellType;
					}
					posX += cellSize / xCof;
				}
				
				if (preCellType == PuzzleCell.CellType.top)
					preCellType = PuzzleCell.CellType.bottom;
				else
					preCellType = PuzzleCell.CellType.top;

				posX = 0f;
				posY -= cellSize / yCof;
				i++;
			} else {

				int strIndx = 0;
				if (emptyStrings == 1) {
					GameObject obj = Instantiate (cellBlockPrefab, Vector3.zero, Quaternion.identity) as GameObject;
					obj.transform.SetParent (cellBlockContainer);
					obj.transform.localScale = Vector3.one;
					obj.transform.localPosition = Vector3.zero;
					currentBlock = obj.GetComponent<PuzzleCellBlock> ();
					emptyStrings = 0;
					preCellType = PuzzleCell.CellType.top;
					blockOffset = 0f;
				} else {
					preCellType = PuzzleCell.CellType.bottom;
					strIndx = 1;
				}

				float pX = 0f;
				float pY = -strIndx * cellSize / yCof;
				bool increaseOffset = true;
				for (int j = 0; j < chars.Length; j++) {
					if (increaseOffset && strIndx == 0 && chars [j].Equals ('0')) {
						blockOffset -= cellSize;
						increaseOffset = false;
					} else
						increaseOffset = false;
					
					if (chars [j].Equals ('1')) {
						GameObject obj = Instantiate (cellPrefab, Vector3.zero, Quaternion.identity) as GameObject;
						obj.transform.SetParent (currentBlock.transform);
						obj.transform.localScale = Vector3.one;
						obj.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (pX + blockOffset, pY);
						obj.GetComponent<RectTransform> ().sizeDelta = new Vector2 (cellSize, cellSize);
						obj.GetComponent<Image> ().color = cellColor;
						obj.name = strIndx.ToString () + j.ToString ();
						PuzzleCell pc = obj.GetComponent<PuzzleCell> ();
						pc.TypeCell = preCellType;
						currentBlock.cells.Add (new PuzzleCellBlock.PuzzleCellBlockType (preCellType, obj.transform));
						pc.enabled = false;
					}
					pX += cellSize / xCof;
					if (preCellType == PuzzleCell.CellType.top)
						preCellType = PuzzleCell.CellType.bottom;
					else
						preCellType = PuzzleCell.CellType.top;
				}
			}
		}
		Vector3 newPos = targetContainer.localPosition;
		newPos -= new Vector3 (xOffset, -yOffset, 0f);
		targetContainer.localPosition = newPos;
	}

	public void OnItmClick (PuzzleCellBlock objBlock)
	{
		if (currentBlock != null)
			Destroy (currentBlock.gameObject);

		GameObject obj = Instantiate (objBlock.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
		obj.transform.SetParent (transform);
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localScale = Vector3.one;
		PuzzleCellBlock pzCellBlock = obj.GetComponent<PuzzleCellBlock> ();
		foreach (Transform child in obj.transform) {
			PuzzleCell puzzleCell = child.gameObject.GetComponent<PuzzleCell> ();
			PuzzleCell.CellType cellType = objBlock.cells.Find (k => k.transform.name == child.name).type;
			pzCellBlock.cells.Add (new PuzzleCellBlock.PuzzleCellBlockType (cellType, child));
		}
		CanvasGroup canvasGroup = obj.AddComponent<CanvasGroup> ();
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		currentBlock = obj.GetComponent<RectTransform> ();
	}

	public void OnTargetClick ()
	{
		if (currentBlock == null)
			return;
		PuzzleCellBlock block = currentBlock.GetComponent<PuzzleCellBlock> ();
		List<PuzzleCell> cellsOnTarget = new List<PuzzleCell> ();
		foreach (PuzzleCellBlock.PuzzleCellBlockType bCell in block.cells) {
			float dist = Mathf.Infinity;
			PuzzleCell findedCell = null;
			foreach (PuzzleCell puzzleCell in cells) {
				if (cellsOnTarget.Contains (puzzleCell))
					continue;
				float val = Vector2.Distance (bCell.transform.position, puzzleCell.rectTransform.position);
				if (val < dist) {
					dist = val;
					findedCell = puzzleCell;
				}
			}

			if (findedCell != null && findedCell.TypeCell == bCell.type && findedCell.IsActive) {
				cellsOnTarget.Add (findedCell);
			} else {
				Debug.Log ("Inappropriate block");
				return;
			}
		}

		for (int i = 0; i < cellsOnTarget.Count; i++) {
			cellsOnTarget [i].GetComponent<Image> ().color = cellColor;
			cellsOnTarget [i].IsActive = false;
		}
		Destroy (currentBlock.gameObject);
	}
}
		