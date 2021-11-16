using UnityEngine;
using TMPro;

public enum TutorialState
{
    Start,
    Movement,
    ZoomFunc,
    NeedAttack,
    NeedParry,
    NeedCharge,
    NeedPickup,
    NeedInventory,
    NeedWeaponSwitch,
    NeedThrow,
    NeedFood,
    NeedKill,
    Finished
}

public class TutorialScript : MonoBehaviour
{
    private TutorialState state = TutorialState.Start;

    private float delay = 4.0f;
    private float wait = 0f;
    private KeyCode continueButton = KeyCode.E;

    public GameObject tutorialSword;
    public GameObject tutorialShuriken;
    public GameObject tutorialFood;

    public GameObject[] wallsToMove;
    public GameObject lightToTurnOn;

    private TextMeshProUGUI displayedText;
    private string helpText = "Welcome to Japan-gea!\nLet's go through the basics now!\nPress 'E' to move to next tutorial.";

    // Start is called before the first frame update
    private void Start()
    {
        LevelEvents.levelEvents.onEnemyDeathTriggerEnter += OnEnemyKilled;
        displayedText = transform.GetChild(0).Find("HelpText").GetComponent<TextMeshProUGUI>();
        displayedText.text = helpText;
    }

    // Update is called once per frame
    private void Update()
    {
        if (state == TutorialState.Start)
        {
            if (Input.GetKeyDown(continueButton))
            {
                state = TutorialState.Movement;
                helpText = GetHelpText(state);
                UpdateText(helpText);
            }
        }
        else if (state == TutorialState.Movement)
        {
            if (Input.GetKeyDown(continueButton))
            {
                state = TutorialState.ZoomFunc;
                helpText = GetHelpText(state);
                UpdateText(helpText);
            }
        }
        else if (state == TutorialState.ZoomFunc)
        {
            if (Input.GetKeyDown(continueButton))
            {
                state = TutorialState.NeedAttack;
                helpText = GetHelpText(state);
                UpdateText(helpText);
            }
        }
        else if (state == TutorialState.NeedAttack)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                state = TutorialState.NeedParry;
                helpText = GetHelpText(state);
                UpdateText(helpText);
            }
        }
        else if (state == TutorialState.NeedParry)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                state = TutorialState.NeedCharge;
                helpText = GetHelpText(state);
                UpdateText(helpText);
            }
        }
        else if (state == TutorialState.NeedCharge)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                state = TutorialState.NeedPickup;
                helpText = GetHelpText(state);
                UpdateText(helpText);
                tutorialSword.SetActive(true);
                tutorialShuriken.SetActive(true);
                tutorialFood.SetActive(true);
            }
        }
        else if (state == TutorialState.NeedPickup)
        {
            if (InventoryController.inventoryController.onHand != null
                && InventoryController.inventoryController.projectile != null
                && InventoryController.inventoryController.consumable != null)
            {
                state = TutorialState.NeedThrow;
                helpText = GetHelpText(state);
                UpdateText(helpText);
            }
        }
        else if (state == TutorialState.NeedThrow)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                PlayerStatHUD.psh.UpdateCurrentHealth(-10);
                state = TutorialState.NeedFood;
                helpText = GetHelpText(state);
                UpdateText(helpText);
            }
        }
        else if (state == TutorialState.NeedFood)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                state = TutorialState.NeedInventory;
                helpText = GetHelpText(state);
                UpdateText(helpText);
            }
        }
        else if (state == TutorialState.NeedInventory)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                state = TutorialState.NeedWeaponSwitch;
                helpText = GetHelpText(state);
                UpdateText(helpText);
            }
        }
        else if (state == TutorialState.NeedWeaponSwitch)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                state = TutorialState.NeedKill;
                helpText = GetHelpText(state);
                UpdateText(helpText);
                OpenRoom();
            }
        }
        else if (state == TutorialState.NeedKill)
        {
            //Wait for enemy death event
        }
        else if (state == TutorialState.Finished)
        {
            wait += Time.deltaTime;
            if (wait >= delay - 2)
            {
                helpText = "Proceed through the door.\nUse the boat to play.";
                UpdateText(helpText);
            }
        }
    }

    private string GetHelpText(TutorialState state)
    {
        string retval;
        if (state == TutorialState.Movement)
        {
            retval = "Use WASD to move your character.\nPress 'E' to continue.";
        }
        else if (state == TutorialState.ZoomFunc)
        {
            retval = "Use mouse scroll to change camera zoom level.\nClick the mouse scroll to go back to default zoom level.\nPress 'E' to continue.";
        }
        else if (state == TutorialState.NeedAttack)
        {
            retval = "Use left click to attack.";
        }
        else if (state == TutorialState.NeedParry)
        {
            retval = "Use right click to block attacks and\nparry projectiles.";
        }
        else if (state == TutorialState.NeedCharge)
        {
            retval = "Use space to dash.";
        }
        else if (state == TutorialState.NeedPickup)
        {
            lightToTurnOn.SetActive(true);
            retval = "Hover your mouse over the item and click 'E' to\npick up the sword, shuriken and food.";
        }
        else if (state == TutorialState.NeedInventory)
        {
            retval = "Press 'Tab' to view your inventory";
        }
        else if (state == TutorialState.NeedWeaponSwitch)
        {
            retval = "Press 'Q' to switch between weapons";
        }
        else if (state == TutorialState.NeedThrow)
        {
            retval = "Press 'F' to throw shuriken";
        }
        else if (state == TutorialState.NeedFood)
        {
            retval = "Press 'C' to use consumable";
        }
        else if (state == TutorialState.NeedKill)
        {
            retval = "Advance to the next room and kill the enemy.";
        }
        else
        {
            retval = "Congratulations on completing the tutorial.";
        }
        return retval;
    }

    private void OpenRoom()
    {
        if (wallsToMove.Length != 3)
        {
            Debug.LogWarning("Wrong wall assignment");
        }
        foreach (GameObject _wall in wallsToMove)
        {
            _wall.GetComponent<MoveWallScript>().MoveWall();
        }
    }

    private void OnEnemyKilled(Vector3 pos)
    {
        state = TutorialState.Finished;
        helpText = GetHelpText(state);
        UpdateText(helpText);
    }

    private void UpdateText(string newText)
    {
        displayedText.text = newText;
    }
}