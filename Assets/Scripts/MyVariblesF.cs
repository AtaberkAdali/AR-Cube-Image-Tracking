using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

//Buraya �ok yorum sat�r� yazm�yorum neden kullan�ld�klar� genelde belli.

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
                    _instance = new GameObject().AddComponent<MyVariblesF>();//E�er ki bu obje yoksa olu�turuyor.
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
    public bool OpenFullScreen { get; set; }//ekran� Full Screen Yapma veya normale d�nd�rme de�i�keni.
    public bool IsVideoFullScreen { get; set; }//full screen yapt�ktan sonra resim gitse de oynatmak i�in olan de�i�ken.
    public bool IsLittleVideoOpen { get; set; }//�u anda oynayan bir k���k video var m� bunu anlatan de�i�ken.
    public float VideoDuration { get; set; }//Video s�resi

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

        PrefabsTransformParent = GameObject.Find("PrefabsParent");//olu�turdu�umuz objelerin d�zenli durmas�n� ayn� zamanda da hi�bir ba�ka parent'� olamad���ndan emin oluyoruz.

        PositionSetupVec = new Vector3(0, 0.03f, 0);//G�r�nen g�r�nt�n�n biraz �zerinde yer almas� i�in yapt���m�z d�zenleme.

        OpenFullScreen = false;
        IsVideoFullScreen = false;
        IsLittleVideoOpen = false;
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(0);
    }
}
