using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 空間操作可能な境界
/// </summary>
public class SpaceScreen : MasterScreen
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        @event.SetSelectedGameObject(firstObject);
        SelectFirstElement();
    }

    protected override void Initialize()
    {
        if (!@event)
            @event = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    protected override void SelectFirstElement()
    {
        var g = transform.GetChild(0).GetChild(0).gameObject;
        @event.SetSelectedGameObject(g);
    }
}
