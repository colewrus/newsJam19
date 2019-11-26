using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dselector : MonoBehaviour
{

    public static Dselector instance = null;
    public GM manager;  

    Dialogue[] tree;



    //game state
    public GameObject TitleCard;
    int position; //this tracks what line we have pulled from the csv so we know the right responses for the buttons;
    public List<int> pastPositions = new List<int>(); //throw past numbers here so we don't repeat
    public bool qBool; //this controls when the buttons can be pressed

    //UI Shtuph
    public TextMeshProUGUI question;
    public TextMeshProUGUI response;
    public TextMeshProUGUI tidbits;

    //CSV Parser
    public TextAsset csvFile;

    private char lineSeperater = '\n'; // It defines line seperate character
    private char fieldSeperator = ','; // It defines field seperate chracter

    string[] fields;
    int csvLength;
    public List<string> sField = new List<string>();

   


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        TitleCard.SetActive(true);
        readData();
        CreateTree(csvLength);

        qBool = false;

       
    }


    //create the dialog tree
    void CreateTree(int csvLines)
    {
        tree = new Dialogue[csvLines];

        for(int i=0; i<csvLines; i++)
        {
            tree[i] = new Dialogue(sField[(i * 3) + 0], sField[(i * 3) + 1], sField[(i * 3) + 2]);  
            
        }
            
    }


    //------testing zone-----
    public void StartGame()//should be a button, eventually
    {
        TitleCard.SetActive(false);
        AssignPosition();
        
    }





    //Live area

    public void AssignPosition()
    {
        position = Random.Range(0, tree.Length);
        qBool = false;
        if (!pastPositions.Contains(position))
        {
            pastPositions.Add(position);
            SetQuestion();
        }
        else 
        {
            if (pastPositions.Count < tree.Length)
            {
                AssignPosition();
            }
            else
            {
                //end of game
                question.alignment = TextAlignmentOptions.Center;
                question.text = "Congrats you have ruined Thanksgiving!\nStress Level: " + manager.currentScore;
                StartCoroutine(FadeTextToFullAlpha(1, question));
                manager.EndMusic();
            }

        }

        Debug.Log("position set " + position);

    }

    //ask the question
    public void SetQuestion() {

        question.text = tree[position].question;
        StartCoroutine(FadeTextToFullAlpha(1, question));
        StartCoroutine(Qdelay(1f));
    }

    //then wait to enable the player the chance to press a response
    IEnumerator Qdelay(float t)
    {
        yield return new WaitForSeconds(t);
        qBool = true;
    }

    //for buttons
    public void Challenge()
    {
        if (qBool)
        {
            response.text = tree[position].challenge;
            StartCoroutine(FadeTextToFullAlpha(1, response));
            qBool = false;
            manager.AddStress(3);
            StartCoroutine(PostAnswer(3));
        }

    }

    public void Explain()
    {
        Debug.Log("explain pressed");
        if (qBool)
        {
            response.text = tree[position].explanation;
            StartCoroutine(FadeTextToFullAlpha(1, response));
            qBool = false;
            manager.AddStress(-0.5f);
            StartCoroutine(PostAnswer(2));
        }
        else
        {
            Debug.Log("explain error");
        }


    }

    public void Boomer()
    {
        if (qBool)
        {
            response.text = "OK BOOMER";
            StartCoroutine(FadeTextToFullAlpha(0.5f, response));
            qBool = false;
            manager.AddStress(5);
            StartCoroutine(PostAnswer(2));
        }

    }

    //Need a button for the distract
    public void Distract()
    {
        if (qBool)
        {
            response.text = "Uh....how about that weather?";
            StartCoroutine(FadeTextToFullAlpha(0.5f, response));
            qBool = false;
            manager.AddStress(1);
            StartCoroutine(PostAnswer(2));
        }
    }


    //fade out the dialog and wait for a spell before starting again
    IEnumerator PostAnswer(float t)
    {
        Debug.Log("stareted post answer");
        yield return new WaitForSeconds(t);
        StartCoroutine(FadeTextToZeroAlpha(1, question));
        StartCoroutine(FadeTextToZeroAlpha(1, response));
        yield return new WaitForSeconds(Random.Range(2, 4));
        AssignPosition();
    }


    // Read data from CSV file
    private void readData()
    {
        string[] records = csvFile.text.Split(lineSeperater);
        csvLength = records.Length;

        Debug.Log("length " + csvLength);

        foreach (string record in records)
        {
            fields = record.Split(fieldSeperator);

      

            foreach (string field in fields)
            {
                sField.Add(field);
               
            }

            //make the Dialogue object and add to the array;
            //contentArea.text += '\n';
        }
    }


    public IEnumerator FadeTextToFullAlpha(float t, TextMeshProUGUI i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }


    public IEnumerator FadeTextToZeroAlpha(float t, TextMeshProUGUI i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }


}
