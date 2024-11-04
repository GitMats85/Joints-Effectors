using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering; //F�r att f� tillg�ng till volymer
using UnityEngine.Rendering.Universal; //F�r att f� tillg�ng till postprocess effekter

public class PoisonEffect : MonoBehaviour
{
    Volume v; //Deklaration av volymen
    ChromaticAberration chromaticAberration; //Deklaration av specifika effekter
    ColorAdjustments colorAdjustments;

    [SerializeField] float oscillationSpeed = 2f;
    [SerializeField] float hueSpeed;

    float x = 0;
    private bool poisioned = false;

    void Start()
    {
        v = GetComponent<Volume>();
        v.profile.TryGet(out chromaticAberration); //Referenser till de specifika effekterna
        v.profile.TryGet(out colorAdjustments);
    }

    void Update()
    {
        if (poisioned)
        {
            float oscillatingValue = Mathf.Sin(Time.time * oscillationSpeed) * 0.5f + 0.5f; //Matematiskformel f�r att loopa v�rdet mellan 0-1)
            chromaticAberration.intensity.value = oscillatingValue;
            colorAdjustments.postExposure.value = oscillatingValue;

            //Hueshift g�r fr�n 0-180, sedan vill jag att den ska g� tillbaka till 0 igen f�r att f� "b�st" effekt
            if (x <= 180)
            {
                colorAdjustments.hueShift.value = x;
                x += Time.deltaTime * hueSpeed;
            }
            else
            { x = 0; }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        { 
            poisioned = true;
            Debug.Log("Player is poisoned!");
        }
    }
}
