using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using Assets.Scripts.PanelService;
using IPanel = Assets.Scripts.PanelService.IPanel;
using PanelSettings = Assets.Scripts.PanelService.PanelSettings;

public interface IPanelService
{
    Task<T> ShowPanelAsync<T>() where T : IPanel;
    Task<T> ShowPanelAsync<T, TD>(TD panelParameter) where T : IPanel;
    Task HidePanelAsync<T>() where T : IPanel;
    Task HidePanelAsync(IPanel panel);
    Task HideAllAsync();
    bool TryGetPanel<T>(out T panel) where T : IPanel;
}

public interface IPanelParameter<T>
{
    public T Parameter { get; set; }
}

public class PanelService : IPanelService
{
    private readonly Dictionary<string, PanelConfig> _panelConfigs = new();
    private readonly List<IPanel> _activePanels = new();
    private readonly IPanelFactory _panelFactory;
    private readonly CanvasHandler _canvasHandler;
    private readonly IPanelPool _panelPool;


    public PanelService(
        IPanelFactory panelFactory,
        PanelSettings panelSettings,
        CanvasHandler canvasHandler,
        IPanelPool panelPool)
    {
        _panelFactory = panelFactory;
        _canvasHandler = canvasHandler;
        _panelPool = panelPool;
        foreach (var config in panelSettings.panelConfigs)
        {
            _panelConfigs[config.prefab.GetType().Name] = config;
            if (config.prewarmCount > 0)
            {
                _panelPool.PrewarmPool(config.prefab.gameObject, config.prewarmCount);
            }
        }
    }

    public async Task<T> ShowPanelAsync<T>() where T : IPanel
    {
        var panelId = typeof(T).Name;

        if (!_panelConfigs.TryGetValue(panelId, out var panelConfig))
        {
            throw new ArgumentException($"Panel config for panelId {panelId} not found!");
        }

        if (TryGetPanel(out T existingPanel))
        {
            await existingPanel.ShowAsync();
            return existingPanel;
        }

        var canvasTransform = _canvasHandler.GetCanvasTransform(panelConfig.canvasId);
        var panel = _panelPool.Get(panelConfig.prefab.gameObject, canvasTransform);

        if (panel is T typedPanel)
        {
            panel.SetPanelData(new PanelData()
            {
                DestroyOnHide = panelConfig.destroyOnHide,
                CanvasId = panelConfig.canvasId,
                PanelId = panelId,
                ShouldFitToCanvas = panelConfig.shouldFitToCanvas
            });

            panel.Reset();
            panel.Initialize();

            if (panelConfig.shouldFitToCanvas)
            {
                panel.FitToCanvas();
            }

            _activePanels.Add(panel);
            await panel.ShowAsync();
            return typedPanel;
        }


        throw new InvalidOperationException($"Created panel is not of type {typeof(T)}!");
    }

    public async Task<T> ShowPanelAsync<T, TD>(TD panelParameter) where T : IPanel
    {
        var panelId = typeof(T).Name;

        if (!_panelConfigs.TryGetValue(panelId, out var panelConfig))
        {
            throw new ArgumentException($"Panel config for panelId {panelId} not found!");
        }

        if (TryGetPanel(out T existingPanel))
        {
            if (existingPanel is IPanelParameter<TD> parameterHolder)
            {
                parameterHolder.Parameter = panelParameter;
            }

            await existingPanel.ShowAsync();
            return existingPanel;
        }

        var canvasTransform = _canvasHandler.GetCanvasTransform(panelConfig.canvasId);
        var panel = _panelPool.Get(panelConfig.prefab.gameObject, canvasTransform);

        if (panel is T typedPanel)
        {
            panel.SetPanelData(new PanelData()
            {
                DestroyOnHide = panelConfig.destroyOnHide,
                CanvasId = panelConfig.canvasId,
                PanelId = panelId,
                ShouldFitToCanvas = panelConfig.shouldFitToCanvas
            });

            if (panel is IPanelParameter<TD> parameterHolder)
            {
                parameterHolder.Parameter = panelParameter;
            }

            panel.Reset();
            panel.Initialize();

            if (panelConfig.shouldFitToCanvas)
            {
                panel.FitToCanvas();
            }

            _activePanels.Add(panel);
            await panel.ShowAsync();
            return typedPanel;
        }


        throw new InvalidOperationException($"Created panel is not of type {typeof(T)}!");
    }

    public async Task HidePanelAsync<T>() where T : IPanel
    {
        if (TryGetPanel(out T panel))
        {
            await HidePanelAsync(panel);
        }
    }

    public async Task HidePanelAsync(IPanel panel)
    {
        if (panel == null || !_activePanels.Contains(panel)) return;
        await panel.HideAsync();
        _activePanels.Remove(panel);

        if (!panel.PanelData.DestroyOnHide)
        {
            _panelPool.Return(panel);
        }
    }

    public async Task HideAllAsync()
    {
        var panels = _activePanels.ToList();
        var tasks = panels.Select(HidePanelAsync);
        await Task.WhenAll(tasks);
    }

    public bool TryGetPanel<T>(out T panel) where T : IPanel
    {
        panel = _activePanels.OfType<T>().FirstOrDefault();
        return panel != null;
    }
}