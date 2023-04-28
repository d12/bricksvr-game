using System.Collections;
using UnityEngine;
using TMPro;

public class LocalAutoSave : MonoBehaviour
{
    
    private UserSettings settings;
    private SessionManager manager;
    private Coroutine routine;
    private int wait = 10;

    public TextMeshProUGUI text;
    
    private void Start()
    {
        settings = UserSettings.GetInstance();

        manager = SessionManager.GetInstance();
        manager.session.didSessionStart += SessionDidStart;
        manager.session.didSessionEnd += SessionDidStart;
    }

    private void SessionDidStart(Session session) {
        if (session.isTutorial) return;

        routine = StartCoroutine(AutoSave());
        settings.AutosaveUpdated.AddListener((int number) => {
            wait = number * 60;
            text.text = $"{number} {(number == 1 ? "Minute": "Minutes")}";
        });
    }

    private void SessionDidEnd(Session _) {
        StopCoroutine(routine);
    }

    private IEnumerator AutoSave() {
        while(settings.AutoSave != 0) {
            LocalSessionLoader.SaveRoom(manager.session.saveDirectory);
            yield return new WaitForSeconds(wait);
        }
    }
}
