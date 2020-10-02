using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

//Cette classe va se charger de détecter les images en RA et de lancer l'interaction correspondante :
// - Si on est dans la scene ppale, on lance le niveau correspondant,
// - Si on est dans une épreuve, on affiche juste l'oeuvre au bon endroit
[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTracker : MonoBehaviour
{
    public Vector3 posOffset;
    public Vector3 rotOffset;

    #region Setup


    ARTrackedImageManager m_TrackedImageManager;
    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        curScannedImage = null;
    }


    #endregion




    public UnityEvent onImageTracked, onImageLost;
    ARTrackedImage curScannedImage;

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach(var trackedImage in eventArgs.added)
        {
            UpdateInfo(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateInfo(trackedImage);
        }



    }

    private void UpdateInfo(ARTrackedImage trackedImage)
    {
        curScannedImage = trackedImage;
        onImageTracked?.Invoke();

        if(trackedImage.trackingState == TrackingState.Limited)
        {
            onImageLost?.Invoke();
        }

    }





    //Appelé par l'event onImageTracked dans la scène ppale
    public void LoadEpreuve()
    {
        if (!ScanPanelButtons.instance)
            return;

        if(!ScreenTransitionImageEffect.instance.isTransitioning && ScanPanelButtons.instance.isScanning && !DialogueEpreuveSystem.instance.isPlaying)
        {
            ScanPanelButtons.instance.ShowDialogueFirstScanThenLoadEpreuve(curScannedImage);
        }
    }

    //Appelé par l'event onImageTracked dans la scène d'épreuve pour déplacer l'oeuvre
    public void PlaceGameObjectOnTrackedImage(GameObject obj)
    {
        
        obj.SetActive(true);
        obj.transform.position = curScannedImage.transform.localPosition + posOffset;


    }

    //Appelé par l'event onImageTracked dans la scène d'épreuve pour déplacer l'oeuvre
    public void PlaceGameObjectOnTrackedImageWithRot(GameObject obj)
    {
        obj.SetActive(true);
        obj.transform.position = curScannedImage.transform.localPosition + posOffset;
        obj.transform.rotation = curScannedImage.transform.rotation * Quaternion.Euler(rotOffset);
    }



}
