// https://stackoverflow.com/questions/67704820/how-do-i-print-unitys-debug-log-to-the-screen-gui

using System.Collections;
using UnityEngine;

namespace Tools.Logger
{
    //TODO: Move out shipping build
    public class GuiLogger : MonoBehaviour
    {
        private const int Size = 16;
        private const int ClearDelay = 5;
        private readonly Queue _logQueue = new();
        private Coroutine _coroutineClear;

        private IEnumerator Start()
        {
            yield return _coroutineClear = StartCoroutine(DequeueLog());
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(Screen.width - 400, 0, 400, Screen.height));
            GUILayout.Label("\n" + string.Join("\n", _logQueue.ToArray()));
            GUILayout.EndArea();
        }

        private IEnumerator DequeueLog()
        {
            yield return new WaitForSeconds(ClearDelay);
            if (_logQueue.Count > 0)
            {
                _logQueue.Dequeue();
            }

            _coroutineClear = StartCoroutine(DequeueLog());
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            //TODO: move it to string builder
            const string be = "[";
            const string bc = "] : ";
            _logQueue.Enqueue(be + type + bc + logString);
            if (type == LogType.Exception)
            {
                _logQueue.Enqueue(stackTrace);
            }

            while (_logQueue.Count > Size)
            {
                _logQueue.Dequeue();
            }
        }
    }
}