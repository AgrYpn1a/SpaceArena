using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] player;
    [SerializeField]
    private GameObject camera;

    private GameObject currentPlayer;

    private static GameController instance;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            return;

        // for testing purposes
        currentPlayer = player[0];
        SpawnPlayer(Vector2.zero);
    }

    public static GameController Instance()
    {
        return instance;
    }

    public void SpawnPlayer(Vector2 position)
    {
        GameObject _player = Instantiate(currentPlayer, position, Quaternion.identity);
        GameObject _camera = Instantiate(camera, new Vector3(position.x, position.y, -10), Quaternion.identity);

        _player.GetComponent<PlayerController>().SetCamera(_camera);
    }
}
