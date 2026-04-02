using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics.Contracts;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static UIManager instance;
    public GameObject UICanvas;
    public List<GameObject> UIPages;
    [SerializeField] EventSystem eventSystem; 
    public Transform InventoryLayout;
    public GameObject pfItem2D;
    [SerializeField] string CurrentUIPage;
    [SerializeField] private string PreviousPage;

    public void NavigateP(string uiPageTag)
    {
        PreviousPage = CurrentUIPage;
        CurrentUIPage = uiPageTag;
        //set all pages inactive except selected "uiPage"
        for (int i = 0; i <= UIPages.Count - 1; i++)
        {
            if (!UIPages[i].CompareTag(uiPageTag))
            {
                UIPages[i].SetActive(false);
            }
            else
            {
                UIPages[i].SetActive(true);
            }
        }
        if (CurrentUIPage == "InGame")
        {
            Time.timeScale = 1f;
            GameObject.Find("Player").GetComponent<CamRotate>().enabled = true;
        }
    }
    void Awake()
    {
        //Make sure UIManager is a don't be destroyed
        if (!ScriptManager.MakeManager(this, ref instance))
            return;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("UIManager alive: " + GetInstanceID());
    }


    private void Update()
    {
        CheckButton();
    }
    public void Quit()
    {
        Application.Quit();
    }
    
    public void CheckButton()
    {
        if (Input.GetButtonDown("Pause" ) && LevelSceneManager.currentLevel != 0)
        {
            if (CurrentUIPage == "InGame")
            {
                NavigateP("Pause");
                Time.timeScale = 0f;
                GameObject.Find("Player").GetComponent<CamRotate>().enabled = false;
            }
            else
            {
                NavigateP("InGame");
            }
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode = LoadSceneMode.Single)
    { 
        if (LevelSceneManager.currentLevel != 0)
        {
            NavigateP("InGame");
            
        }
        else
        {
            NavigateP("Menu");
        }
        foreach (Transform item in InventoryLayout)
        {
            Destroy(item.gameObject);
        }

    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void UpdateItemList(List<ItemData> itemList)
    {
        foreach (Transform item in InventoryLayout)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in itemList)
        { 
            GameObject obj = Instantiate(pfItem2D,InventoryLayout);
            var itemIcon = obj.GetComponent<Image>();

            itemIcon.sprite = item.icon;
        }
    }

    public void BackPreviousPage()
    {
        NavigateP(PreviousPage);
    }

    public void GameOverUI()
    {
        Color ColorFrom = Color.red;
        Color ColorTo = new Color(0f, 0f, 0f, 1f);
        // Find the LosePage GameObject by tag or name
        GameObject losePage = UIPages.FirstOrDefault(page => page.CompareTag("Lose"));
        if (losePage == null) return;

        // Find the BG child (assume named "BG")
        Transform bgTransform = losePage.transform.Find("BG");
        if (bgTransform == null) return;

        Image bgImage = bgTransform.GetComponent<Image>();
        if (bgImage == null) return;

        // Start the color transition coroutine
        StartCoroutine(EaseBGColor(bgImage, ColorFrom, ColorTo, 2f));
    }

    private IEnumerator EaseBGColor(Image bgImage, Color fromColor, Color toColor, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            bgImage.color = Color.Lerp(fromColor, toColor, elapsed / duration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        bgImage.color = toColor;
    }
}
