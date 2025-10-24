using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;
using TMPro;


/// <summary>
/// Script must be attached to camera which is child of FPS camera.
/// Camera must be duplicate as it needs to be activated only when snapshot is being taken.
/// </summary>
[RequireComponent(typeof(Camera))]
public class SnapshotCamera : MonoBehaviour
{
    [SerializeField] RawImage lastImageDisplay;
    [SerializeField] Animator shutterAnimator;
    [SerializeField] GameObject[] gallery;
    [SerializeField] Camera snapCam;
    private List<Snapshot> snapshots;
    private int currentDisplayedImg;

    int resWidth = 256;
    int resHeight = 256;

    byte[] imageBytes;
    Texture2D tempImage;

    int currentSnap;

    int[] scores;

    [SerializeField]
    private float maxSubjectDistance = 15;
    [SerializeField]
    private int playingScore = 2;
    [SerializeField]
    private int sunbathingScore = 1;
    private void Awake()
    {
        snapCam = GetComponent<Camera>();
        if(snapCam.targetTexture == null)
        {
            snapCam.targetTexture = new RenderTexture(resWidth, resHeight, 24);
        }
        else
        {
            resWidth = snapCam.targetTexture.width;
            resHeight = snapCam.targetTexture.height;
        }
        snapCam.gameObject.SetActive(false);
        snapshots = new List<Snapshot>();
        currentDisplayedImg = 0;

        scores = new int[gallery.Length];
    }

    public void CallTakeSnapshot(int currentSnap)
    {
        this.currentSnap = currentSnap;
        snapCam.gameObject.SetActive(true);
    }
    private void LateUpdate()
    {
        if (snapCam.gameObject.activeInHierarchy)
        {
            shutterAnimator.SetTrigger("click");
            snapCam.fieldOfView = transform.parent.GetComponent<Camera>().fieldOfView;

            Texture2D snapshot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            snapCam.Render();
            RenderTexture.active = snapCam.targetTexture;
            snapshot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            byte[] bytes = snapshot.EncodeToPNG();
            string filename = SnapshotName();
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log("Snapshot taken " + filename);

            AssetDatabase.Refresh();
            string lastSnapName = filename.Substring(filename.Length - 36, 32);
            lastImageDisplay.texture = Resources.Load<Texture2D>("Snapshots/" + lastSnapName);

            imageBytes = System.IO.File.ReadAllBytes(filename);
            tempImage = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            ImageConversion.LoadImage(tempImage, imageBytes);
            lastImageDisplay.texture = tempImage;

            gallery[currentSnap - 1].GetComponent<RawImage>().texture = tempImage;

            // Set the subjects and the snapshot data of the snapshot
            //List<SnapshotSubject> subjects = CaptureSubjects();

            DateTime currentTime = DateTime.Now;
            Snapshot newSnap = new Snapshot(currentTime, tempImage, filename);
            //Debug.Log(newSnap);
            snapshots.Add(newSnap);




            gallery[currentSnap - 1].GetComponentInChildren<TMP_Text>().text = "Score: " + scores[currentSnap - 1].ToString();

            snapCam.gameObject.SetActive(false);
        }
    }

    private string SnapshotName()
    {
        return string.Format("{0}/snap_{1}x{2}_{3}.png", Application.persistentDataPath, resWidth, resHeight, System.DateTime.Now.ToString("yyy-MM-dd_HH-mm-ss")); //maybe use persistent data path
    }

    private List<SnapshotSubject> CaptureSubjects()
    {
        // Init list of subjects
        List<SnapshotSubject> subjects = new List<SnapshotSubject>();

        // Get all of the AI agents and filter them based on whether they're visible in the camera
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(snapCam);

        // Return the list of subjects
        return subjects;
    }

    public void AddScore(int score)
    {
        this.scores[currentSnap - 1] += score;
        if(this.scores[currentSnap - 1] < 0)
        {
            this.scores[currentSnap - 1] = 0;
        }
    }
    public void ChangeShownSnapshot(int changeAmt)
    {
        if (snapshots.Count == 0) return;
        currentDisplayedImg += changeAmt;
        if (currentDisplayedImg < 0)
            currentDisplayedImg = snapshots.Count - 1;
        else
            currentDisplayedImg %= snapshots.Count;
        lastImageDisplay.texture = snapshots[currentDisplayedImg].Picture;
    }

    public void DeleteSnap(int index)
    {
        Debug.Log("Delete " + index);
        gallery[index - 1].GetComponent<RawImage>().texture = null;
        scores[index - 1] = 0;
        gallery[index - 1].GetComponentInChildren<TMP_Text>().text = "Score: ";

        int i = 0;
        for(i = index - 1; i < gallery.Length - 1; i++)
        {
            gallery[i].GetComponent<RawImage>().texture = gallery[i + 1].GetComponent<RawImage>().texture;
            scores[i] = scores[i + 1];
            gallery[i].GetComponentInChildren<TMP_Text>().text = "Score: " + scores[i].ToString();
        }
        gallery[i].GetComponent<RawImage>().texture = null;
        scores[i] = 0;
        gallery[i].GetComponentInChildren<TMP_Text>().text = "Score: " + scores[i].ToString();
    }
}
