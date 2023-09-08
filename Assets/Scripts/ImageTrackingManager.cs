using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Video;

//Script isimlendirmesine tak�lmay�n. Sebebi �ok fazla test yapm�� olmam bu y�zden de kararl� s�r�m bu oldu :D

[RequireComponent(typeof(ARTrackedImageManager))]//ImageManager olmadan �al��maz error vermesin diye.
public class ImageTrackingManager : MonoBehaviour
{ 
    private ARCameraManager arCameraManager;//ArCamera Manager Componenti Ayn� zamanda kamera'n�n konumuna ula�mak i�in de kulland�m.
    private ARTrackedImageManager trackedImageManager;

    private MyVariblesF myVariblesF;//Get Set varibles Script instance
    private UIHandler uIHandler;//UIHandler Script instance

    [SerializeField] GameObject[] placeblePrefabs;//Bizim i�inde video bulunan!!Gerekli!! plane veya objelerimiz.
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();//Bizim isimlerine g�re i�lemler yapt��m�z dictionary'miz.

    private void Awake()
    {
        arCameraManager = FindObjectOfType<ARCameraManager>();
        trackedImageManager = FindObjectOfType<ARTrackedImageManager>();
    }
    private void Start()
    {//Bunlar� start'da ba�latmam�n sebebi Singleton yapt�m ve �ak��ma olmas�n� istemedim.
        myVariblesF = MyVariblesF.Instance;
        uIHandler = UIHandler.Instance;
        
        if (arCameraManager == null && trackedImageManager == null && myVariblesF == null && uIHandler == null)
            Debug.LogError("required object is null");

        myVariblesF.ObjectsInitialValues();//Objelerin ilk de�er atamalar�n� ger�ekle�tiriyoruz.
        InstantiatePlaceableObjectsAndAssigments();//Dictionary'miz i�in objeleri olu�turup ekliyoruz.
    }

    #region OnEnableOnDisable
    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += ImageChanged;//Buralar� zaten anlam��s�n�zd�r :) Event muhabbeti
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= ImageChanged;
    }
    #endregion

    private void Update()
    {
        TouchControlForFullScreenUpdateMethod();//FullScreen'e ge�mek i�in ekrana t�klad� m� bunu kotrol ediyoruz.
    }

    private void ImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (ARTrackedImage trackedImage in args.added)
        {//Bir obje eklendi�i zaman yani ilk kez g�rd���nde �al���yor.
            UpdateImage(trackedImage);
        }
        foreach (ARTrackedImage trackedImage in args.updated)
        {//Obje g�ncellenince �al���yor.
            UpdateImage(trackedImage);
            Debug.Log("Updated");
        }
        foreach (ARTrackedImage trackedImage in args.removed)
        {//Obje kald�r�ld���nda �al���yor belki silinebilir ama ilerde laz�m olur diye kals�n.
            spawnedPrefabs[trackedImage.name].SetActive(false);
        }
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        if (myVariblesF.IsVideoFullScreen)//Video Full Screen ise di�er hi�bir �eyle ilgilenmiyoruz ��nk� �ncesinde atamalar yap�lm�� oluyor ve biz videoyu kapatana kadar Burada karma�a yaratm�yoruz :)
            return;
        if (trackedImage.trackingState is TrackingState.Limited || trackedImage.trackingState is TrackingState.None)
        {//E�er ki obje d�zg�n okunamaz veya yoksa burda Videomuzu kapat�yoruz ve return;
            CloseVideo(spawnedPrefabs[trackedImage.referenceImage.name]);
            Debug.Log("Limited");
            return;
        }

        string name = trackedImage.referenceImage.name;//G�ncel ismi al�yoruz ona g�re i�lem yapt�raca��z.
        //Image'in anl�k pozisyonunu al�p kendi objemizin g�r�nece�i �ekilde ekleme yap�yoruz. Ben image'imizden biraz daha yukarda g�r�nmesini istedim.
        Vector3 imagePosition = trackedImage.transform.position + myVariblesF.PositionSetupVec;

        OpenVideo(spawnedPrefabs[name]);//Videomuzu A��yoruz.
        //��te burda k�yamet kopuyor. Objemizin full screen olup olmayaca��n� konumunu a��s�n� her �eyini asl�nda burda yap�yoruz.
        SetFullScreen(spawnedPrefabs[name], imagePosition);
    }




    //------------------------------------------------Faydal� Fonksiyonlar--------------------------------------------------------------------------------//
    private void InstantiatePlaceableObjectsAndAssigments()
    {
        foreach (GameObject prefab in placeblePrefabs)//Objeleri olu�turuyoruz ve sonras�nda bunlar� dictionary'mize ekliyoruz.
        {
            GameObject newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newPrefab.name = prefab.name;
            newPrefab.transform.SetParent(myVariblesF.PrefabsTransformParent.transform);//Burda d�zenli durmas�n� ve parent objesi olmad���ndan emin oluyoruz.
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
    {//Buras� update'de d�n�yor. E�er ki k���k video oynarken ekrana basarsa tam ekran yap�yoruz.
        if (Input.touchCount > 0)
        {//myVariblesF.IsLittleVideoOpen && 
            //Touch touch = Input.GetTouch(0);
            myVariblesF.OpenFullScreen = true;
        }
    }

    private void SetFullScreen(GameObject go, Vector3 position)
    {
        if (!myVariblesF.OpenFullScreen)
        {//Burda Full Screen'i kapat�yoruz.
            MakeVideoLittle(go, position);
        }
        else
        { //Burda full screen yap�yoruz.
            MakeVideoFullScreen(go);
        }
    }
    private void MakeVideoFullScreen(GameObject go)
    {
        if (go.transform.localScale.x <= 0.05f)//Scale'i de�i�tirirsen buraya da bir el at.
        {
            myVariblesF.IsVideoFullScreen = true;// E�er ki video full screen olduysa burda i�lemi yapt�ktan sonra resim gitse de izlenebiliyor.
            SettingPosAndRot(go);//Burda en boy oran�n� ayarlay�p, Transformunu ayarl�yoruz.(pos,rotation,scale)
            uIHandler.OpenFullScreenVideoPanel(go.GetComponent<VideoPlayer>());//Burda arkaplan� vs a��yoruz.
            myVariblesF.IsLittleVideoOpen = false;
        }
    }
    private void MakeVideoLittle(GameObject go, Vector3 position)
    {
        if (go.transform.localScale.x >= 0.05f)//Scale'i de�i�tirirsen buraya da bir el at.
        {
            MakeDefault(go);//K���k video ayarlar�n� a��yoruz.
            uIHandler.CloseFullScreenVideoPanel();
        }
        myVariblesF.IsLittleVideoOpen = true;
        go.transform.position = position; //position
        go.transform.LookAt(arCameraManager.gameObject.transform);//Burada s�rekli olarak kameram�za bakmas�n� sa�l�yoruz b�ylece farkl� a��larda ve daha g�zel bir sunum oluyor.
        go.transform.Rotate(80, 0, 0);//Burada da zorunlu d�n�d�yoruz g�r�nmesi i�in.
    }
    #endregion

    #region PosAndRotationFunctions
    private void SettingPosAndRot(GameObject go)
    {
        go.transform.SetParent(arCameraManager.gameObject.transform);//�nce kameram�z�n child'� yap�yoruz.
        go.transform.position = (arCameraManager.gameObject.transform.forward * 1.2f)+ arCameraManager.gameObject.transform.position;//Sonra pozisyonu kameram�zdan bir miktar uzakta ve g�zel bir seyir sunacak �ekilde ayarl�yoruz.
        go.transform.localRotation = Quaternion.Euler(90, 180, 0);//go.transform.localRotation = Quaternion.Euler(90, 0, 180); Bu da i� yap�yor.
        SettingAspect(go);
    }
    private void SettingAspect(GameObject go)
    {//Videoya g�re EnBoy oran� ayarl�yoruz.
        float videoWidth = go.GetComponent<VideoPlayer>().width;
        float videoHeight = go.GetComponent<VideoPlayer>().height;
        float videoAspect = videoWidth / videoHeight;
        go.transform.localScale = new Vector3(0.076f, 0.01f, (1 / videoAspect) * 0.076f);//Bunun sebebi z uzakl���na ba�l� ve ben b�yle ayarlad�m ilerde z'yi de�i�tirirsen bu boyutland�rmay� da ayarla.
    }

    private void MakeDefault(GameObject go)
    {
        go.transform.SetParent(myVariblesF.PrefabsTransformParent.transform);
        go.transform.localScale = new Vector3(0.01f,0.01f,0.01f);//K�p'�m k���k oldu�u i�in video da k���k buras� da ayarlan�labilir.
    }
    #endregion

}
