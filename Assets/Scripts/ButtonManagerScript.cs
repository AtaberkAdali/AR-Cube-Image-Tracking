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
    {//Burara ekranýmýzý kapatmak için deðiþkenleri deðiþtiriyoruz.
        myVariblesF.IsVideoFullScreen = false;//Artýk full videoda deðil bunun için deðiþtiriyoruz.
        myVariblesF.OpenFullScreen = false;//Yeni videoyu açacaðý zaman önce küçük görmesi için deðiþtiriyoruz.
        myVariblesF.LoadScene();
        //myVariblesF.LoadScene();
    }
}
