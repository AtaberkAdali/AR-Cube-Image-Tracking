using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Video;

//Script isimlendirmesine takýlmayýn. Sebebi çok fazla test yapmýþ olmam bu yüzden de kararlý sürüm bu oldu :D

[RequireComponent(typeof(ARTrackedImageManager))]//ImageManager olmadan çalýþmaz error vermesin diye.
public class ImageTrackingManager : MonoBehaviour
{ 
    private ARCameraManager arCameraManager;//ArCamera Manager Componenti Ayný zamanda kamera'nýn konumuna ulaþmak için de kullandým.
    private ARTrackedImageManager trackedImageManager;

    private MyVariblesF myVariblesF;//Get Set varibles Script instance
    private UIHandler uIHandler;//UIHandler Script instance

    [SerializeField] GameObject[] placeblePrefabs;//Bizim içinde video bulunan!!Gerekli!! plane veya objelerimiz.
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();//Bizim isimlerine göre iþlemler yaptðýmýz dictionary'miz.

    private void Awake()
    {
        arCameraManager = FindObjectOfType<ARCameraManager>();
        trackedImageManager = FindObjectOfType<ARTrackedImageManager>();
    }
    private void Start()
    {//Bunlarý start'da baþlatmamýn sebebi Singleton yaptým ve çakýþma olmasýný istemedim.
        myVariblesF = MyVariblesF.Instance;
        uIHandler = UIHandler.Instance;
        
        if (arCameraManager == null && trackedImageManager == null && myVariblesF == null && uIHandler == null)
            Debug.LogError("required object is null");

        myVariblesF.ObjectsInitialValues();//Objelerin ilk deðer atamalarýný gerçekleþtiriyoruz.
        InstantiatePlaceableObjectsAndAssigments();//Dictionary'miz için objeleri oluþturup ekliyoruz.
    }

    #region OnEnableOnDisable
    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += ImageChanged;//Buralarý zaten anlamýþsýnýzdýr :) Event muhabbeti
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= ImageChanged;
    }
    #endregion

    private void Update()
    {
        TouchControlForFullScreenUpdateMethod();//FullScreen'e geçmek için ekrana týkladý mý bunu kotrol ediyoruz.
    }

    private void ImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (ARTrackedImage trackedImage in args.added)
        {//Bir obje eklendiði zaman yani ilk kez gördüðünde çalýþýyor.
            UpdateImage(trackedImage);
        }
        foreach (ARTrackedImage trackedImage in args.updated)
        {//Obje güncellenince çalýþýyor.
            UpdateImage(trackedImage);
            Debug.Log("Updated");
        }
        foreach (ARTrackedImage trackedImage in args.removed)
        {//Obje kaldýrýldýðýnda çalýþýyor belki silinebilir ama ilerde lazým olur diye kalsýn.
            spawnedPrefabs[trackedImage.name].SetActive(false);
        }
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        if (myVariblesF.IsVideoFullScreen)//Video Full Screen ise diðer hiçbir þeyle ilgilenmiyoruz çünkü öncesinde atamalar yapýlmýþ oluyor ve biz videoyu kapatana kadar Burada karmaþa yaratmýyoruz :)
            return;
        if (trackedImage.trackingState is TrackingState.Limited || trackedImage.trackingState is TrackingState.None)
        {//Eðer ki obje düzgün okunamaz veya yoksa burda Videomuzu kapatýyoruz ve return;
            CloseVideo(spawnedPrefabs[trackedImage.referenceImage.name]);
            Debug.Log("Limited");
            return;
        }

        string name = trackedImage.referenceImage.name;//Güncel ismi alýyoruz ona göre iþlem yaptýracaðýz.
        //Image'in anlýk pozisyonunu alýp kendi objemizin görüneceði þekilde ekleme yapýyoruz. Ben image'imizden biraz daha yukarda görünmesini istedim.
        Vector3 imagePosition = trackedImage.transform.position + myVariblesF.PositionSetupVec;

        OpenVideo(spawnedPrefabs[name]);//Videomuzu Açýyoruz.
        //Ýþte burda kýyamet kopuyor. Objemizin full screen olup olmayacaðýný konumunu açýsýný her þeyini aslýnda burda yapýyoruz.
        SetFullScreen(spawnedPrefabs[name], imagePosition);
    }




    //------------------------------------------------Faydalý Fonksiyonlar--------------------------------------------------------------------------------//
    private void InstantiatePlaceableObjectsAndAssigments()
    {
        foreach (GameObject prefab in placeblePrefabs)//Objeleri oluþturuyoruz ve sonrasýnda bunlarý dictionary'mize ekliyoruz.
        {
            GameObject newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newPrefab.name = prefab.name;
            newPrefab.transform.SetParent(myVariblesF.PrefabsTransformParent.transform);//Burda düzenli durmasýný ve parent objesi olmadýðýndan emin oluyoruz.
            spawnedPrefabs.Add(prefab.name, newPrefab);
            spawnedPrefabs[newPrefab.name].SetActive(false);
        }
    }

    #region OpenOrCloseVideoFunctions
    private void OpenVideo(GameObject go)
    {
        if (go.GetComponent<VideoPlayer>() != null && !go.GetComponent<VideoPlayer>().isPlaying)//Null check for errors, isPlaying for performance.
        {
            go.SetActive(true);
            go.GetComponent<VideoPlayer>().time = 0f;
            go.GetComponent<VideoPlayer>().Play();
        }
        
    }
    private void CloseVideo(GameObject go)
    {
        go.SetActive(false);
        uIHandler.CloseFullScreenVideoPanel();
        myVariblesF.IsLittleVideoOpen = false;
    }
    #endregion

    #region FullScreenFunctions
    private void TouchControlForFullScreenUpdateMethod()
    {//Burasý update'de dönüyor. Eðer ki küçük video oynarken ekrana basarsa tam ekran yapýyoruz.
        if (Input.touchCount > 0)
        {//myVariblesF.IsLittleVideoOpen && 
            //Touch touch = Input.GetTouch(0);
            myVariblesF.OpenFullScreen = true;
        }
    }

    private void SetFullScreen(GameObject go, Vector3 position)
    {
        if (!myVariblesF.OpenFullScreen)
        {//Burda Full Screen'i kapatýyoruz.
            MakeVideoLittle(go, position);
        }
        else
        { //Burda full screen yapýyoruz.
            MakeVideoFullScreen(go);
        }
    }
    private void MakeVideoFullScreen(GameObject go)
    {
        if (go.transform.localScale.x <= 0.05f)//Scale'i deðiþtirirsen buraya da bir el at.
        {
            myVariblesF.IsVideoFullScreen = true;// Eðer ki video full screen olduysa burda iþlemi yaptýktan sonra resim gitse de izlenebiliyor.
            SettingPosAndRot(go);//Burda en boy oranýný ayarlayýp, Transformunu ayarlýyoruz.(pos,rotation,scale)
            uIHandler.OpenFullScreenVideoPanel(go.GetComponent<VideoPlayer>());//Burda arkaplaný vs açýyoruz.
            myVariblesF.IsLittleVideoOpen = false;
        }
    }
    private void MakeVideoLittle(GameObject go, Vector3 position)
    {
        if (go.transform.localScale.x >= 0.05f)//Scale'i deðiþtirirsen buraya da bir el at.
        {
            MakeDefault(go);//Küçük video ayarlarýný açýyoruz.
            uIHandler.CloseFullScreenVideoPanel();
        }
        myVariblesF.IsLittleVideoOpen = true;
        go.transform.position = position; //position
        go.transform.LookAt(arCameraManager.gameObject.transform);//Burada sürekli olarak kameramýza bakmasýný saðlýyoruz böylece farklý açýlarda ve daha güzel bir sunum oluyor.
        go.transform.Rotate(80, 0, 0);//Burada da zorunlu dönüdüyoruz görünmesi için.
    }
    #endregion

    #region PosAndRotationFunctions
    private void SettingPosAndRot(GameObject go)
    {
        go.transform.SetParent(arCameraManager.gameObject.transform);//Önce kameramýzýn child'ý yapýyoruz.
        go.transform.position = (arCameraManager.gameObject.transform.forward * 1.2f)+ arCameraManager.gameObject.transform.position;//Sonra pozisyonu kameramýzdan bir miktar uzakta ve güzel bir seyir sunacak þekilde ayarlýyoruz.
        go.transform.localRotation = Quaternion.Euler(90, 180, 0);//go.transform.localRotation = Quaternion.Euler(90, 0, 180); Bu da iþ yapýyor.
        SettingAspect(go);
    }
    private void SettingAspect(GameObject go)
    {//Videoya göre EnBoy oraný ayarlýyoruz.
        float videoWidth = go.GetComponent<VideoPlayer>().width;
        float videoHeight = go.GetComponent<VideoPlayer>().height;
        float videoAspect = videoWidth / videoHeight;
        go.transform.localScale = new Vector3(0.076f, 0.01f, (1 / videoAspect) * 0.076f);//Bunun sebebi z uzaklýðýna baðlý ve ben böyle ayarladým ilerde z'yi deðiþtirirsen bu boyutlandýrmayý da ayarla.
    }

    private void MakeDefault(GameObject go)
    {
        go.transform.SetParent(myVariblesF.PrefabsTransformParent.transform);
        go.transform.localScale = new Vector3(0.01f,0.01f,0.01f);//Küp'üm küçük olduðu için video da küçük burasý da ayarlanýlabilir.
    }
    #endregion

}
