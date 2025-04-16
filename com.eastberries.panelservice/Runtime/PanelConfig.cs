﻿using System;

namespace Assets.Scripts.PanelService
{
    using UnityEngine;

    [Serializable]
    public class PanelConfig
    {
        public PanelBase prefab;
        public bool destroyOnHide;
        public CanvasId canvasId = CanvasId.MainCanvas;
        public bool shouldFitToCanvas = true;
        public int prewarmCount; // Number of instances to pre-instantiate
    }
}