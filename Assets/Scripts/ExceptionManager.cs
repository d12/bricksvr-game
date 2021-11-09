using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExceptionManager : MonoBehaviour
{
    private const int ExceptionsPerCooldown = 5;
    private const float CooldownTime = 5f;

    private int _exceptionsLeftBeforeTimeout = ExceptionsPerCooldown;
    private Coroutine _cooldownCoroutine;

    private void OnEnable()
    {
        Application.logMessageReceivedThreaded += LogReceived;
    }

    private void OnDisable()
    {
        Application.logMessageReceivedThreaded -= LogReceived;
    }

    private void LogReceived(string condition, string stacktrace, LogType type)
    {
        if (_exceptionsLeftBeforeTimeout <= 0) return;
        if (type != LogType.Error && type != LogType.Exception) return;

        StartCoroutine(BrickServerInterface.GetInstance().SendException(condition, stacktrace, type));

        _exceptionsLeftBeforeTimeout -= 1;
        if (_exceptionsLeftBeforeTimeout <= 0 && _cooldownCoroutine == null)
        {
            _cooldownCoroutine = StartCoroutine(EnableReportingCooldown());
        }
    }

    private IEnumerator EnableReportingCooldown()
    {
        yield return new WaitForSeconds(CooldownTime);

        _exceptionsLeftBeforeTimeout = ExceptionsPerCooldown;
        _cooldownCoroutine = null;
    }
}
