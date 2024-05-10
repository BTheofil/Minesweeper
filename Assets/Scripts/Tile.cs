using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour {

    [Header("The Sprites")]
    [SerializeField] private Sprite unClickedTile;
    [SerializeField] private Sprite flaggedTile;
    [SerializeField] private List<Sprite> clickedTiles;
    [SerializeField] private Sprite mineTile;
    [SerializeField] private Sprite mineWrongTile;
    [SerializeField] private Sprite mineHitTile;

    private SpriteRenderer spriteRenderer;

    public bool isFlagged = false;
    public bool isActive = true;
    public bool isMine = false;
    public int mineCount = 0;

    public GameManager gameManager;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseOver() {
        if (isActive) {
            if (Input.GetMouseButton(0)) {
                //left click to reveal tile
                ClickedTile();
            } else if (Input.GetMouseButton(1)) {
                //toggle the flag
                isFlagged = !isFlagged;

                if (isFlagged) {
                    spriteRenderer.sprite = flaggedTile;
                } else {
                    spriteRenderer.sprite = unClickedTile;
                }
            }
        }
    }

    public void ClickedTile() {
        //dont allow left clicks on flags
        if (isActive && !isFlagged) {
            isActive = false;

            if (isMine) {
                //game over
                spriteRenderer.sprite = mineHitTile;
                GameManager.Instance.GameOver();
            } else {
                spriteRenderer.sprite = clickedTiles[mineCount];

                if (mineCount == 0) {
                    GameManager.Instance.ClickHeighbours(this);
                }

                GameManager.Instance.CheckGameOver();
            }
        }
    }

    public void ShowGameOverState() {
        if (isActive) {
            isActive = false;
            if (isMine & !isFlagged) {
                // If mine and not flagged show mine.
                spriteRenderer.sprite = mineTile;
            } else if (isFlagged && !isMine) {
                // If flagged incorrectly show crossthrough mine
                spriteRenderer.sprite = mineWrongTile;
            }
        }
    }

    public void SetFlaggedIfMine() {
        if (isMine) {
            isFlagged = true;
            spriteRenderer.sprite = flaggedTile;
        }
    }
}
