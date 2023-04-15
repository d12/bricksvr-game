using ProfanityFilter;
using UnityEngine;
using TMPro;


public class CreateKeyboardManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public GameObject createMenu;
    public GameObject loadingMenu;
    public LoadingScreen loadingScreen;

    private string _enteredName = "";
    private const int MaxNameLength = 15;
    private string _defaultNameLabel;
    private ProfanityFilter.ProfanityFilter _profanityFilter;

    private const string DefaultName = "My World";
    private bool uppercase = true;

    private void Awake()
    {
        _defaultNameLabel = nameText.text;
        _profanityFilter = new ProfanityFilter.ProfanityFilter();
    }

    private void OnEnable()
    {
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

    public void CapslockPress(GameObject gameObject) {
        TextMeshProUGUI[] components = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        foreach(TextMeshProUGUI text in components) {
            text.text = uppercase ? text.text.ToLower() : text.text.ToUpper();
        }

        uppercase = !uppercase;
    }

    public void BackspacePress()
    {
        if (_enteredName.Length == 0)
            return;

        _enteredName = _enteredName.Remove(_enteredName.Length - 1, 1);
        UpdateNameOnUI();
    }

    public void SubmitAndCreateRoom()
    {
        if (_profanityFilter.ContainsProfanity(_enteredName.ToLower()))
            return;

        createMenu.SetActive(false);
        loadingMenu.SetActive(true);

        loadingScreen.CreateRoom(_enteredName == "" ? DefaultName : _enteredName);
    }

    public void ClearName()
    {
        _enteredName = "";
        UpdateNameOnUI();
    }

    private void UpdateNameOnUI()
    {
        nameText.text = _enteredName.Length == 0 ? _defaultNameLabel : _enteredName;
    }
}