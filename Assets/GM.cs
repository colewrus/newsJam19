using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;


public class GM : MonoBehaviour
{

    public Slider meter;
    public AudioClip[] clips;
    AudioSource source; 
  
    float maxScore;
    public float currentScore;

    float tickTimer = 2.0f;
    float tick;

   


    //add to csv
    public GameObject formPanel;
    public TMP_InputField fIncite;
    public TMP_InputField fExplain;
    public TMP_InputField fChallenge;

    private char lineSeperater = '\n'; // It defines line seperate character
    private char fieldSeperator = ','; // It defines field seperate chracter

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        formPanel.SetActive(false);
        currentScore = 0;
        maxScore = meter.maxValue;
        source.clip = clips[3];
        source.loop = true;
        source.Play();
    }



    public void AddStress(float val)
    {
        if (currentScore + val > 0)
            currentScore += val;
        else
            currentScore = 0;
        meter.value = currentScore;
        
    }





    public void AddValue()//for submitting the form
    {
        // Following line adds data to CSV file

        File.AppendAllText(getPath() + "/boomer.csv", lineSeperater + fIncite.text + fieldSeperator + fExplain.text + fieldSeperator + fChallenge.text);
        formPanel.SetActive(false);
        source.clip = clips[3];
        source.Play();
        Dselector.instance.TitleCard.SetActive(true);
    }

    // Get path for given CSV file
    private static string getPath()
    {
#if UNITY_EDITOR
        return Application.dataPath;
#elif UNITY_ANDROID
return Application.persistentDataPath;// +fileName;
#elif UNITY_IPHONE
return GetiPhoneDocumentsPath();// +"/"+fileName;
#else
return Application.dataPath;// +"/"+ fileName;
#endif
    }

    public void EndMusic()
    {
        if(currentScore > 10 && currentScore < 16) //tense
        {
            source.clip = clips[0];
            source.Play();
        }
        if(currentScore >= 16)//stress
        {
            source.clip = clips[1];
            source.Play();
        }
        if(currentScore <= 10)//calm
        {
            source.clip = clips[2];
            source.Play();
        }

        StartCoroutine(FormDelay());
    }

    IEnumerator FormDelay()
    {
        yield return new WaitForSeconds(6);
        formPanel.SetActive(true);
    }


}
