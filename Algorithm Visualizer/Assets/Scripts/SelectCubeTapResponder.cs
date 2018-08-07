// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoloToolkit.Unity.InputModule.Tests
{

    /// <summary>
    /// This class implements IInputClickHandler to handle the tap gesture.
    /// </summary>
    public class SelectCubeTapResponder : MonoBehaviour, IInputClickHandler
    {

        SortControl control;
        Transform cube;

        private float timeBetweenClicks= 0.3f;  
        private float timestamp = 0f;

        private void Start()
        {
            GameObject go = GameObject.FindWithTag("GameController");
            control = (SortControl)go.GetComponent(typeof(SortControl));
            cube = this.gameObject.transform;
        }
        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (control.interactive && Time.time >= timestamp)
            {
                cube = this.gameObject.transform;
                control.selectCube(cube.GetComponent<Data>().index);
                timestamp = Time.time + timeBetweenClicks;
            }
        }
    }
}
