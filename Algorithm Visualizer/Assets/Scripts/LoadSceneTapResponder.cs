// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoloToolkit.Unity.InputModule.Tests
{

    /// <summary>
    /// This class implements IInputClickHandler to handle the tap gesture.
    /// It increases the scale of the object when tapped.
    /// </summary>
    public class LoadSceneTapResponder : MonoBehaviour, IInputClickHandler
    {

        public String toLoad;

        public void OnInputClicked(InputClickedEventData eventData)
        {
            SceneManager.LoadScene(toLoad);
        }
    }
}