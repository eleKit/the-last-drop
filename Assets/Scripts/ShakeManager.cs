using System.Collections;
using System.Collections.Generic;
using POLIMIGameCollective;
using UnityEngine;

public class ShakeManager : Singleton<ShakeManager> {

    public float avrgTime = 0.5f;
    public float peakLevel = 0.6f;
    public float endCountTime = 0.6f;
    [Tooltip("How many shakes are required")]
    public float ShakeReq = 2; 
    public int shakeDir;
    public int shakeCount;

    Vector3 avrgAcc = Vector3.zero;
    int countPos;
    int countNeg;
    int lastPeak;
    int firstPeak;
    bool counting;
    float timer;

    void Update()
    {
        if (ShakeDetector())
        { // call ShakeDetector every Update!
          // the device was shaken up and the count is in shakeCount
          // the direction of the first shake is in shakeDir (1 or -1)

            if( Mathf.Abs(shakeCount) > ShakeReq)
                EventManager.TriggerEvent("Shake");

        }
        // the variable counting tells when the device is being shaken:   <---- BULLSHIT!
/*        if (counting)
        {
            EventManager.TriggerEvent("Shake");
        }
        */
    }

    bool ShakeDetector()
    {
        // read acceleration:
        Vector3 curAcc = Input.acceleration;

        // update average value:
        avrgAcc = Vector3.Lerp(avrgAcc, curAcc, avrgTime * Time.deltaTime);

        // calculate peak size:
        curAcc -= avrgAcc;

        // variable peak is zero when no peak detected...
        int peak = 0;

        // or +/- 1 according to the peak polarity:
        if (curAcc.y > peakLevel) peak = 1;
        if (curAcc.y < -peakLevel) peak = -1;

        // do nothing if peak is the same of previous frame:
        if (peak == lastPeak) return false;

        // peak changed state: process it
        lastPeak = peak; // update lastPeak
        if (peak != 0)
        { // if a peak was detected...
            timer = 0; // clear end count timer...
            if (peak > 0) // and increment corresponding count
            {
                countPos++;
            }
            else
            {
                countNeg++;
            }
            if (!counting)
            { // if it's the first peak...
                counting = true; // start shake counting
                firstPeak = peak; // save the first peak direction
            }
        }
        else // but if no peak detected...
        if (counting)
        { // and it was counting...
            timer += Time.deltaTime; // increment timer
            if (timer > endCountTime)
            { // if endCountTime reached...
                counting = false; // finish counting...
                shakeDir = firstPeak; // inform direction of first shake...
                if (countPos > countNeg) // and return the higher count
                    shakeCount = countPos;
                else
                    shakeCount = countNeg;
                // zero counters and become ready for next shake count
                countPos = 0;
                countNeg = 0;
                return true; // count finished
            }
        }

        return false;
    }

}
