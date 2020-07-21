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
    }


    #endregion




    public UnityEvent onImageTracked;
    ARTrackedImage curScannedImage;

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
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
        // Disable the visual plane if it is not being tracked
        if (trackedImage.trackingState != TrackingState.Tracking)
        {
            onImageTracked?.Invoke();
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
        //A tester pour voir si les objets s'affichent mieux maintenant
        curScannedImage.transform.position = Vector3.one * .1f;

        print(curScannedImage.name + "  " + Vector3.Distance(obj.transform.position, curScannedImage.transform.position));
        obj.SetActive(true);
        obj.transform.position = curScannedImage.transform.localPosition;
        //obj.transform.rotation = curScannedImage.transform.rotation;
    }
}
