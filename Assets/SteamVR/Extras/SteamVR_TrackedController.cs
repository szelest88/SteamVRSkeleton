﻿//======= Copyright (c) Valve Corporation, All rights reserved. ===============
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public struct ClickedEventArgs
{
    public uint controllerIndex;
    public uint flags;
    public float padX, padY;
}

public delegate void ClickedEventHandler(object sender, ClickedEventArgs e);

public class SteamVR_TrackedController : MonoBehaviour
{
    public uint controllerIndex;
    public VRControllerState_t controllerState;
    public bool triggerPressed = false;
    public bool steamPressed = false;
    public bool menuPressed = false;
    public bool padPressed = false;
    public bool padTouched = false;
    public bool gripped = false;

    public event ClickedEventHandler MenuButtonClicked;
    public event ClickedEventHandler MenuButtonUnclicked;
    public event ClickedEventHandler TriggerClicked;
    public event ClickedEventHandler TriggerUnclicked;
    public event ClickedEventHandler SteamClicked;
    public event ClickedEventHandler PadClicked;
    public event ClickedEventHandler PadUnclicked;
    public event ClickedEventHandler PadTouched;
    public event ClickedEventHandler PadUntouched;
    public event ClickedEventHandler Gripped;
    public event ClickedEventHandler Ungripped;

    // Use this for initialization
    void Start()
    {
        is_gun = true;
        if (this.GetComponent<SteamVR_TrackedObject>() == null)
        {
            gameObject.AddComponent<SteamVR_TrackedObject>();
        }

		if (controllerIndex != 0)
		{
			this.GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)controllerIndex;
			if (this.GetComponent<SteamVR_RenderModel>() != null)
			{
				this.GetComponent<SteamVR_RenderModel>().index = (SteamVR_TrackedObject.EIndex)controllerIndex;
			}
		}
		else
		{
			controllerIndex = (uint) this.GetComponent<SteamVR_TrackedObject>().index;
        }
        for(int i =0;i<transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "Shield")
            {
                is_gun = false;
            }
        }
    }
    public bool is_gun;
	public void SetDeviceIndex(int index)
	{
			this.controllerIndex = (uint) index;
	}

	public virtual void OnTriggerClicked(ClickedEventArgs e)
    {
        if (TriggerClicked != null)
        {
            TriggerClicked(this, e);
        }
    }

    public virtual void OnTriggerUnclicked(ClickedEventArgs e)
    {
        if (TriggerUnclicked != null)
            TriggerUnclicked(this, e);
    }

    public virtual void OnMenuClicked(ClickedEventArgs e)
    {
        if (MenuButtonClicked != null)
            MenuButtonClicked(this, e);
    }

    public virtual void OnMenuUnclicked(ClickedEventArgs e)
    {
        if (MenuButtonUnclicked != null)
            MenuButtonUnclicked(this, e);
    }

    public virtual void OnSteamClicked(ClickedEventArgs e)
    {
        if (SteamClicked != null)
            SteamClicked(this, e);
    }

    public virtual void OnPadClicked(ClickedEventArgs e)
    {
        if (PadClicked != null)
            PadClicked(this, e);
    }

    public virtual void OnPadUnclicked(ClickedEventArgs e)
    {
        if (PadUnclicked != null)
            PadUnclicked(this, e);
    }

    public virtual void OnPadTouched(ClickedEventArgs e)
    {
        if (PadTouched != null)
            PadTouched(this, e);
    }

    public virtual void OnPadUntouched(ClickedEventArgs e)
    {
        if (PadUntouched != null)
            PadUntouched(this, e);
    }

    public virtual void OnGripped(ClickedEventArgs e)
    {
        if (Gripped != null)
            Gripped(this, e);
    }

    public virtual void OnUngripped(ClickedEventArgs e)
    {
        if (Ungripped != null)
            Ungripped(this, e);
    }


    
    public int bulletSpeed;

    public GameObject bulletPrefab;
    public ShieldEntity shield;
    // Update is called once per frame
    void Update()
    {
        
        var system = OpenVR.System; // this fires
        if (system != null && system.GetControllerState(controllerIndex, ref controllerState))
		{

            ulong trigger = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_SteamVR_Trigger));
            

            if (trigger > 0L && !triggerPressed)
            {

            //    SteamVR_Controller.Input((int)controllerIndex).TriggerHapticPulse(2000);
                if (is_gun)         
                {
                    Debug.LogError("JEB Z GUNA!");
                    var bullet = (GameObject)Instantiate(bulletPrefab, transform.position, transform.rotation);
                    bullet.GetComponent<Rigidbody>().velocity = transform.rotation * Vector3.forward * bulletSpeed;
                }
                triggerPressed = true;
                ClickedEventArgs e;
                e.controllerIndex = controllerIndex;
                e.flags = (uint)controllerState.ulButtonPressed;
                e.padX = controllerState.rAxis0.x;
                e.padY = controllerState.rAxis0.y;
                OnTriggerClicked(e);

            }
            else if (trigger == 0L && triggerPressed)
            {
                triggerPressed = false;
                ClickedEventArgs e;
                e.controllerIndex = controllerIndex;
                e.flags = (uint)controllerState.ulButtonPressed;
                e.padX = controllerState.rAxis0.x;
                e.padY = controllerState.rAxis0.y;
                OnTriggerUnclicked(e);
            }

            ulong grip = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_Grip));
            if (grip > 0L && !gripped)
            {
                gripped = true;
                ClickedEventArgs e;
                e.controllerIndex = controllerIndex;
                e.flags = (uint)controllerState.ulButtonPressed;
                e.padX = controllerState.rAxis0.x;
                e.padY = controllerState.rAxis0.y;
                OnGripped(e);

            }
            else if (grip == 0L && gripped)
            {
                gripped = false;
                ClickedEventArgs e;
                e.controllerIndex = controllerIndex;
                e.flags = (uint)controllerState.ulButtonPressed;
                e.padX = controllerState.rAxis0.x;
                e.padY = controllerState.rAxis0.y;
                OnUngripped(e);
            }

            ulong pad = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_SteamVR_Touchpad));
            if (pad > 0L && !padPressed)
            {
                padPressed = true;
                ClickedEventArgs e;
                e.controllerIndex = controllerIndex;
                e.flags = (uint)controllerState.ulButtonPressed;
                e.padX = controllerState.rAxis0.x;
                e.padY = controllerState.rAxis0.y;
                OnPadClicked(e);
            }
            else if (pad == 0L && padPressed)
            {
                
                padPressed = false;
                ClickedEventArgs e;
                e.controllerIndex = controllerIndex;
                e.flags = (uint)controllerState.ulButtonPressed;
                e.padX = controllerState.rAxis0.x;
                e.padY = controllerState.rAxis0.y;
                OnPadUnclicked(e);
            }

            ulong menu = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_ApplicationMenu));
            if (menu > 0L && !menuPressed)
            {
                menuPressed = true;
                ClickedEventArgs e;
                e.controllerIndex = controllerIndex;
                
                e.flags = (uint)controllerState.ulButtonPressed;
                e.padX = controllerState.rAxis0.x;
                e.padY = controllerState.rAxis0.y;
                OnMenuClicked(e);
            }
            else if (menu == 0L && menuPressed)
            {
                menuPressed = false;
                ClickedEventArgs e;
                e.controllerIndex = controllerIndex;
                e.flags = (uint)controllerState.ulButtonPressed;
                e.padX = controllerState.rAxis0.x;
                e.padY = controllerState.rAxis0.y;
                OnMenuUnclicked(e);
            }

            pad = controllerState.ulButtonTouched & (1UL << ((int)EVRButtonId.k_EButton_SteamVR_Touchpad));
            if (pad > 0L && !padTouched)
            {
                
                padTouched = true;
                ClickedEventArgs e;
                e.controllerIndex = controllerIndex;
                e.flags = (uint)controllerState.ulButtonPressed;
                e.padX = controllerState.rAxis0.x;
                e.padY = controllerState.rAxis0.y;
                OnPadTouched(e);

            }
            else if (pad == 0L && padTouched)
            {
                padTouched = false;
                ClickedEventArgs e;
                e.controllerIndex = controllerIndex;
                e.flags = (uint)controllerState.ulButtonPressed;
                e.padX = controllerState.rAxis0.x;
                e.padY = controllerState.rAxis0.y;
                OnPadUntouched(e);
            }

            if(triggerPressed && !is_gun)
            {
                shield.Trigger();
            }
        }
    }
}
