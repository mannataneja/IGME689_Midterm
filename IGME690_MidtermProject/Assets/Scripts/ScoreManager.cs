using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private SnapshotCamera snapCam;
    [SerializeField] private TrailWanderer[] animals;
    [SerializeField] private RealtimeSunController sunController;

    public int score = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var animalList = FindObjectsByType<TrailWanderer>(FindObjectsSortMode.None);
        animals = animalList;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Score()
    {
        var animalList = FindObjectsByType<TrailWanderer>(FindObjectsSortMode.None);
        animals = animalList;
        foreach (TrailWanderer animal in animals)
        {
            if (animal.isVisible)
            {
                if (animal.gameObject.tag == "chicken")
                {
                    snapCam.AddScore(10);
                }
                if (animal.gameObject.tag == "deer")
                {
                    snapCam.AddScore(20);
                }
                if (animal.gameObject.tag == "dog")
                {
                    snapCam.AddScore(5);
                }
                if (animal.gameObject.tag == "horse")
                {
                    snapCam.AddScore(10);
                }
                if (animal.gameObject.tag == "cat")
                {
                    snapCam.AddScore(30);
                }
            }

        }
    }
}
