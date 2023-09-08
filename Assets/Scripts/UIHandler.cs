using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class UIHandler : MonoBehaviour
{
    #region singleton
    private static UIHandler _instance;
    public static UIHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIHandler>();
                if (_instance == null)
                {
                    _instance = new GameObject().AddComponent<UIHandler>();
                }
            }
            return _instance;
        }
    }
    #endregion

    private MyVariblesF myVariblesF;
    private void Awake()
    {//Singleton
        if (_instance != null)
            Destroy(this);
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        myVariblesF = MyVariblesF.Instance;
    }
    public void OpenFullScreenVideoPanel(VideoPlayer videoPlayer)
    {//Burada videomuzun full ekran yapan arkaplan� vs a��yoruz ve s�reyi ekrana yazd�r�yoruz.
        myVariblesF.BackgroundPlane.SetActive(true);
        myVariblesF.CloseButton.SetActive(true);
        myVariblesF.CountdownTextClose.gameObject.SetActive(true);
        myVariblesF.VideoDuration = (float)videoPlayer.length - (float)videoPlayer.time;//Burda toplam s�reyi oynat�lan s�recden ��kart�yoruz. //Bu y�zden olmuyor. 
        StartCoroutine(VideoCountdownOpenText());
    }
    
    IEnumerator VideoCountdownOpenText()
    {//Burada sayac�m�z� ba�lat�yoruz ve kullan�c� kapatmazsa videoyu s�resi bitti�inde kendili�inden kapan�yor.
        while(myVariblesF.IsVideoFullScreen && myVariblesF.VideoDuration > 0)
        {
            myVariblesF.CountdownTextClose.gameObject.SetActive(true);
            myVariblesF.VideoDuration -= Time.fixedDeltaTime;
            myVariblesF.CountdownTextClose.text = myVariblesF.VideoDuration.ToString("F2");
            yield return new WaitForFixedUpdate();
        }
        if(myVariblesF.VideoDuration <= 0)
        {
            myVariblesF.IsVideoFullScreen = false;
            myVariblesF.OpenFullScreen = false;
            myVariblesF.LoadScene();//En son dayanamad�m scene loadlatt�m. Touch'daki myVariblesF.IsLittleVideoOpen'da s�k�nt� var onu ��z.
        }
    }

    public void CloseFullScreenVideoPanel()
    {//
        myVariblesF.BackgroundPlane.SetActive(false);
        myVariblesF.CloseButton.SetActive(false);
        myVariblesF.CountdownTextClose.gameObject.SetActive(false);
    }


}
