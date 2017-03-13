using UnityEngine;
using System.Collections;

/// <summary>
/// UpDown Button, changes state based on if it is up or down
/// </summary>
[AddComponentMenu("2D Toolkit/UI/tk2dUIUpDownButton")]
public class tk2dUIUpDownButton : tk2dUIBaseItemControl
{
    /// <summary>
    /// State that will be active if up and deactivated if down
    /// </summary>
    public GameObject upStateGO;

    /// <summary>
    /// State that will be active if down and deactivated if up
    /// </summary>
    public GameObject downStateGO;

    /// <summary>
    /// Use OnRelase instead of OnUp to toggle state
    /// </summary>
    [SerializeField]
    private bool useOnReleaseInsteadOfOnUp = false;

    public bool UseOnReleaseInsteadOfOnUp
    {
        get { return useOnReleaseInsteadOfOnUp; }
    }

//	[SerializeField]
	public bool isActive;

//	public bool IsActive {
//		get { return isActive; }
//		set { isActive = IsActive; }
//	}

    private bool isDown = false;

    void Start()
    {
		isActive = true;
        SetState();
    }

    void OnEnable()
    {
        if (uiItem)
        {
            uiItem.OnDown += ButtonDown;
            if (useOnReleaseInsteadOfOnUp)
            {
                uiItem.OnRelease += ButtonUp;
            }
            else
            {
                uiItem.OnUp += ButtonUp;
            }
        }
    }

    void OnDisable()
    {
        if (uiItem)
        {
            uiItem.OnDown -= ButtonDown;
            if (useOnReleaseInsteadOfOnUp)
            {
                uiItem.OnRelease -= ButtonUp;
            }
            else
            {
                uiItem.OnUp -= ButtonUp;
            }
        }
    }

    private void ButtonUp()
    {
        isDown = false;
        SetState();
    }

    private void ButtonDown()
    {
        isDown = true;
        SetState();
    }

    private void SetState()
    {
		Debug.Log ("A isActive= "+ isActive);
		if (isActive) {
//			Debug.Log ("B");
			ChangeGameObjectActiveStateWithNullCheck (upStateGO, !isDown);
			ChangeGameObjectActiveStateWithNullCheck (downStateGO, isDown);
		}
    }

    /// <summary>
    /// Internal do not use
    /// </summary>
    public void InternalSetUseOnReleaseInsteadOfOnUp(bool state)
    {
        useOnReleaseInsteadOfOnUp = state;
    }
}

