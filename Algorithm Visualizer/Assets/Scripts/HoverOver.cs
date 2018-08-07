// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using DG.Tweening;

namespace HoloToolkit.Unity.InputModule.Tests
{
    /// <summary>
    /// This class implements IFocusable to respond to gaze changes.
    /// </summary>
    public class HoverOver : MonoBehaviour, IFocusable
    {

        SortControl control;
        Transform cube;

        private void Start()
        {
            GameObject go = GameObject.FindWithTag("GameController");
            control = (SortControl)go.GetComponent(typeof(SortControl));
            cube = this.gameObject.transform;
        }

        public void OnFocusEnter()
        {
            if (control.interactive)
            {
                control.HoverUp(cube);
            }
            
        }

        public void OnFocusExit()
        {
            if (control.interactive)
            {
                control.HoverDown(cube);
            }
        }

    }
}
