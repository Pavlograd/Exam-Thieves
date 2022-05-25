using System.Collections;
using System.Collections.Generic;
using PlayFab.Networking;
using StaticClassSettingsGame;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Stamina")]
    [SerializeField] private Color normalColor;
    [SerializeField] private Color waitReloadColor;
    [SerializeField] private RectTransform staminaFill;
    [SerializeField] private GameObject staminaGO;
    [Header("CatchedUI")]
    [SerializeField] private GameObject catchedUI;
    [SerializeField] private Button pressA;
    [SerializeField] private RectTransform catchedTimeCD;
    [SerializeField] private RectTransform catchedPlayerCD;
    [Header("CodeNB")]
    [SerializeField] private TMP_Text textCode;
    [Header("LifeNB")]
    [SerializeField] private GameObject lifeGO;
    [SerializeField] private Sprite[] brokenHearth;
    [Header("Position")]
    [SerializeField] private GameObject[] postionList;
    [Header("Run")]
    [SerializeField] private GameObject runGo;
    [Header("Power")]
    [SerializeField] private GameObject powerGO;
    [Header("SpectatorGO")]
    [SerializeField] private GameObject spectatorGo;
    [Header("PauseMenuGO")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private UIPause _UIpause;
    [Header("PauseMenuUI")]
    [SerializeField] private GameObject pasueMenuUI;
    [SerializeField] private GameObject settingsMenuUI;
    [SerializeField] private Button resumeBtn;
    [Header("SettingUI")]
    [SerializeField] private Slider horizontaleSlider;
    [SerializeField] private Slider verticalSlider;
    [SerializeField] private TMP_Text horizontaleTextValue;
    [SerializeField] private TMP_Text verticaleTextValue;
    [SerializeField] private TMP_Text openText;
    [Header("Interaction")]
    [SerializeField] private GameObject prompt;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private GameObject promptImage;
    [Header("Respawn")]
    [SerializeField] private GameObject respawnGo;
    [Header("Timer")]
    [SerializeField] private TMP_Text timerText;

    private PlayerController controller;
    private PlayerCatched _playerCatched;

    private Image powerImage;
    private Image powerImageButton;

    private Image imageStaminaFill;

    [Header("PowersUI")]
    [SerializeField] Image imageFlash;
    [SerializeField] private RectTransform hackingFill;
    [SerializeField] private GameObject hackingGO;
    private Color colorPowerOff = new Color(0.59f, 0.59f, 0.59f, 0.39f);
    
    public void Quit()
    {
        UnityNetworkServer.Instance.StopClient();
    }


    void Start()
    {
        imageStaminaFill = staminaFill.gameObject.GetComponent<Image>();
        _playerCatched = controller.gameObject.GetComponent<PlayerCatched>();
        PauseMenu.isOn = false;
        horizontaleSlider.value = PlayerSettings.gamePadSensitivityX;
        verticalSlider.value = PlayerSettings.gamePadSensitivityY;
        powerImage = powerGO.GetComponent<Image>();
        powerImageButton = powerGO.transform.GetChild(0).GetComponent<Image>();
        if (controller._playerSetup.type == "Jock")
            powerGO.SetActive(false);
    }

    private void Disallow()
    {
        staminaGO.SetActive(false);
        catchedUI.SetActive(false);
        postionList[0].SetActive(false);
        postionList[1].SetActive(false);
        runGo.SetActive(false);
        pauseMenu.SetActive(false);
        powerGO.SetActive(false);
        prompt.SetActive(false);
        respawnGo.SetActive(false);
    }

    void Update()
    {
        if (GameManager.instance.endGame)
        {
            Disallow();    
            return;
        }
        TogglePauseMenu();
        ToggleChangeCode();
        ToggleChangeLife();
        TogglePromptInteraction();
        ToggleTimer();
        if (!controller._player.isDeath)
        {
            ToggleOpenText();
            ToggleStatusPower();
            TogglePositionPlayer();
            ToggleFlash();
            ToggleHacking();
            if (_playerCatched._catchedRun)
            {
                ToggleCatchPlayer();
            }
            else
            {
                catchedUI.SetActive(false);
                SetStaminaAmount(controller.GetCurrentStamina());
            }
        }
        else
        {
            ToggleSpectatorMode();
        }
    }

    private void ToggleTimer()
    {
        float timeValue = GameManager.instance.timeValue;
        if (timeValue <= 180)
        {
            timerText.color = Color.red;
        }
        else
        {
            timerText.color = Color.white;
        }
        if (timeValue < 0)
        {
            timeValue = 0;
        }
        float minutes = Mathf.FloorToInt(timeValue / 60);
        float seconds = Mathf.FloorToInt(timeValue % 60);

        timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

    private void TogglePromptInteraction()
    {
        if (controller.interactableObject != null && !prompt.activeSelf)
        {
            promptText.text = controller.interactableObject.GetComponent<Interactable>().promptMessage;
            prompt.SetActive(true);

            promptImage.SetActive(controller.interactableObject.GetComponent<Interactable>().posPromptImage != Vector3.zero);
            promptImage.transform.localPosition = controller.interactableObject.GetComponent<Interactable>().posPromptImage;
        }
        else if (controller.interactableObject == null && prompt.activeSelf)
        {
            prompt.SetActive(false);
        }
    }

    private void ToggleSpectatorMode()
    {
        powerGO.SetActive(false);
        runGo.SetActive(false);
        ChangePosition(-1);
        spectatorGo.SetActive(true);
        hackingGO.SetActive(false);
        catchedUI.SetActive(false);
    }

    public void SetPowerImage(Sprite image)
    {
        powerGO.GetComponent<Image>().sprite = image;
    }

    private void ToggleStatusPower()
    {
        if (controller._playerCharacter.GetHasPower() && powerImage.color != Color.white)
        {
            powerImage.color = Color.white;
            powerImageButton.color = Color.white;
        }
        else if (!controller._playerCharacter.GetHasPower() && powerImage.color != colorPowerOff)
        {
            powerImage.color = colorPowerOff;
            powerImageButton.color = colorPowerOff;
        }
    }

    public void ToggleFlash()
    {
        if (controller._playerCharacter.flash)
        {
            Debug.Log("FLASH");
            controller._playerCharacter.flash = false;
            StartCoroutine("FlashCoroutine");
        }
    }

    private void TogglePositionPlayer()
    {
        if (controller._playerMotor.getIsCrounching())
        {
            ChangePosition(1);
        }
        else
        {
            ChangePosition(0);
        }
    }

    private void ChangePosition(int position)
    {
        for (int i = 0; i < postionList.Length; i++)
        {
            if (i == position)
            {
                postionList[i].SetActive(true);
            }
            else
            {
                postionList[i].SetActive(false);
            }
        }
    }

    IEnumerator FlashCoroutine()
    {
        imageFlash.color = Color.clear;

        // Fast flash
        while (imageFlash.color.a <= 0.9)
        {
            imageFlash.color = Color.Lerp(imageFlash.color, Color.white, 50 * Time.deltaTime);
            yield return new WaitForSeconds(0.05f);
        }

        // UnFlash
        while (imageFlash.color.a >= 0.1)
        {
            imageFlash.color = Color.Lerp(imageFlash.color, Color.clear, 10 * Time.deltaTime);
            yield return new WaitForSeconds(0.05f);
        }

        imageFlash.color = Color.clear;
        yield return null;
    }

    public void ToggleHacking()
    {
        if (controller._playerCharacter.hackingProgression == 0.0f)
        {
            hackingGO.SetActive(false);
        }
        else
        {
            hackingGO.SetActive(true);
            // Need to divide by the hacking's duration
            hackingFill.localScale = new Vector3(controller._playerCharacter.hackingProgression / 2.0f, 1f, 1f);
        }
    }

    public void ToggleCatchPlayer()
    {
        staminaGO.SetActive(false);
        catchedUI.SetActive(true);
        catchedTimeCD.localScale = new Vector3(controller.GetCurrentCatchedCD() / GameManager.instance.secondToEscape, 1f, 1f);
        catchedPlayerCD.localScale = new Vector3(controller.GetCurrentCatchedPlayerCD() / GameManager.instance.GetNbPressToEscapeDivide(), 1f, 1f);

    }

    public void ToggleSettingsMenu()
    {
        pasueMenuUI.SetActive(!pasueMenuUI.activeSelf);
    }

    public void ToggleChangeCode()
    {
        textCode.text = "Code: " + GameManager.instance.numberOfCode + "/" + GameManager.instance.numberMaxCode;
    }

    private void ToggleBrokenHearth(int i)
    {
        if (GameManager.instance.numberOfLife <= 0) return;
        int numberCatch = GameManager.instance.numberCatch;
        numberCatch = numberCatch >= 4 ? 3 : numberCatch;
        lifeGO.transform.GetChild(i - 1).GetChild(0).gameObject.GetComponent<Image>().sprite = brokenHearth[numberCatch];

    }

    private void ToggleChangeLife()
    {
        int i = GameManager.instance.numberOfLife;
        if (i != 0)
            ToggleBrokenHearth(i);
        foreach (Transform child in lifeGO.transform)
        {
            if (i <= 0)
            {
                child.GetChild(0).gameObject.SetActive(false);
            }
            i--;
        }
        return;
        //textLife.text = "Life: " + GameManager.instance.numberOfLife + "/" + GameManager.instance.numberMaxLife;
    }

    void ToggleOpenText()
    {
        if (controller.interactableObject != null)
        {
            openText.text = "Open " + controller.interactableObject.name;
        }
        else
        {
            openText.text = "";
        }
    }

    public void TogglePauseMenu(bool force = false)
    {
        if (PauseMenu.isOn != controller.pause || force)
        {
            if (force) controller.pause = false;

            if (!pauseMenu.activeSelf)
            {
                _UIpause.Activate();
                //pasueMenuUI.SetActive(true);
                //settingsMenuUI.SetActive(false);
                //resumeBtn.Select();
            }
            else
            {
                _UIpause.Desactivate();
            }

            PauseMenu.isOn = pauseMenu.activeSelf;
        }
    }

    public void ToggleHorizontaleSensibility()
    {
        horizontaleTextValue.text = horizontaleSlider.value.ToString();
        //controller.MouseSensitivityX = horizontaleSlider.value;
        PlayerSettings.gamePadSensitivityX = horizontaleSlider.value;
    }

    public void ToggleVerticaleSensibility()
    {
        verticaleTextValue.text = verticalSlider.value.ToString();
        //controller.MouseSensitivityY = verticalSlider.value;
        PlayerSettings.gamePadSensitivityY = verticalSlider.value;
    }

    public void SetPlayerController(PlayerController _controller)
    {
        controller = _controller;
    }

    void SetStaminaAmount(float _amount)
    {
        staminaFill.localScale = new Vector3(_amount, 1f, 1f);
        displayStamina(_amount);
        changeColorStamina();
    }

    private void changeColorStamina()
    {
        if (controller.GetwWaitStaminaReload())
            imageStaminaFill.color = waitReloadColor;
        else
            imageStaminaFill.color = normalColor;
    }

    private void displayStamina(float _amount)
    {
        if (_amount >= 1f)
        {
            staminaGO.SetActive(false);
        }
        else
        {
            staminaGO.SetActive(true);
        }
    }

    public void CatchedPlayerPressA()
    {
        pressA.Select();
        pressA.onClick.Invoke();

    }

    public void PrintRespawn()
    {
        respawnGo.SetActive(true);
        StartCoroutine(DissalowRespawn());
    }

    IEnumerator DissalowRespawn()
    {
        yield return new WaitForSeconds(2f);
        respawnGo.SetActive(false);
    }
}
