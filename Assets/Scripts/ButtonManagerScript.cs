using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManagerScript : MonoBehaviour
{
    private UIHandler uIHandler;
    private MyVariblesF myVariblesF;
    private void Start()
    {
        uIHandler = UIHandler.Instance;
        myVariblesF = MyVariblesF.Instance;
    }
    public void CloseFullScreen()
    {//Burara ekran�m�z� kapatmak i�in de�i�kenleri de�i�tiriyoruz.
        myVariblesF.IsVideoFullScreen = false;//Art�k full videoda de�il bunun i�in de�i�tiriyoruz.
        myVariblesF.OpenFullScreen = false;//Yeni videoyu a�aca�� zaman �nce k���k g�rmesi i�in de�i�tiriyoruz.
        myVariblesF.LoadScene();
        //myVariblesF.LoadScene();
    }
}
