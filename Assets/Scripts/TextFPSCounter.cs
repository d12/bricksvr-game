using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TextFPSCounter : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    private Dictionary<int, string> _fpsToUIStringMap;

    private List<int> _last10Times;
    void Start()
    {
        _last10Times = new List<int>();
        _fpsToUIStringMap = new Dictionary<int, string>();
    }

    void Update()
    {
        int currentFPS = Mathf.RoundToInt(1.0f / Time.deltaTime);
        _last10Times.Add(currentFPS);

        if (_last10Times.Count > 5)
        {
            _last10Times.RemoveAt(0);
        }

        if (Time.frameCount % 20 == 0)
        {
            int avgFPS = _last10Times.Sum() / _last10Times.Count;

            if (!_fpsToUIStringMap.ContainsKey(avgFPS))
            {
                _fpsToUIStringMap[avgFPS] = "FPS: " + avgFPS;
            }

            textDisplay.text = _fpsToUIStringMap[avgFPS];
        }
    }
}
