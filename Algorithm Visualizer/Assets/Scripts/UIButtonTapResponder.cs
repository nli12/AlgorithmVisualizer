using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoloToolkit.Unity.InputModule.Tests
{

    /// <summary>
    /// This class implements IInputClickHandler to handle the tap gesture.
    /// It increases the scale of the object when tapped.
    /// </summary>
    public class UIButtonTapResponder : MonoBehaviour, IInputClickHandler
    {

        public String UIType;
        SortControl control;

        private void Start()
        {
            GameObject go = GameObject.FindWithTag("GameController");
            control = (SortControl)go.GetComponent(typeof(SortControl));
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {

            switch (UIType)
            {
                case "Play":
                    control.PlayScene();
                    break;
                case "Pause":
                    control.PauseScene();
                    break;
                case "Reset":
                    SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
                    break;
                default:
                    break;
            }
        }
    }
}
