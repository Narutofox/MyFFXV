using Gamekit3D;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLevel : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] EmemiesToDestroy;
    private bool startDescend = false;
    private ParticleSystem _particleSystem;
    void Start()
    {
        _particleSystem = gameObject.GetComponent<ParticleSystem>();
        _particleSystem.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        if (AllEnemiesDead())
        {
            LevelDone();
        }

        if (startDescend && transform.position.y > 2f)
        {
            transform.Translate(Vector3.down * Time.deltaTime, Space.World);          
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            TransitionPoint transitionPoint = new TransitionPoint
            {
                newSceneName = "MainMenu",
                requiresInventoryCheck = false
            };

            SceneController.GameFinished = true;
            SceneController.TransitionToScene(transitionPoint);
        }
    }

    private bool AllEnemiesDead()
    {
        foreach (var enemy in EmemiesToDestroy)
        {
            if (enemy != null)
            {
                return false;
            }
        }

        return true;
    }

    public void LevelDone()
    {
        _particleSystem.Play(true);
        startDescend = true;
    }
}
