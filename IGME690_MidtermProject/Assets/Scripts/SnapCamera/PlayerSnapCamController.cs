using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class PlayerSnapCamController : MonoBehaviour
{
    [SerializeField] SnapshotCamera snapCam;
    [SerializeField] float cooldownTime = 3;
    [SerializeField] Slider cooldownSlider;
    [SerializeField] TMP_Text snapCounter;
    [SerializeField] Canvas gallery;
    [SerializeField] Canvas pauseMenu;
    float lastClick;
    float nextClick;

    int maxSnaps = 10;
    int currentSnap = 0;
    // Update is called once per frame
    private void Start()
    {
        lastClick = Time.time - cooldownTime;
        cooldownSlider.maxValue = cooldownTime;
        snapCounter.text = "Photos Taken : " + currentSnap + "/" + maxSnaps;

        gallery.enabled = false;

        ResumGame();
        //gallery.GetComponentInChildren<RectTransform>().localScale = new Vector3(0, 0, 0);
    }
    void Update()
    {
        if (currentSnap < maxSnaps && Input.GetKeyDown(KeyCode.Mouse1) && Time.time > nextClick && !gallery.enabled && !pauseMenu.enabled)
        {
            Debug.Log("SNAP!");
            lastClick = Time.time;
            nextClick = Time.time + cooldownTime;

            currentSnap++;
            snapCounter.text = "Photos Taken : " + currentSnap + "/" + maxSnaps;

            snapCam.CallTakeSnapshot(currentSnap);
        }

        ///<summary>
        ///cycle through images taken 
        /// </summary>
        /// 
/*        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            snapCam.ChangeShownSnapshot(-1);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            snapCam.ChangeShownSnapshot(1);*/

        else if (Input.GetKeyDown(KeyCode.G))
        {
            if (!gallery.enabled)
            {
                DisplayGallery();
            }
            else
            {
                HideGallery();
            }
        }
            
        cooldownSlider.value = Time.time - lastClick;

        if(currentSnap >= maxSnaps)
        {
            snapCounter.color = Color.red;
        }
        else
        {
            snapCounter.color = Color.white;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseMenu.enabled)
            {
                PauseGame();
            }
            else
            {
                ResumGame();
            }
        }
    }
    public void DeleteSnap(int index)
    {
        snapCam.DeleteSnap(index);
        currentSnap--;
        snapCounter.text = "Photos Taken : " + currentSnap + "/" + maxSnaps;
    }
    public void DisplayGallery()
    {
        gallery.enabled = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void HideGallery()
    {
        gallery.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void PauseGame()
    {
        pauseMenu.enabled = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
    }
    public void ResumGame()
    {
        pauseMenu.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
