using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField]
    private GameObject loginPanel;

    [SerializeField]
    private GameObject registrationPanel;

    [SerializeField]
    private GameObject gamePanel;

    [SerializeField]
    private GameObject accountSettingsPanel;

    [Space]
    [SerializeField]
    private GameObject emailVerificationPanel;

    [SerializeField]
    private TMP_Text emailVerificationText;

    [Space]
    [Header("Profile Picture Update Data")]
    public GameObject profileUpdatePanel;
    public Image profileImage;
    public TMP_InputField urlInputFrield;

    private void Awake()
    {
        CreateInstance();
    }

    private void CreateInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ClearUI()
    {
        loginPanel.SetActive(false);
        registrationPanel.SetActive(false);
        emailVerificationPanel.SetActive(false);
        gamePanel.SetActive(false);
        accountSettingsPanel.SetActive(false);
    }

    public void OpenLoginPanel()
    {
        ClearUI();
        loginPanel.SetActive(true);
    }

    public void OpenCloseProfileUpdatePanel()
    {
        profileUpdatePanel.SetActive(!profileUpdatePanel.activeSelf);
    }

    public void OpenRegistrationPanel()
    {
        ClearUI();
        registrationPanel.SetActive(true);
    }

    public void OpenGamePanel()
    {
        ClearUI();
        gamePanel.SetActive(true);
    }

    public void OpenAccountSettings()
    {
        ClearUI();
        accountSettingsPanel.SetActive(true);
    }

    public void ShowVerificationResponse(bool isEmailSent, string emailId, string errorMessage)
    {
        ClearUI();
        emailVerificationPanel.SetActive(true);

        if (isEmailSent)
        {
            emailVerificationText.text = $"Please verify your email address \n Verification email has been sent to {emailId}";
        }
        else
        {
            emailVerificationText.text = $"Couldn't send email : {errorMessage}";
        }
    }

    public void LoadProfileImage(string url)
    {
        StartCoroutine(routine: (LoadProfileImageIE(url)));
    }

    public IEnumerator LoadProfileImageIE(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            Sprite sprite = Sprite.Create(texture, new Rect(x: 0, y: 0, texture.width, texture.height), pivot: new Vector2());
            profileImage.sprite = sprite;
            profileUpdatePanel.SetActive(false);
        }
    }

    public string GetProfileUpdateURL()
    {
        return urlInputFrield.text;
    }
}
