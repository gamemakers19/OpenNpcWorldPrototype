using UnityEngine;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour
{
    [Header("Core")]
    public KeyCode InteractButton = KeyCode.E;

    public LayerMask Mask;
    public float InteractionRange;

    public Camera PlayerCamera;

    [Header("Interaction with Bed")]
    public GameObject panel;
    public InputField inputField;

    [Header("Assigned automatically")]
    public GameObject InteractableObject;

    public GameObject QuestUiWindow;
    private bool questWindowActive = false;
    public Quest quest;

    private void Awake()
    {
        SetSleepPanelState(false);
    }

    private void Update()
    {
        CheckDialouge();
        InterationWithObjects();
    }

    public void CheckDialouge()
    {
        if (Input.GetKeyDown(InteractButton))
        {
            RaycastHit hit;
            if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out hit, InteractionRange, Mask))
            {
                DialogueManager dialogue = hit.transform.GetComponentInParent<DialogueManager>();
                if (dialogue == null)
                    dialogue = hit.transform.GetComponentInChildren<DialogueManager>();
                if (dialogue == null)
                    return;
                Vector3 rot = dialogue.transform.eulerAngles;
                dialogue.transform.LookAt(transform);
                dialogue.transform.eulerAngles = new Vector3(rot.x, dialogue.transform.eulerAngles.y, rot.z);
                dialogue.say("Hello there. How are you");
            }
            QuestWindowToggle();
        }
    }

    public void InterationWithObjects()
    {
        RaycastHit hit;
        if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out hit, InteractionRange) && Input.GetKeyDown(InteractButton))
        {
            if (hit.collider.gameObject.GetComponentInChildren<IInteractable>() != null)
            {
                InteractableObject = hit.collider.gameObject;

                IInteractable interactable = hit.collider.gameObject.GetComponentInChildren<IInteractable>();
                interactable.Interact(this);
            }
        }
    }

    public void SetSleepPanelState()
    {
        panel.SetActive(!panel.activeInHierarchy);

        Cursor.visible = panel.activeInHierarchy;
        Cursor.lockState = (panel.activeInHierarchy == false) ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void SetSleepPanelState(bool state)
    {
        panel.SetActive(state);

        Cursor.visible = state;
        Cursor.lockState = (state  == false) ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void ChooseSleep()
    {
        InteractableObject.GetComponentInChildren<Bed>().ChooseSleep(inputField.GetComponentInChildren<AmountField>().Amount, this);
        SetSleepPanelState(false);
    }

    private void QuestWindowToggle()
    {
        if (Input.GetKeyDown(KeyCode.X) && questWindowActive == false)
        {

            QuestUiWindow.SetActive(true);
            questWindowActive = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else if (Input.GetKeyDown(KeyCode.X) && questWindowActive == true)
        {
            QuestUiWindow.SetActive(false);
            questWindowActive = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
