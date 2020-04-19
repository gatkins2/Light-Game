using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    public GameObject mousePopup;
    public GameObject arrowsPopup;

    public static bool arrowsPopupShown;

    const float mousePopupTimer = 3f;
    float startTime;
    bool timerRunning;

    // Start is called before the first frame update
    void Start()
    {
        // Start timer for mouse popup
        startTime = Time.time;
        timerRunning = true;
        arrowsPopupShown = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Turn timer off on mouse click
        if (mousePopup != null && Input.GetKeyDown(mousePopup.GetComponent<InstructionsPopUp>().completeKey))
            timerRunning = false;

        // Timer for mouse popup
        if (timerRunning && Time.time - startTime > mousePopupTimer)
        {
            mousePopup.GetComponent<InstructionsPopUp>().PopUp();
            timerRunning = false;
        }

        // Destroy after all popups have been shown
        if (!timerRunning && arrowsPopupShown)
            Destroy(this);
    }

    /// <summary>
    /// Show arrows popup next to prism
    /// </summary>
    /// <param name="prism"></param>
    public void ArrowsPopup(GameObject prism)
    {
        GameObject room = Camera.main.GetComponent<RoomController>().GetObjectRoom(prism);
        Vector3 offset = new Vector3(0, -room.GetComponent<BoxCollider2D>().size.y * 0.3f);
        arrowsPopup.transform.position = room.transform.position + offset;

        arrowsPopup.transform.GetChild(0).GetComponent<InstructionsPopUp>().PopUp();
        arrowsPopupShown = true;
    }
}
