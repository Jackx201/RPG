using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;
//sing UnityEngine.UIElements;


public class BranchingDialog : MonoBehaviour
{
    [SerializeField] private GameObject branchingCanvas;
    [SerializeField] private GameObject dialogprefab;
    [SerializeField] private GameObject answersPrefab;
    [SerializeField] private TextAssetValue dialogValue;
    [SerializeField] private Story myStory;
    [SerializeField] private GameObject dialogHolder;
    [SerializeField] private GameObject answersHolder;
    [SerializeField] private ScrollRect dialogScroll;

    public void EnableCanvas()
    {
        branchingCanvas.SetActive(true);
        SetStory();
        RefreshView();
    }

    public void DisableCanvas()
    {
        branchingCanvas.SetActive(false );
    }

    public void SetStory(){
        if(dialogValue.value)
        {
            DeleteDialogs();
            myStory = new Story(dialogValue.value.text);
        } else {
            //Debug.Log("Crashed :(");
        }
    }

    public void RefreshView()
    {
        while (myStory.canContinue)
        {
            MakeNewDialog(myStory.Continue());
        }
        if(myStory.currentChoices.Count > 0)
        {
            MakeNewChoices();
        }
        else 
        {
            branchingCanvas.SetActive(false );
        }
        StartCoroutine(ScrollCo());
    }

    void MakeNewDialog(string newDialog)
    {
        //Generate the prefab dialog inside the scroll view
        DialogObject newDialogObject = 
        Instantiate(dialogprefab, dialogHolder.transform).GetComponent<DialogObject>();
        newDialogObject.Setup(newDialog);
    }

    void MakeNewChoices()
    {

        for(int i =0 ; i < answersHolder.transform.childCount; i++)
        {
            Destroy(answersHolder.transform.GetChild(i).gameObject);
        }
        for(int i = 0; i < myStory.currentChoices.Count; i++)
        {
            MakeNewresponse(myStory.currentChoices[i].text, i);
        }


    }

    void MakeNewresponse(string newDialog, int choiceValue)
    {
        ResponseObject newAnswerObject =
        Instantiate(answersPrefab, answersHolder.transform).GetComponent<ResponseObject>();
        newAnswerObject.Setup(newDialog, choiceValue);
        Button responeButton = newAnswerObject.gameObject.GetComponent<Button>();
        if(responeButton)
        {
            responeButton.onClick.AddListener(delegate {ChooseChoice(choiceValue);});
        }
    }

        void ChooseChoice(int choice)
        {
            myStory.ChooseChoiceIndex(choice);
            RefreshView();
        } 

        IEnumerator ScrollCo()
        {
            yield return null;
            dialogScroll.verticalNormalizedPosition = 0f;
        }

        void DeleteDialogs()
        {
            for(int i = 0; i < dialogHolder.transform.childCount; i++)
            {
                Destroy(dialogHolder.transform.GetChild(i).gameObject);
            }
        }
}

