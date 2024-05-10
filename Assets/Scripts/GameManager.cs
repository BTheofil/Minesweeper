using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    [SerializeField] private Transform tilePrefab;
    [SerializeField] private Transform gameHolder; //to not spam the game hiearhz

    private int width;
    private int height;
    private int mineCount;

    private List<Tile> tiles = new();

    private readonly float tileSize = 0.5f;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        CreateGameBoard(9, 9, 10);

        ResetGameState();
    }

    public void CreateGameBoard(int width, int height, int mineCount) {
        this.width = width;
        this.height = height;
        this.mineCount = mineCount;

        //create the tiles
        for (int row = 0; row < height; row++) {
            for (int col = 0; col < width; col++) {

                //position and size
                Transform tileTransform = Instantiate(tilePrefab);
                tileTransform.parent = gameHolder;

                float x = col - ((width - 1) / 2.0f);
                float y = row - ((height - 1) / 2.0f);

                tileTransform.localPosition = new Vector2(x * tileSize, y * tileSize);

                Tile tile = tileTransform.GetComponent<Tile>();
                tiles.Add(tile);
            }
        }
    }

    public void ClickHeighbours(Tile tile) {
        int location = tiles.IndexOf(tile);
        foreach (int pos in GetNeighbours(location)) {
            tiles[pos].ClickedTile();
        }
    }

    public void GameOver() {
        foreach (Tile tile in tiles) {
            tile.ShowGameOverState();
        }
    }

    public void CheckGameOver() {
        int count = 0;
        foreach (Tile tile in tiles) {
            if (tile.isActive) {
                count++;
            }
        }
        if (count == mineCount) {
            Debug.Log("Winner");
            foreach (Tile tile in tiles) {
                tile.isActive = false;
                tile.SetFlaggedIfMine();
            }
        }
    
    }

    private void ResetGameState() {
        int[] minePositions = Enumerable.Range(0, tiles.Count).OrderBy(x => Random.Range(0f, 1.0f)).ToArray();

        for (int i = 0; i < mineCount; i++) {
            int pos = minePositions[i];
            tiles[pos].isMine = true;
        }

        for (int i = 0; i < tiles.Count; i++) {
            tiles[i].mineCount = HowManyMines(i);
        }
    }

    private int HowManyMines(int location) {
        int count = 0;
        foreach (int pos in GetNeighbours(location)) {
            if (tiles[pos].isMine) {
                count++;
            }
        }
        return count;
    }

    // Given a position, return the positions of all neighbours.
    private List<int> GetNeighbours(int pos) {
        List<int> neighbours = new();
        int row = pos / width;
        int col = pos % width;
        // (0,0) is bottom left.
        if (row < (height - 1)) {
            neighbours.Add(pos + width); // North
            if (col > 0) {
                neighbours.Add(pos + width - 1); // North-West
            }
            if (col < (width - 1)) {
                neighbours.Add(pos + width + 1); // North-East
            }
        }
        if (col > 0) {
            neighbours.Add(pos - 1); // West
        }
        if (col < (width - 1)) {
            neighbours.Add(pos + 1); // East
        }
        if (row > 0) {
            neighbours.Add(pos - width); // South
            if (col > 0) {
                neighbours.Add(pos - width - 1); // South-West
            }
            if (col < (width - 1)) {
                neighbours.Add(pos - width + 1); // South-East
            }
        }
        return neighbours;
    }
}
