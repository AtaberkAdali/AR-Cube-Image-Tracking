using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

//Buraya çok yorum satýrý yazmýyorum neden kullanýldýklarý genelde belli.

public class MyVariblesF : MonoBehaviour 
{
    #region singleton
    private static MyVariblesF _instance;
    public static MyVariblesF Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<MyVariblesF>();
                if(_instance == null)
                {
                    _instance = new GameObject().AddComponent<MyVariblesF>();//Eðer ki bu obje yoksa oluþturuyor.
                }
            }
            return _instance;
        }
    }
    #endregion

    
    [SerializeField] GameObject backgroundPlaneGO, closeButtonGO;
    [SerializeField] TextMeshProUGUI countdownTextCloseGO;

    public GameObject BackgroundPlane { get; private set; }
    public GameObject CloseButton { get; private set; }
    public GameObject PrefabsTransformParent { get; private set; }
    public TextMeshProUGUI CountdownTextClose { get; set; }
    public Vector3 PositionSetupVec { get; set; }
    public bool OpenFullScreen { get; set; }//ekraný Full Screen Yapma veya normale döndürme deðiþkeni.
    public bool IsVideoFullScreen { get; set; }//full screen yaptýktan sonra resim gitse de oynatmak için olan deðiþken.
    public bool IsLittleVideoOpen { get; set; }//Þu anda oynayan bir küçük video var mý bunu anlatan deðiþken.
    public float VideoDuration { get; set; }//Video süresi

    private void Awake()
    {//Singleton
        if (_instance != null)
            Destroy(this);
        DontDestroyOnLoad(this);
    }

    public void ObjectsInitialValues()
    {
        BackgroundPlane = backgroundPlaneGO;
        CloseButton = closeButtonGO;
        CountdownTextClose = countdownTextCloseGO;
        
        CountdownTextClose.text = " ";
        CountdownTextClose.gameObject.SetActive(false);

        BackgroundPlane.SetActive(false);
        CloseButton.SetActive(false);

        PrefabsTransformParent = GameObject.Find("PrefabsParent");//oluþturduðumuz objelerin düzenli durmasýný ayný zamanda da hiçbir baþka parent'ý olamadýðýndan emin oluyoruz.

        PositionSetupVec = new Vector3(0, 0.03f, 0);//Görünen görüntünün biraz üzerinde yer almasý için yaptýðýmýz düzenleme.

        OpenFullScreen = false;
        IsVideoFullScreen = false;
        IsLittleVideoOpen = false;
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(0);
    }
}
