using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UT_gallery : MonoBehaviour
{
    public GameObject nextButton;
    public GameObject prevButton;
    public GameObject skipToEndButton;
    public GameObject skipToStartButton;
    public bool loopPagination;
    public bool enableArrowKeys;
    public GameObject[] Pages;

    [Space(10)]
    public bool enableCoinNav;
    public GameObject coinNavContainer;
    public float coinNavHorizontalPadding = 100;
    public float coinNavVerticalPadding = 100;
    private RectTransform coinNavRect;
    public GameObject coinNavSeed;
    public float coinNavImgScale = 1f;
    public Sprite coinNavImgSelected;
    public Sprite coinNavImgUnselected;
    private GameObject[] coinNavArray;
    private RectTransform[] coinNavSeedRectArray;

    [HideInInspector]
    public int pageIndex;
    private bool firstPage;
    private bool lastPage;

    void OnEnable()
    {
        FirstPage();

        if (enableCoinNav)
        {
            EnableCoinNav();
            UpdateCoinNav();
        }
        else
        {
            DisableCoinNav();
        }
    }

    private void Update()
    {
        if (enableCoinNav)
        {
            //Update coin nav size from user input
            coinNavRect.sizeDelta = new Vector2(coinNavHorizontalPadding, coinNavVerticalPadding);
        }

        if (enableArrowKeys)
        {
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                NextPage();
            }

            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                PrevPage();
            }
        }
    }

    public void FirstPage()
    {
        ResetPages();
        pageIndex = 0;
        Pages[pageIndex].SetActive(true);
        //If there's only one page, show no buttons, otherwise, show/hide accordingly
        if (Pages.Length > 1)
        {
            if (loopPagination)
            {
                if (nextButton != null) { nextButton.SetActive(true); }
                if (prevButton != null) { prevButton.SetActive(true); }
                if (skipToEndButton != null) { skipToEndButton.SetActive(true); }
                if (skipToStartButton != null) { skipToStartButton.SetActive(true); }
            }
            else
            {
                if (nextButton != null) { nextButton.SetActive(true); }
                if (prevButton != null) { prevButton.SetActive(false); }
                if (skipToEndButton != null) { skipToEndButton.SetActive(true); }
                if (skipToStartButton != null) { skipToStartButton.SetActive(false); }
            }
        }
        else //Hide all buttons if there's only one page
        {
            if (nextButton != null) { nextButton.SetActive(false); }
            if (prevButton != null) { prevButton.SetActive(false); }
            if (skipToEndButton != null) { skipToEndButton.SetActive(false); }
            if (skipToStartButton != null) { skipToStartButton.SetActive(false); }
        }

        lastPage = false;
        firstPage = true;
    }

    public void LastPage()
    {
        ResetPages();
        pageIndex = Pages.Length - 1;
        Pages[pageIndex].SetActive(true);
        if (loopPagination)
        {
            if (nextButton != null) { nextButton.SetActive(true); }
            if (prevButton != null) { prevButton.SetActive(true); }
            if (skipToEndButton != null) { skipToEndButton.SetActive(true); }
            if (skipToStartButton != null) { skipToStartButton.SetActive(true); }
        }
        else
        {
            if (nextButton != null) { nextButton.SetActive(false); }
            if (prevButton != null) { prevButton.SetActive(true); }
            if (skipToEndButton != null) { skipToEndButton.SetActive(false); }
            if (skipToStartButton != null) { skipToStartButton.SetActive(true); }
        }
        lastPage = true;
        firstPage = false;
    }

    public void NextPage()
    {
        if (!lastPage) //This isnt getting called when clicking to last page from first page
        {
            if (pageIndex < Pages.Length - 1)//Clicking upward through pages
            {
                ResetPages();
                firstPage = false;
                lastPage = false;
                pageIndex++;
                Pages[pageIndex].SetActive(true);
            }

            if (pageIndex == Pages.Length - 1) //Reached last page by clicking Next
            {
                LastPage();
            }

        }
        else //If this is the last page, then reset page back to first
        {
            ResetPages();
            if (loopPagination)
            {
                //Make sure there's at least more than one page before we attempt to adjust the index
                if (Pages.Length > 1)
                {
                    //pageIndex = 0;
                    //lastPage = false;
                    FirstPage();
                }

            }
            else //Stay on same page
            {
                pageIndex = Pages.Length - 1;
                lastPage = true;
            }
            Pages[pageIndex].SetActive(true);
        }

        if (prevButton != null) { prevButton.SetActive(true); }
        if (skipToStartButton != null) { skipToStartButton.SetActive(true); }

        if (enableCoinNav)
        {
            UpdateCoinNav();
        }
            
    }

    public void PrevPage()
    {
        if (!firstPage)
        {
            if (pageIndex >= 0)//Clicking upward through pages
            {
                ResetPages();
                firstPage = false;
                lastPage = false;
                pageIndex--;
                Pages[pageIndex].SetActive(true);
            }

            if (pageIndex <= 0) //Reached last page by clicking Next
            {
                FirstPage();
            }
        }
        else //If this is the last page, then reset page back to first
        {
            ResetPages();
            if (loopPagination)
            {
                //Make sure there's at least more than one page before we attempt to adjust the index
                if (Pages.Length > 1)
                {
                    pageIndex = Pages.Length - 1;
                    firstPage = false;
                }

                if (pageIndex == Pages.Length - 1)
                {
                    lastPage = true;
                }

                if (pageIndex <= 0)
                {
                    FirstPage();
                }
            }
            else //Stay on same page
            {
                pageIndex = 0;
                firstPage = true;
            }
            Pages[pageIndex].SetActive(true);

        }

        if (nextButton != null) { nextButton.SetActive(true); }
        if (skipToEndButton != null) { skipToEndButton.SetActive(true); }

        if (enableCoinNav)
        {
            UpdateCoinNav();
        }
    }

    public void CloseGalllery()
    {
        gameObject.SetActive(false);
    }

    void ResetPages()
    {
        int i = 0;
        foreach (GameObject subPage in Pages)
        {
            Pages[i].SetActive(false);
            i++;
        }
    }

    void EnableCoinNav()
    {
        if (coinNavContainer != null)
        {
            //Set length for Coin Nav Array
            coinNavArray = new GameObject[Pages.Length];

            //Set RectTransform for Coin Nav Container
            coinNavRect = coinNavContainer.GetComponent<RectTransform>();

            //Set array length for Coin Nav Seed
            coinNavSeedRectArray = new RectTransform[Pages.Length];

            int i = 0;
            foreach (GameObject subPage in Pages)
            {
                //Clone seed and to Coin Nav Array
                coinNavArray[i] = Instantiate(coinNavSeed, transform.position, Quaternion.identity) as GameObject;

                //Parent seed clone to Coin Nav Container
                coinNavArray[i].transform.SetParent(coinNavContainer.transform);


                //Set RectTransform for this Coin Nav Seed
                coinNavSeedRectArray[i] = coinNavArray[i].GetComponent<RectTransform>();
                coinNavSeedRectArray[i].localScale = new Vector3(coinNavImgScale, coinNavImgScale, 1f);

                i++;
            }


            coinNavSeed.SetActive(false);
        }
    }

    void DisableCoinNav()
    {
        if (coinNavContainer != null)
        {
            coinNavContainer.SetActive(false);
        }
    }

    void UpdateCoinNav()
    {
        int i = 0;
        foreach (GameObject subPage in Pages)
        {
            if (i == pageIndex)
            {
                //Set highlight image
                Image coinNavImage = coinNavArray[i].GetComponent<Image>();
                coinNavImage.sprite = coinNavImgSelected;
            }
            else
            {
                //Set inactive image
                Image coinNavImage = coinNavArray[i].GetComponent<Image>();
                coinNavImage.sprite = coinNavImgUnselected;
            }

            i++;
        }
    }
}
