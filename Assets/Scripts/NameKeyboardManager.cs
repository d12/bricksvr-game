using UnityEngine;
using TMPro;

public class NameKeyboardManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public UserSettings userSettings;
    public GameObject mainMenu;
    public GameObject miscMenu;

    private string _enteredName = "";
    private const int MaxNameLength = 15;
    private string _defaultNameLabel;
    private ProfanityFilter.ProfanityFilter _profanityFilter;
    private bool uppercase = true;

    private void Awake()
    {
        _defaultNameLabel = nameText.text;
        _profanityFilter = new ProfanityFilter.ProfanityFilter();
    }

    private void OnEnable()
    {
        _enteredName = userSettings.Nickname;
        UpdateNameOnUI();
    }

    public void KeyPress(string key)
    {
        if (_enteredName.Length >= MaxNameLength)
            return;

        if (_enteredName.Length == 0 && key == " ")
            return;

        _enteredName += uppercase ? key.ToUpper() : key.ToLower();
        UpdateNameOnUI();
    }

    public void BackspacePress()
    {
        if (_enteredName.Length == 0)
            return;

        _enteredName = _enteredName.Remove(_enteredName.Length - 1, 1);
        UpdateNameOnUI();
    }

    public void CapslockPress(GameObject gameObject) {
        TextMeshProUGUI[] components = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        foreach(TextMeshProUGUI text in components) {
            text.text = uppercase ? text.text.ToLower() : text.text.ToUpper();
        }

        uppercase = !uppercase;
    }

    public void SubmitAndReturnToMain()
    {
        if (_enteredName.Length == 0)
            return;

        if (_profanityFilter.ContainsProfanity(_enteredName.ToLower()))
            return;

        userSettings.Nickname = _enteredName;

        gameObject.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void SubmitAndReturnToMisc()
    {
        if (_enteredName.Length == 0)
            return;

        if (_profanityFilter.ContainsProfanity(_enteredName.ToLower()))
            return;

        userSettings.Nickname = _enteredName;

        gameObject.SetActive(false);
        miscMenu.SetActive(true);
    }

    private void UpdateNameOnUI()
    {
        nameText.text = _enteredName.Length == 0 ? _defaultNameLabel : _enteredName;
    }
}
