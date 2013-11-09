﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Assumes vertical, just rotate the base for horizontal.
/// </summary>
public class UIEnergyBar : MonoBehaviour {
    public delegate void GenericCallback(UIEnergyBar bar);

    public UILabel label;
    public UISprite icon;

    public UISprite bar; //make sure anchor is bottom and it is in tile mode
    public int barHeight = 3; //height of each bar

    public UISprite panelTop; //make sure anchor is bottom
    public float panelTopYOfs;
    public UISprite panelBase; //make sure anchor is bottom

    public float smoothPerBarDelay = 0.1f; //for when smoothing

    public event GenericCallback animateEndCallback;

    private int mCurMaxBar = 1;
    private int mCurNumBar;

    private bool mIsAnimate;
    private float mCurT;
    private float mEndT;
    private float mDelay;
    private float mCurV;
    private float mLastAnimTime;
    private float mCurAnimTime;

    public int max {
        get { return mCurMaxBar; }
        set {
            if(mCurMaxBar != value) {
                mCurMaxBar = value;
                RefreshHeight();
            }
        }
    }

    public int current {
        get { return mCurNumBar; }
        set {
            if(mCurNumBar != value) {
                mCurT = (float)value;
                mCurNumBar = value;
                                
                if(mIsAnimate && animateEndCallback != null) {
                    animateEndCallback(this);
                }

                mIsAnimate = false;

                RefreshBars();
            }
        }
    }

    /// <summary>
    /// Use this for smoothing the transition
    /// </summary>
    public int currentSmooth {
        get { return mCurNumBar; }
        set {
            if(mCurNumBar != value) {
                mCurNumBar = value;
                mEndT = (float)value;

                float numBars = Mathf.Abs(Mathf.Round(mEndT - mCurT));
                mDelay = numBars * smoothPerBarDelay;

                if(!mIsAnimate) {
                    mIsAnimate = true;
                    mLastAnimTime = Time.realtimeSinceStartup;
                    mCurAnimTime = 0.0f;
                }
            }
            else {
                if(!mIsAnimate && animateEndCallback != null) {
                    animateEndCallback(this);
                }
            }
        }
    }

    public void SetIconSprite(string atlasRef) {
        if(icon) {
            icon.spriteName = atlasRef;
            icon.MakePixelPerfect();
        }
    }

    public void SetBarSprite(string atlasRef) {
        bar.spriteName = atlasRef;
    }

    public void SetBarColor(Color clr) {
        bar.color = clr;
    }

    void OnDisable() {
        mCurT = (float)mCurNumBar;
        mIsAnimate = false;
        mCurV = 0.0f;
        mCurAnimTime = 0.0f;
    }

    void OnDestroy() {
        animateEndCallback = null;
    }

    void RefreshHeight() {
        int h = barHeight * mCurMaxBar;

        if(panelBase) {
            panelBase.height = h;
        }

        if(panelTop) {
            Vector3 topPos = new Vector3(0, h + panelTopYOfs, 0);
            panelTop.transform.localPosition = topPos;
        }
    }

    void RefreshBars() {
        if(mCurNumBar == 0)
            bar.gameObject.SetActive(false);
        else {
            bar.gameObject.SetActive(true);
            bar.height = mCurNumBar*barHeight;
        }
    }

    void Update() {
        if(mIsAnimate) {
            float dt = Time.realtimeSinceStartup - mLastAnimTime;
            mCurT = Mathf.SmoothDamp(mCurT, mEndT, ref mCurV, mDelay, Mathf.Infinity, dt);
            mLastAnimTime = Time.realtimeSinceStartup;
            mCurAnimTime += dt;

            if(mCurAnimTime >= mDelay) {
                RefreshBars();

                mIsAnimate = false;

                if(animateEndCallback != null)
                    animateEndCallback(this);
            }
            else {
                int b = Mathf.RoundToInt(mCurT);
                if(b == 0)
                    bar.gameObject.SetActive(false);
                else {
                    bar.gameObject.SetActive(true);
                    bar.height = b * barHeight;
                }
            }
        }
    }
}