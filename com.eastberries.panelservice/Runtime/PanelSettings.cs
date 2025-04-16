namespace Assets.Scripts.PanelService
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "PanelSettings", menuName = "Panel System/Panel Settings")]
    public class PanelSettings : ScriptableObject
    {
        public List<PanelConfig> panelConfigs = new();
    }
    
    
}